using UI.BuilderScene;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Builder.Camera
{
    /*
     * @brief Build scene에서 카메라 회전(이동X)을 담당하는 컴포넌트
     */
    public class CameraRotation : MonoBehaviour
    {
        #region SerializeFields
        
        [SerializeField] private float sensitivity;
        [SerializeField] private Transform camera;
        [SerializeField] private BuilderSceneVerifyModals verifyModalManager;
        
        #endregion

        #region PrivateFields

        private bool _isRotateEnabled = false;

        #endregion
        
        private void Start()
        {
            ResetRotation();
        }

        public void ResetRotation()
        {
            var spawnSite = GameObject.Find("SpawnSite").transform;
            transform.rotation = spawnSite.rotation;
        }

        public void OnRotate(InputAction.CallbackContext ctx)
        {
            if (!_isRotateEnabled) return;
            if (verifyModalManager.showingModal) return;

            var value = ctx.ReadValue<Vector2>();
            
            transform.Rotate(Vector3.up, value.x * sensitivity);
            camera.Rotate(Vector3.right, -value.y * sensitivity);
        }
        
        public void OnRotateEnable(InputAction.CallbackContext ctx)
        {
            var value = ctx.ReadValue<float>();
            _isRotateEnabled = value > 0.5f;    // value is 1 or 0 (float)
        }
    }
}