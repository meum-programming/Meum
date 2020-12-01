using UI.BuilderScene;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace Game.Artwork
{
    /*
     * @brief Artwork를 설치하기 위한 컴포넌트
     * @details Artwork의 선택, 설치, 이동, 제거의 기능을 함
     */
    public class ArtworkPlacer : MonoBehaviour
    {
        #region SerializedFields
        
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
        
        #endregion
        
        #region PrivateFields
        
        private float _nowScale = 0.5f;
        private float _defaultScaleZ = 0.0f;

        private Transform _selected = null;
        private bool _moving = false;
        private bool _nowEditing3D = false;
        
        #endregion
        
        /*
         * @brief 선택된 UI.Content에 해당하는 Artwork 오브젝트를 생성
         */
        public void CreateSelected(UI.Content data)
        {
            Vector3 pos = Mouse.current.position.ReadValue();
            pos.z = -10.0f;
            pos = cam.ScreenToWorldPoint(pos);

            _nowEditing3D = data.data.type_artwork == 1;
            _selected = Instantiate(_nowEditing3D ? object3DPrefab : paintPrefab, pos, Quaternion.identity, transform).transform;
            Assert.IsNotNull(_selected);
            var artworkInfo = _selected.GetComponent<ArtworkInfo>();
            Assert.IsNotNull(artworkInfo);
            artworkInfo.UpdateWithArtworkContent(data);

            StartMovingObj();
        }
        
        /*
         * @brief 선택된 UI.Content에 해당하는 2D Artwork 오브젝트를 배너로 생성
         */
        public void CreateSelectedBanner(UI.Content data, string url)
        {
            Assert.IsTrue(data.data.type_artwork == 0);
            
            Vector3 pos = Mouse.current.position.ReadValue();
            pos.z = -10.0f;
            pos = cam.ScreenToWorldPoint(pos);

            _selected = Instantiate(paintPrefab, pos, Quaternion.identity, transform).transform;
            Assert.IsNotNull(_selected);
            var paintInfo = _selected.GetComponent<ArtworkInfo>();
            Assert.IsNotNull(paintInfo);
            paintInfo.UpdateWithArtworkContent(data);
            paintInfo.BannerUrl = url;

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
            
            if(toggleButton.toggled)
                toggleButton.InvokeButton();
            toggleButton.SetButtonEnable(false);

            _selected.gameObject.layer = LayerMask.NameToLayer("NotPlaced");
        }
        
        /*
         * @brief 선택된 오브젝트를 그만 움직이도록 함
         */
        private void StopMovingObj()
        {
            _moving = false;
            if(!toggleButton.toggled)
                toggleButton.InvokeButton();
            toggleButton.SetButtonEnable(true);
            
            if(_selected)
                _selected.gameObject.layer = LayerMask.NameToLayer("Placeable");
        }

        public void DeleteSelected()
        {
            if (_selected)
            {
                StopMovingObj();
                Destroy(_selected.gameObject);
                _selected = null;
            }
        }
        
        /*
         * @brief Select(클릭) 이벤트에 대한 콜백
         */
        public void OnSelect(InputAction.CallbackContext ctx)
        {
            if (verifyModalManager.showingModal) return;
            if (EventSystem.current.IsPointerOverGameObject()) // 마우스가 UI Element 위에 있는경우 아무것도 하지 않음
                return;
            
            var ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());
            var mask = 1 << LayerMask.NameToLayer("Placeable");
            if (Physics.Raycast(ray, out var hit, 100.0f, mask))
            {
                var objectHit = hit.transform;
                
                if (!_moving && objectHit.CompareTag("Paint"))
                {
                    _selected = objectHit;
                    actionSelector.SetSelected(_selected);
                    _nowEditing3D = objectHit.GetComponent<ArtworkInfo>().ArtworkType == 1;
                    _moving = false;
                }
                else
                    Deselect();
            }
            else if(!_moving)
                Deselect();
        }
        
        /*
         * @brief Rotate 이벤트에 대한 콜백
         */
        public void OnRotateArtwork(InputAction.CallbackContext ctx)
        {
            if (_selected && _moving && ctx.performed)
            {
                var value = ctx.ReadValue<float>();
                _selected.rotation = Quaternion.Euler(_selected.up * (90.0f * value)) * _selected.rotation;
            }
        }
        
        /*
         * @brief Resize 이벤트에 대한 콜백
         */
        public void OnResizeArtwork(InputAction.CallbackContext ctx)
        {
            if (!ctx.performed) return;
            
            var value = ctx.ReadValue<float>() / 120.0f;    // scroll raw value: [-120, 120]
            if (_selected && _moving && Mathf.Abs(value) > 1e-10)
            {
                var scale = _selected.localScale;
                var ratioX = scale.x / scale.z;
                var ratioY = scale.y / scale.z;
                _nowScale = Mathf.Clamp(_nowScale + value * scaleFactor, 0, 1);
                scale.z = Mathf.Lerp(_defaultScaleZ / scaleLimit, _defaultScaleZ * scaleLimit, _nowScale);
                scale.x = scale.z * ratioX;
                if(_nowEditing3D)
                    scale.y = scale.z * ratioY;
                _selected.localScale = scale;
            }
        }

        private void Update()
        {
            if (verifyModalManager.showingModal) return;
            
            if (_selected && _moving)
            {
                var ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());
                var mask = 1 << LayerMask.NameToLayer("Placeable");
                if (Physics.Raycast(ray, out var hit, 100.0f, mask))
                {
                    var objectHit = hit.transform;
                    
                    if (objectHit.CompareTag("Paint") || objectHit.CompareTag("Wall") || objectHit.CompareTag("Floor"))
                        AlignPaintTransform(_selected, hit, objectHit.CompareTag("Wall"));
                }
            }
        }
        
        /*
         * @brief Ray의 hit정보에 따라 Artwork의 transform을 설정해주는 함수
         */
        private void AlignPaintTransform(Transform paint, RaycastHit hit, bool snap)
        {
            var placePosition = hit.point;
            var objectHit = hit.transform;
            var distanceFromCenterY = hit.point.y - objectHit.transform.position.y;
            placePosition.y = objectHit.transform.position.y 
                              + (snap ? SnapFloat(distanceFromCenterY, snapGridSizeY) : distanceFromCenterY);
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
