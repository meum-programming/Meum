using System;
using System.Collections;
using System.Collections.Generic;

using UI.BuilderScene;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Builder.Camera
{
    /*
     * @brief Build scene 에서 카메라의 이동(회전X)을 담당하는 컴포넌트
     */
    public class CameraMove : MonoBehaviour
    {
        [SerializeField] private float speed;
        [SerializeField] private float runSpeed;
        [SerializeField] private Transform camera;
        [SerializeField] private BuilderSceneVerifyModals verifyModalManager;

        private Vector3 _moveVector;
        private bool _running;

        private void Start()
        {
            ResetPosition();
        }

        public void ResetPosition()
        {
            var spawnSite = GameObject.Find("SpawnSite").transform;
            transform.position = spawnSite.position;
        }
        
        /*
         * @brief Update 함수에서 실제 이동을 함, OnMove는 moveVector 갱신만
         */
        private void Update()
        {
            if (verifyModalManager.showingModal) return;
            
            var direction = camera.right * _moveVector.x +
                            camera.forward * _moveVector.y;
            direction.Normalize();

            var delta = direction * ((_running ? runSpeed : speed) * Time.deltaTime);
            transform.Translate(delta, Space.World);
        }

        public void OnMove(InputAction.CallbackContext ctx)
        {
            var value = ctx.ReadValue<Vector2>();
            _moveVector = value;
        }
        
        public void OnRun(InputAction.CallbackContext ctx)
        {
            var value = ctx.ReadValue<float>();
            _running = value > 0.5f;    // value is 1 or 0 (float)
        }
    }
}
