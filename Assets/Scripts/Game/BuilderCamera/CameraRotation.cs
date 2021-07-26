using Core.Socket;
using UI.BuilderScene;
using UnityEngine;
//using UnityEngine.InputSystem;

namespace Game.Builder.Camera
{
    /*
     * @brief Build scene에서 카메라 회전(이동X)을 담당하는 컴포넌트
     */
    public class CameraRotation : MonoBehaviour
    {
        [SerializeField] private float sensitivity;
        [SerializeField] private Transform camera;
        [SerializeField] private BuilderSceneVerifyModals verifyModalManager;

        private bool _isRotateEnabled = false;

        private void Start()
        {
            ResetRotation();
        }

        public void ResetRotation()
        {
            //var spawnSite = GameObject.Find("SpawnSite").transform;
            //transform.rotation = spawnSite.rotation;
            transform.rotation = DataSynchronizer.Get().GetSpawnRot();
        }

        private void Update()
        {
            OnRotate();
        }

        public void OnRotate()
        {
            //if (!_isRotateEnabled) return;
            if (verifyModalManager.showingModal) return;

            //마우스 오른쪽 버튼이 눌린게 아니라면
            if (!Input.GetMouseButton(1))
                return;

            Vector2 delta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

            var value = delta * 10;

            transform.Rotate(Vector3.up, value.x * sensitivity);
            camera.Rotate(Vector3.right, -value.y * sensitivity);
        }
    }
}