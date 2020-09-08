using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace Gallery.Builder
{
    public class PaintPlacer : MonoBehaviour
    {
        [SerializeField] private GameObject paintPrefab;
        [SerializeField] private Camera cam;

        [Header("Image scaling")] [SerializeField]
        private float scaleFactor;

        [SerializeField] private float scaleLimit;
        private float _nowScale = 0.5f;
        private float _defaultScaleY = 0.0f;

        [Header("Image Picking")] [SerializeField]
        private GameObject modalPannel;

        [SerializeField] private InputField modalInputfield;

        private Transform _selected = null;
        private bool _modalShowing = false;

        private void Awake()
        {
            _defaultScaleY = paintPrefab.transform.localScale.y;

            modalInputfield.onEndEdit.RemoveAllListeners();
            modalInputfield.onEndEdit.AddListener(SelectImage);
        }

        private void SelectImage(string url)
        {
            var paintInfo = _selected.GetComponent<PaintInfo>();
            paintInfo.URL = url;
            SwitchModal();
        }

        private void SwitchModal()
        {
            _modalShowing = !_modalShowing;
            modalPannel.SetActive(_modalShowing);
            modalInputfield.text = "";
        }

        private void Update()
        {
            if (_modalShowing) return;

            if (_selected || Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.X))
            {
                var ray = cam.ScreenPointToRay(Input.mousePosition);
                var layermask = 1 << LayerMask.NameToLayer("Wall");
                if (_selected == null) layermask += 1 << LayerMask.NameToLayer("Paint");
                if (Physics.Raycast(ray, out var hit, 100.0f, layermask))
                {
                    var objectHit = hit.transform;
                    if (_selected && objectHit.CompareTag("Wall"))
                        AlignPaintTransform(_selected, hit);
                    if (Input.GetMouseButtonDown(0))
                    {
                        if (objectHit.CompareTag("Wall"))
                        {
                            if (_selected)
                                _selected = null;
                            else
                            {
                                _selected = Instantiate(paintPrefab, hit.point, Quaternion.identity, transform)
                                    .transform;
                                AlignPaintTransform(_selected, hit);
                                SwitchModal();
                            }
                        }
                        else if (objectHit.CompareTag("Paint"))
                            _selected = objectHit;
                    }
                    else if (Input.GetKeyDown(KeyCode.X))
                    {
                        if (_selected)
                        {
                            Destroy(_selected.gameObject);
                            _selected = null;
                        }
                        else if (objectHit.CompareTag("Paint"))
                        {
                            Destroy(objectHit.gameObject);
                        }
                    }
                }
            }

            var scrollDelta = Input.GetAxis("Mouse ScrollWheel");
            if (_selected && Mathf.Abs(scrollDelta) > 1e-10)
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
            Vector3 placePosition = hit.point;
            var objectHit = hit.transform;
            var wallInfo = objectHit.GetComponent<WallInfo>();
            Debug.Assert(wallInfo);
            var normVec = wallInfo.GetNormalDir(cam.transform.position);
            placePosition.y = objectHit.transform.position.y + wallInfo.placeHeight;
            placePosition += normVec * wallInfo.placeDistance;
            _selected.rotation = Quaternion.LookRotation(normVec);
            _selected.position = placePosition;
        }
    }
}
