using System;
using System.Collections;
using System.Collections.Generic;
using UI;
using UI.BuilderScene;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace Gallery.Builder
{
    public class ArtworkPlacer : MonoBehaviour
    {
        [Header("Placing Info")]
        [SerializeField] private GameObject paintPrefab;
        [SerializeField] private GameObject object3DPrefab;
        [SerializeField] private Camera cam;
        [SerializeField] private float snapGridSizeY;

        [Header("Referencing Objs")] 
        [SerializeField] private SidebarToggleButton toggleButton;
        [SerializeField] private PlacerActionSelector actionSelector;
        [SerializeField] private BuilderSceneVerifyModals verifyModalManager;
        
        [Header("Image scaling")] 
        [SerializeField] private float scaleFactor;
        [SerializeField] private float scaleLimit;
        private float _nowScale = 0.5f;
        private float _defaultScaleZ = 0.0f;

        private Transform _selected = null;
        private bool _moving = false;
        private bool _nowEditing3D = false;

        public void Select(Content data)
        {
            var pos = Input.mousePosition;
            pos.z = 10.0f;
            pos = Camera.main.ScreenToWorldPoint(pos);

            _selected = Instantiate(paintPrefab, pos, Quaternion.identity, transform).transform;
            var paintInfo = _selected.GetComponent<ArtworkInfo>();
            paintInfo.SetupWithContent(data);

            _nowEditing3D = false;
            StartMovingObj();
        }

        public void Select3D(Content data)
        {
            var pos = Input.mousePosition;
            pos.z = 10.0f;
            pos = Camera.main.ScreenToWorldPoint(pos);

            _selected = Instantiate(object3DPrefab, pos, Quaternion.identity, transform).transform;
            var object3DInfo = _selected.GetComponent<ArtworkInfo>();
            object3DInfo.SetupWithContent(data);

            _nowEditing3D = true;
            StartMovingObj();
        }

        public void SelectBanner(Content data, string url)
        {
            var pos = Input.mousePosition;
            pos.z = 10.0f;
            pos = Camera.main.ScreenToWorldPoint(pos);

            _selected = Instantiate(paintPrefab, pos, Quaternion.identity, transform).transform;
            var paintInfo = _selected.GetComponent<ArtworkInfo>();
            paintInfo.SetupBannerWithContent(data, url);

            StartMovingObj();
        }

        public void Deselect()
        {
            actionSelector.Deselect();
            _selected = null;
            _moving = false;
        }

        public void StartMovingObj()
        {
            _moving = true;
            ResetScale();
            
            if(toggleButton.toggled)
                toggleButton.InvokeButton();
            toggleButton.SetButtonEnable(false);

            _selected.gameObject.layer = LayerMask.NameToLayer("NotPlaced");
        }

        private void StopMovingObj()
        {
            _moving = false;
            if(!toggleButton.toggled)
                toggleButton.InvokeButton();
            toggleButton.SetButtonEnable(true);
            
            _selected.gameObject.layer = LayerMask.NameToLayer("Paint");
        }

        public void DeleteSelected()
        {
            if (_selected)
            {
                Destroy(_selected.gameObject);
                _selected = null;
            }
        }

        private void Update()
        {
            if (verifyModalManager.showingModal) return;
            
            if (_selected || Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.X))
            {
                var ray = cam.ScreenPointToRay(Input.mousePosition);
                var layermask = (1 << LayerMask.NameToLayer("Placable")) + (1 << LayerMask.NameToLayer("Paint"));
                // if(!_moving) layermask += 1 << LayerMask.NameToLayer("Paint");
                if (Physics.Raycast(ray, out var hit, 100.0f, layermask))
                {
                    var objectHit = hit.transform;
                    
                    // if (_selected && _moving && objectHit.CompareTag("Wall"))
                    if (_selected && _moving && (objectHit.CompareTag("Paint") || objectHit.CompareTag("Wall")))
                        AlignPaintTransform(_selected, hit);
                    if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
                    {
                        if (_selected && _moving)
                        {
                            StopMovingObj();
                            _selected = null;
                        }
                        else if (objectHit.CompareTag("Paint"))
                        {
                            _selected = objectHit;
                            actionSelector.SetSelected(_selected);
                            _nowEditing3D = objectHit.GetComponent<ArtworkInfo>().artworkInfo.type_artwork == 1;
                            _moving = false;
                        }
                        else
                            Deselect();
                    }
                }
                else if (!_moving && Input.GetMouseButtonDown(0))
                    Deselect();
            }

            var scrollDelta = Input.GetAxis("Mouse ScrollWheel");
            if (_selected && _moving && Mathf.Abs(scrollDelta) > 1e-10)
            {
                var scale = _selected.localScale;
                var ratioX = scale.x / scale.z;
                var ratioY = scale.y / scale.z;
                _nowScale = Mathf.Clamp(_nowScale + scrollDelta * scaleFactor, 0, 1);
                scale.z = Mathf.Lerp(_defaultScaleZ / scaleLimit, _defaultScaleZ * scaleLimit, _nowScale);
                scale.x = scale.z * ratioX;
                if(_nowEditing3D)
                    scale.y = scale.z * ratioY;
                _selected.localScale = scale;
            }

            if (_selected && _moving)
            {
                if (Input.GetKeyDown(KeyCode.Z))
                    _selected.rotation = Quaternion.Euler(_selected.up * 90.0f) * _selected.rotation;
                if (Input.GetKeyDown(KeyCode.X))
                    _selected.rotation = Quaternion.Euler(_selected.up * -90.0f) * _selected.rotation;
            }
        }

        private void AlignPaintTransform(Transform paint, RaycastHit hit)
        {
            var placePosition = hit.point;
            var objectHit = hit.transform;
            var wallInfo = objectHit.GetComponent<WallInfo>();
            // Debug.Assert(wallInfo);
            // var normVec = wallInfo.GetNormalDir(placePosition, cam.transform.position);
            var distanceFromCenterY = hit.point.y - objectHit.transform.position.y;
            placePosition.y = objectHit.transform.position.y + SnapFloat(distanceFromCenterY, snapGridSizeY);
            paint.rotation = Quaternion.FromToRotation(paint.up, hit.normal) * paint.rotation;
            paint.position = placePosition;
        }

        private float SnapFloat(float v, float gridSize)
        {
            var precision = 100000;
            var gridSizeRounded = Mathf.RoundToInt(gridSize * precision);
            float result = Mathf.RoundToInt(v * precision) / gridSizeRounded * gridSizeRounded;
            result /= precision;
            return result;
        }

        private void ResetScale()
        {
            _defaultScaleZ = _selected.transform.localScale.z;
            _nowScale = (_defaultScaleZ - _defaultScaleZ / scaleLimit)
                        / (_defaultScaleZ * scaleLimit - _defaultScaleZ / scaleLimit);
        }
    }
}
