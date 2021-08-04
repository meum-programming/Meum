using UI.BuilderScene;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using DG.Tweening;
using System.Collections;
using UnityEngine.Events;

namespace Game.Artwork
{
    /*
     * @brief Artwork를 설치하기 위한 컴포넌트
     * @details Artwork의 선택, 설치, 이동, 제거의 기능을 함
     */
    public class ArtworkPlacer : MonoBehaviour
    {
        
        [SerializeField] private SidebarToggleButton toggleButton;
        [SerializeField] private PlacerActionSelector actionSelector;
        [SerializeField] private BuilderSceneVerifyModals verifyModalManager;
        
        [Header("Placing Info")]
        [SerializeField] private GameObject paintPrefab;
        [SerializeField] private GameObject object3DPrefab;
        [SerializeField] private Camera cam;
        [SerializeField] private float snapGridSizeY;

        [Header("Image scaling")] 
        [SerializeField] private float scaleFactor;
        [SerializeField] private float scaleLimit;

        private float _nowScale = 0.5f;
        private float _defaultScaleZ = 0.0f;
        public Transform _selected = null;
        private bool _moving = false;
        private bool _nowEditing3D = false;

        Tween scaleTween = null;
        Vector3 hitNormal = Vector3.zero;
        bool hitNormalReady = false;

        /*
         * @brief 선택한 오브젝트를 반환 
         */
        public Transform GetSelected()
        {
            return _selected;
        }
        
        /*
         * @brief 선택된 UI.Content에 해당하는 Artwork 오브젝트를 생성
         */
        public void CreateSelected(UI.ContentViewer.Content data)
        {
            Vector3 pos = Input.mousePosition;
            pos.z = -10.0f;
            pos = cam.ScreenToWorldPoint(pos);

            _nowEditing3D = data.Data.type_artwork == 1;

            if (_selected != null)
            {
                LayerSet("Placeable");
            }

            _selected = Instantiate(_nowEditing3D ? object3DPrefab : paintPrefab, pos, Quaternion.identity, transform).transform;
            Assert.IsNotNull(_selected);
            var artworkInfo = _selected.GetComponent<ArtworkInfo>();
            Assert.IsNotNull(artworkInfo);

            artworkInfo.UpdateWithArtworkContent(data , ()=> CreateCompleteOn());
        }

        void CreateCompleteOn()
        {
            if (_nowEditing3D)
            {
                _selected = _selected.GetComponentInChildren<MeshRenderer>().transform;
            }

            StartMovingObj();
        }
        
        /*
         * @brief 선택된 UI.Content에 해당하는 2D Artwork 오브젝트를 배너로 생성
         */
        public void CreateSelectedBanner(UI.ContentViewer.Content data, string url)
        {
            Assert.IsTrue(data.Data.type_artwork == 0);

            Vector3 pos = Input.mousePosition;
            pos.z = -10.0f;
            pos = cam.ScreenToWorldPoint(pos);

            if (_selected != null)
            {
                LayerSet("Placeable");
            }

            _selected = Instantiate(paintPrefab, pos, Quaternion.identity, transform).transform;
            Assert.IsNotNull(_selected);
            var paintInfo = _selected.GetComponent<ArtworkInfo>();
            Assert.IsNotNull(paintInfo);
            paintInfo.UpdateWithArtworkContent(data);
            paintInfo.bannerUrl = url;

            StartMovingObj();
        }
        
        public void Deselect()
        {
            StopMovingObj();
            actionSelector.Deselect();
            _selected = null;
        }
        
        /*
         * @brief 선택된 오브젝트를 움직이기 시작 (처음 선택했을 때는 action selector만 나옴)
         */
        public void StartMovingObj()
        {
            _moving = true;
            ResetScale();
            
            toggleButton.ToggleChange(true);

            _selected.gameObject.layer = LayerMask.NameToLayer("NotPlaced");
        }
        
        /*
         * @brief 선택된 오브젝트를 그만 움직이도록 함
         */
        private void StopMovingObj()
        {
            _moving = false;
            
            toggleButton.ToggleChange(false);
            
            if(_selected)
                _selected.gameObject.layer = LayerMask.NameToLayer("Placeable");
        }

        public void DeleteSelected()
        {
            if (_selected)
            {
                StopMovingObj();

                //2D 오브젝트라면
                if (_selected.GetComponent<ArtworkInfo>())
                {
                    Destroy(_selected.gameObject);
                }
                else
                {
                    Destroy(_selected.parent.gameObject);
                }

                _selected = null;
            }
        }

        
        
        /*
         * @brief Select(클릭) 이벤트에 대한 콜백
         */
        public void OnSelect()
        {
            
            if (!Input.GetMouseButtonUp(0))
                return;
            
            if (verifyModalManager.showingModal) return;
            if (EventSystem.current.IsPointerOverGameObject()) // 마우스가 UI Element 위에 있는경우 아무것도 하지 않음
                return;

            var ray = cam.ScreenPointToRay(Input.mousePosition);
            var mask = 1 << LayerMask.NameToLayer("Placeable");
            if (Physics.Raycast(ray, out var hit, 100.0f, mask))
            {
                var objectHit = hit.transform;
                
                if (!_moving && objectHit.CompareTag("Paint"))
                {
                    ArtworkInfo artworkInfo = null;

                    if (_selected != null)
                    {
                        LayerSet("Placeable");
                    }

                    _selected = objectHit;
                    actionSelector.SetSelected(_selected);

                    artworkInfo = GetArtWorkInfo();

                    _nowEditing3D = artworkInfo.ArtworkType == 1;
                    _moving = false;

                    StartCoroutine(HitNomalValueSet()); 
                }
                else
                {
                    if (!actionSelector.dragOn)
                    {
                        Deselect();
                    }
                }

            }
            else if(!_moving)
                Deselect();
        }

        ArtworkInfo GetArtWorkInfo()
        {
            if (_selected == null)
            {
                return null;
            }

            ArtworkInfo artworkInfo = _selected.GetComponent<ArtworkInfo>();

            //3D 오브젝트는 부모오브젝트에 ArtworkInfo클래스를 가지고 있다
            if (artworkInfo == null)
            {
                artworkInfo = _selected.GetComponentInParent<ArtworkInfo>();
            }

            return artworkInfo;
        }

        /*
         * @brief Rotate 이벤트에 대한 콜백
         */
        public void OnRotateArtwork()
        {
            if (!_selected || !_moving)
                return;


            float value = 0;

            if (Input.GetKeyDown(KeyCode.Z))
            {
                value = -1;
            }
            else if (Input.GetKeyDown(KeyCode.X))
            {
                value = 1;
            }

            if (value != 0)
            {
                RotateSelected(value);
            }
        }
        public void RotateSelected(float value = 1)
        {
            if (_selected)
            {
                Transform targetTransform = _nowEditing3D ? _selected.parent : _selected;
                targetTransform.rotation = Quaternion.Euler(hitNormal * value * 15) * targetTransform.rotation;
            }
        }

        IEnumerator HitNomalValueSet()
        {
            LayerSet("NotPlaced");

            yield return new WaitForEndOfFrame();

            RaySet(true);
        }

        void LayerSet(string layerValue)
        {
            if (_selected == null)
            {
                return;
            }

            _selected.gameObject.layer = LayerMask.NameToLayer(layerValue);

            if (_nowEditing3D)
            {
                _selected.parent.gameObject.layer = LayerMask.NameToLayer(layerValue);
            }
        }

        /*
         * @brief Resize 이벤트에 대한 콜백
         */
        public void OnResizeArtwork()
        {
            float wheelInputValue = Input.GetAxis("Mouse ScrollWheel");

            if (_moving)
            {
                SetResize(wheelInputValue);
            }
        }

        public void SetResize(float value)
        {
            if (_selected && Mathf.Abs(value) > 1e-10)
            {
                Transform targetTransform = _nowEditing3D ? _selected.parent : _selected;
                var scale = targetTransform.localScale;
                var ratioX = scale.x / scale.z;
                var ratioY = scale.y / scale.z;
                _nowScale = Mathf.Clamp(_nowScale + value * scaleFactor, 0.1f, 1);

                scale.z = Mathf.Lerp(_defaultScaleZ / scaleLimit, _defaultScaleZ * scaleLimit, _nowScale);

                scale.x = scale.z * ratioX;
                if (_nowEditing3D)
                {
                    scale.y = scale.z * ratioY;
                }

                if (scaleTween != null && scaleTween.IsPlaying())
                {
                    scaleTween.Kill();
                }

                scaleTween = targetTransform.DOScale(scale, 0.5f);
            }
        }

        private void Update()
        {
            InputCheck();

            if (verifyModalManager == null) 
            {
                Debug.LogWarning("verifyModalManager null");
            }
            
            if (verifyModalManager.showingModal) return;
            
            if (_selected && _moving)
            {
                RaySet();
            }
        }

        void InputCheck()
        {
            OnSelect();
            OnResizeArtwork();
            OnRotateArtwork();
        }

        void RaySet(bool hitNormalSetOnly = false)
        {
            var ray = cam.ScreenPointToRay(Input.mousePosition);
            var mask = 1 << LayerMask.NameToLayer("Placeable");
            if (Physics.Raycast(ray, out var hit, 100.0f, mask))
            {
                var objectHit = hit.transform;

                if (objectHit.CompareTag("Paint") || objectHit.CompareTag("Wall") || objectHit.CompareTag("Floor"))
                {
                    if (_nowEditing3D)
                    {
                        if (objectHit != _selected)
                        {
                            AlignPaintTransform(_selected.parent, hit, objectHit.CompareTag("Wall"), hitNormalSetOnly);
                        }
                    }
                    else
                    {
                        AlignPaintTransform(_selected, hit, objectHit.CompareTag("Wall"), hitNormalSetOnly);
                    }
                }
            }
        }
        
        /*
         * @brief Ray의 hit정보에 따라 Artwork의 transform을 설정해주는 함수
         */
        private void AlignPaintTransform(Transform paint, RaycastHit hit, bool snap , bool hitNormalSetOnly = false)
        {
            var placePosition = hit.point;
            var objectHit = hit.transform;
            var distanceFromCenterY = hit.point.y - objectHit.transform.position.y;
            placePosition.y = objectHit.transform.position.y 
                              + (snap ? SnapFloat(distanceFromCenterY, snapGridSizeY) : distanceFromCenterY);

            hitNormal = hit.normal;
            hitNormalReady = true;


            if (!hitNormalSetOnly)
            {
                paint.rotation = Quaternion.FromToRotation(paint.up, hit.normal) * paint.rotation;
                paint.position = placePosition;
            }
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
            if (_nowEditing3D)
            {
                _defaultScaleZ = _selected.transform.parent.localScale.z;
            }
            else
            {
                _defaultScaleZ = _selected.transform.localScale.z;
            }

            _nowScale = (_defaultScaleZ - _defaultScaleZ / scaleLimit)
                        / (_defaultScaleZ * scaleLimit - _defaultScaleZ / scaleLimit);
        }
    }
}
