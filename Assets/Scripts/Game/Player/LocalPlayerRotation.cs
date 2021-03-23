using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;

namespace Game.Player
{
    /*
     * @brief 플레이어의 회전을 제어하는 컴포넌트(화면기준 X방향 드래그시 회전)
     */
    public class LocalPlayerRotation : MonoBehaviour
    {
        [SerializeField] private Transform cameraPivot;
        //[SerializeField] private float sensitivity;
        [SerializeField] private LocalPlayerCamera cameraController;

        private Transform _transform;
        private bool _isMoving = false;
        private bool _isRotateEnabled = false;

        private void Awake()
        {
            Assert.IsNotNull(cameraPivot);
            Assert.IsNotNull(cameraController);
            
            _transform = transform;
        }

        void OnEnable()
        {
            if (FindObjectOfType<Joystick>())
            {
                FindObjectOfType<Joystick>().moveEventOn += OnMoveJoystick;
            }
        }
        void OnDisable()
        {
            if (FindObjectOfType<Joystick>())
            {
                FindObjectOfType<Joystick>().moveEventOn -= OnMoveJoystick;
            }
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
             * cameraPivot은 플레이어의 child로 들어가서 플레이어가 회전하면 같이 돌아감 
             * 카메라 방향은 돌아가면 안되기때문에 회전전에 저장하고 덮어씌움
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

        public void OnMoveJoystick(Vector2 value)
        {
            if (UI.ChattingUI.ChattingUI.Get().InputFieldActivated())
                return;

            if (value.sqrMagnitude < 1e-3) _isMoving = false;
            else _isMoving = true;
        }


        public void OnRotate(InputAction.CallbackContext ctx)
        {
            if (cameraController.IsSwitchingView || !cameraController.IsFirstPersonView)
                return;

            if (!_isRotateEnabled) return;
            
            var value = ctx.ReadValue<Vector2>();

            float sensitivity = DataManager.Instance.GetMouseSensitivityValue();

            transform.Rotate(Vector3.up, value.x * sensitivity * 0.1f);
        }

        public void OnRotateKeybordInput(Vector2 value)
        {
            //회전 감도 임시적용
            float sensitivity = 0.1f;

            transform.Rotate(Vector3.up, value.x * sensitivity);
        }

        public void OnRotateEnable(InputAction.CallbackContext ctx)
        {
            var value = ctx.ReadValue<float>();
            _isRotateEnabled = value > 0.5f;    // value is 1 or 0 (float)
        }
    }
}