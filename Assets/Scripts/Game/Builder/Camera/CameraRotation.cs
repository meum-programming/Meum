using UI.BuilderScene;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Builder.Camera
{
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
            // TODO: 좀 더 좋은 방식으로 바꾸기
            var spawnSite = GameObject.Find("SpawnSite").transform;
            transform.rotation = spawnSite.rotation;
        }

        public void OnRotate(InputAction.CallbackContext ctx)
        {
            if (!_isRotateEnabled) return;

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