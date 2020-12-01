using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;

namespace Game.Player
{
    public class LocalPlayerRotation : MonoBehaviour
    {
        #region SerializeFields

        [SerializeField] private Transform cameraPivot;
        [SerializeField] private float sensitivity;
        [SerializeField] private LocalPlayerCamera cameraController;

        #endregion

        #region PrivateFields
        
        private Transform _transform;
        private bool _isMoving = false;
        private bool _isRotateEnabled = false;
        
        #endregion

        private void Awake()
        {
            Assert.IsNotNull(cameraPivot);
            Assert.IsNotNull(cameraController);
            
            _transform = transform;
        }

        private void Update()
        {
            if(_isMoving)
                FitCharacterRotationWithCamera();
        }
        /*
         * @breif 카메라 방향과 캐릭터 방향을 맞춤
         */
        private void FitCharacterRotationWithCamera()
        {
            if (cameraController.IsSwitchingView || cameraController.IsFirstPersonView)
                return;

            var camPivotRotationBackup = cameraPivot.rotation;

            var targetRotation = Quaternion.LookRotation(cameraPivot.forward, Vector3.up);
            _transform.rotation = Damp(_transform.rotation, targetRotation, 5.0f, Time.deltaTime);

            /*
             * cam pivot is child of Player GameObject
             * to prevent camera direction from rotating
             * backup rotation value and put after rotating character
             */
            cameraPivot.rotation = camPivotRotationBackup;
        }

        // http://www.rorydriscoll.com/2016/03/07/frame-rate-independent-damping-using-lerp/
        private Quaternion Damp(Quaternion a, Quaternion b, float lambda, float dt)
        {
            return Quaternion.Slerp(a, b, 1 - Mathf.Exp(-lambda * dt));
        }
        
        public void OnMove(InputAction.CallbackContext ctx)
        {
            if (UI.ChattingUI.ChattingUI.Get().InputFieldActivated())
                return;
            
            var value = ctx.ReadValue<Vector2>();
            if (value.sqrMagnitude < 1e-3) _isMoving = false;
            else _isMoving = true;
        }

        /*
         * @breif Rotate(캐릭터 회전) 콜백
         */
        public void OnRotate(InputAction.CallbackContext ctx)
        {
            if (cameraController.IsSwitchingView || !cameraController.IsFirstPersonView)
                return;

            if (!_isRotateEnabled) return;
            
            var value = ctx.ReadValue<Vector2>();

            transform.Rotate(Vector3.up, value.x * sensitivity);
        }

        public void OnRotateEnable(InputAction.CallbackContext ctx)
        {
            var value = ctx.ReadValue<float>();
            _isRotateEnabled = value > 0.5f;    // value is 1 or 0 (float)
        }
    }
}