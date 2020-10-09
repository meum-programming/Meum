using System;
using System.Collections;
using System.Collections.Generic;
using UI;
using UI.BuilderScene;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace Gallery.Builder
{
    public class ArtworkPlacer : MonoBehaviour
    {
        [Header("Placing Info")]
        [SerializeField] private GameObject paintPrefab;
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
        private float _defaultScaleY = 0.0f;

        private Transform _selected = null;
        private bool _moving = false;

        private void Awake()
        {
            _defaultScaleY = paintPrefab.transform.localScale.y;
        }

        public void Select(UI.Content data)
        {
            var pos = Input.mousePosition;
            pos.z = 10.0f;
            pos = Camera.main.ScreenToWorldPoint(pos);

            _selected = Instantiate(paintPrefab, pos, Quaternion.identity, transform).transform;
            var paintInfo = _selected.GetComponent<ArtworkInfo>();
            paintInfo.SetTexture(data.image.texture as Texture2D);
            paintInfo.url = data.data.thumbnail_url;

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
            if(toggleButton.toggled)
                toggleButton.InvokeButton();
            toggleButton.SetButtonEnable(false);
        }

        private void StopMovingObj()
        {
            _moving = false;
            toggleButton.InvokeButton();
            toggleButton.SetButtonEnable(true);
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
                var layermask = 1 << LayerMask.NameToLayer("Wall");
                if(!_moving) layermask += 1 << LayerMask.NameToLayer("Paint");
                if (Physics.Raycast(ray, out var hit, 100.0f, layermask))
                {
                    var objectHit = hit.transform;
                    
                    if (_selected && _moving && objectHit.CompareTag("Wall"))
                        AlignPaintTransform(_selected, hit);
                    
                    if (Input.GetMouseButtonDown(0))
                    {
                        if (_selected && _moving && objectHit.CompareTag("Wall"))
                        {
                            _selected = null;
                            StopMovingObj();
                        }
                        else if (objectHit.CompareTag("Paint"))
                        {
                            _selected = objectHit;
                            actionSelector.SetSelected(_selected);
                            _moving = false;
                        }
                    }
                }
            }

            var scrollDelta = Input.GetAxis("Mouse ScrollWheel");
            if (_selected && _moving && Mathf.Abs(scrollDelta) > 1e-10)
            {
                var scale = _selected.localScale;
                var ratio = scale.x / scale.y;
                _nowScale = Mathf.Clamp(_nowScale + scrollDelta * scaleFactor, 0, 1);
                scale.y = Mathf.Lerp(_defaultScaleY / scaleLimit, _defaultScaleY * scaleLimit, _nowScale);
                scale.x = scale.y * ratio;
                _selected.localScale = scale;
            }
        }

        private void AlignPaintTransform(Transform paint, RaycastHit hit)
        {
            var placePosition = hit.point;
            var objectHit = hit.transform;
            var wallInfo = objectHit.GetComponent<WallInfo>();
            Debug.Assert(wallInfo);
            var normVec = wallInfo.GetNormalDir(placePosition, cam.transform.position);
            var distanceFromCenterY = hit.point.y - objectHit.transform.position.y;
            placePosition.y = objectHit.transform.position.y + SnapFloat(distanceFromCenterY, snapGridSizeY);
            placePosition += normVec * wallInfo.placeDistance;
            paint.rotation = Quaternion.LookRotation(normVec);
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
    }
}
