using System;
using System.Collections;
using System.Collections.Generic;

using UI.BuilderScene;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Builder.Camera
{
    public class CameraMove : MonoBehaviour
    {
        #region SerializeFields
        
        [SerializeField] private float speed;
        [SerializeField] private Transform camera;
        [SerializeField] private BuilderSceneVerifyModals verifyModalManager;
        
        #endregion

        #region PrivateFields

        private Vector3 _moveVector;

        #endregion

        private void Start()
        {
            ResetPosition();
        }

        public void ResetPosition()
        {
            // TODO: 좀 더 좋은 방식으로 바꾸기
            var spawnSite = GameObject.Find("SpawnSite").transform;
            transform.position = spawnSite.position;
        }

        private void Update()
        {
            // TODO: 좀 더 좋은 방식으로 바꾸기
            if (verifyModalManager.showingModal) return;
            
            var direction = camera.right * _moveVector.x +
                            camera.forward * _moveVector.y;
            direction.Normalize();

            var delta = direction * (speed * Time.deltaTime);
            transform.Translate(delta, Space.World);
        }


        public void OnMove(InputAction.CallbackContext ctx)
        {
            var value = ctx.ReadValue<Vector2>();
            _moveVector = value;
        }
    }
}
