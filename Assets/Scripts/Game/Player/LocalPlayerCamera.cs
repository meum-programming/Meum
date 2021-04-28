﻿using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;
using DG.Tweening;

namespace Game.Player
{
    /*
     * @brief 플레이어의 카메라를 제어하는 컴포넌트(화면기준 Y방향 드래그시 회전), 1인칭과 3인칭의 상황을 모두 제어
     */
    [RequireComponent(typeof(Camera))]
    public class LocalPlayerCamera : MonoBehaviour
    {
        [SerializeField] private float sensitivity;
        [SerializeField] private float cameraRotationLimit;
        [SerializeField] private Transform cameraPivot;
        
        [Header("Switching")] 
        [SerializeField] private Transform firstPersonCamTransform;
        [SerializeField] private Transform thirdPersonCamTransform;
        [SerializeField] private float switchingDuration;

        [SerializeField] LocalPlayerRotation localPlayerRotation;
        [SerializeField] Transform lookAtTargetObj;
        [SerializeField] Transform moveCheckObj;

        public bool IsFirstPersonView { get; private set; }
        public bool IsSwitchingView
        {
            get { return !ReferenceEquals(_switching, null); }
        }

        private Vector3 _defaultEulerAngle;
        private Vector3 _cameraRotationDelta;
        private Camera _camera;
        private Transform _transform;
        private IEnumerator _switching = null;
        private bool _isRotateEnabled;
        public float _thirdPersonCamDistance;

        [SerializeField] CameraColliderChecker cameraColliderChecker;

        private void Awake()
        {
            Assert.IsNotNull(cameraPivot);
            Assert.IsNotNull(firstPersonCamTransform);
            Assert.IsNotNull(thirdPersonCamTransform);
            
            _camera = GetComponent<Camera>();
            _transform = transform;

            //IsFirstPersonView = false;
            //_transform.localPosition = thirdPersonCamTransform.localPosition;
            //_transform.localRotation = thirdPersonCamTransform.localRotation;

            //1인칭 시점으로 세팅
            IsFirstPersonView = true;
            _transform.localPosition = firstPersonCamTransform.localPosition;
            _transform.localRotation = firstPersonCamTransform.localRotation;
            //StartCoroutine(SwitchView());

            _defaultEulerAngle = _transform.localEulerAngles;
            _camera.cullingMask = ~0;

            _thirdPersonCamDistance = (cameraPivot.position - thirdPersonCamTransform.position).magnitude;
        }

        

        private void Update()
        {
            IsColliderOnCheck();
            
        }

        void IsColliderOnCheck()
        {
            if (cameraColliderChecker.isColliderOn == false)
                return;
          
            float checkDis = Vector3.Distance(cameraPivot.position, _camera.transform.position);
            MoveCheckObjPosChange(Vector3.forward * checkDis * 0.05f);
        }


        public void OnRotate(InputAction.CallbackContext ctx)
        {
            if (IsSwitchingView) return;    // 인칭 전환중이라면 아무것도 안함
            if (!_isRotateEnabled) 
            {
                cameraRotFlag = false;
                carmeraRotValue = Vector2.zero;
                return;
            }

            var value = ctx.ReadValue<Vector2>();
            float sensitivity = DataManager.Instance.GetMouseSensitivityValue();

            var test = value.x * sensitivity * 0.1f;

            Vector2 setValue = new Vector2(-value.y, value.x);
            setValue *= sensitivity * 0.1f;

            Vector2 rotValue = cameraPivot.rotation.eulerAngles;
            rotValue += setValue;
            cameraPivot.rotation = Quaternion.Euler(rotValue);
        }


        bool cameraRotFlag = false;
        Vector2 carmeraRotValue = Vector2.zero;

        public void OnRotateKeybordInput(InputAction.CallbackContext ctx)
        {
            if (UI.ChattingUI.ChattingUI.Get() != null && UI.ChattingUI.ChattingUI.Get().InputFieldActivated())
            {
                return;
            }

            CameraRotFlagChange(ctx, 2f);
        }

        void CameraRotFlagChange(InputAction.CallbackContext ctx, float addValue = 10)
        {
            if (ctx.action.phase == InputActionPhase.Started)
            {
                cameraRotFlag = true;
                carmeraRotValue = ctx.ReadValue<Vector2>() * addValue;
            }
            else if (ctx.action.phase == InputActionPhase.Performed)
            {
                cameraRotFlag = true;
                carmeraRotValue = ctx.ReadValue<Vector2>() * addValue;
            }
            else if (ctx.action.phase == InputActionPhase.Canceled)
            {
                cameraRotFlag = false;
                carmeraRotValue = Vector2.zero;
            }
        }


        void InputCheck()
        {
            if (cameraRotFlag)
            {
                _cameraRotationDelta.x -= carmeraRotValue.y * sensitivity;
                _cameraRotationDelta.x = Mathf.Clamp(
                    _cameraRotationDelta.x,
                    -cameraRotationLimit,
                    cameraRotationLimit);

                //if (!IsFirstPersonView)
                {
                    //cameraPivot.Rotate(Vector3.up, carmeraRotValue.x * sensitivity);
                }

                var newCameraRotation = _defaultEulerAngle;
                newCameraRotation += _cameraRotationDelta;

                //_transform.localEulerAngles = newCameraRotation;
                cameraPivot.localEulerAngles = newCameraRotation;

                //_transform.parent.localEulerAngles = newCameraRotation;

                if (IsFirstPersonView)
                {
                    //localPlayerRotation.OnRotateKeybordInput(carmeraRotValue);
                }
            }
        }


        public void OnSwitchView(InputAction.CallbackContext ctx)
        {
            if (UI.ChattingUI.ChattingUI.Get().InputFieldActivated())
                return;

            if (ctx.performed)
                OnSwitchView();
        }

        public void OnSwitchView()
        {
            StartCoroutine(_switching = SwitchView());
        }


        /*
         * @brief 1인칭과 3인칭을 전환하는 코루틴
         * @details 카메라의 local transform을 바꾸고, 1인칭일시에 플레이어를 안보이게 함
         */
        private IEnumerator SwitchView()
        {
            var posSrc = _transform.localPosition;
            var posDst = IsFirstPersonView
                ? thirdPersonCamTransform.localPosition
                : firstPersonCamTransform.localPosition;

            var rotSrc = _transform.localRotation;
            var rotDst = IsFirstPersonView
                ? thirdPersonCamTransform.localRotation
                : firstPersonCamTransform.localRotation;

            var pivotRotSrc = cameraPivot.localRotation;
            var pivotRotDst = Quaternion.identity;
            
            var t = 0.0f;
            while (t < 1.0f)
            {
                _transform.localPosition = Vector3.Lerp(posSrc, posDst, t);
                _transform.localRotation = Quaternion.Lerp(rotSrc, rotDst, t);
                cameraPivot.localRotation = Quaternion.Lerp(pivotRotSrc, pivotRotDst, t);
                
                t += Time.deltaTime / switchingDuration;
                yield return null;
            }

            _transform.localPosition = posDst;
            _transform.localRotation = rotDst;
            cameraPivot.localRotation = pivotRotDst;

            _defaultEulerAngle = _transform.localEulerAngles;
            _cameraRotationDelta = Vector3.zero;

            IsFirstPersonView = !IsFirstPersonView;
            if (IsFirstPersonView)
                _camera.cullingMask = ~LayerMask.GetMask("LocalPlayer");
            else
                _camera.cullingMask = ~0;
            
            _switching = null;
        }
        
        public void OnRotateEnable(InputAction.CallbackContext ctx)
        {
            var value = ctx.ReadValue<float>();
            _isRotateEnabled = value > 0.5f; // value is 1 or 0 (float)
        }

        public void OnCameraPosReset(InputAction.CallbackContext ctx)
        {
            if (UI.ChattingUI.ChattingUI.Get().InputFieldActivated())
                return;

            if (!ctx.performed) return;

            var value = ctx.ReadValue<float>() / 120;    // scroll raw value: [-120, 120]

            if (Mathf.Abs(value) > 1e-10)
            {
                MoveCheckObjPosChange(Vector3.forward * value);
            }
        }

        void MoveCheckObjPosChange(Vector3 value)
        {
            moveCheckObj.transform.Translate(value);

            if (moveCheckObj.transform.localPosition.z > 0.15f)
            {
                moveCheckObj.transform.localPosition = new Vector3(0, 1.65f, 0.15f);
            }

            CameraReset();
        }

        Tween moveTween = null;
        Tween rotTween = null;

        void CameraReset()
        {
            if (moveTween != null && moveTween.IsPlaying())
            {
                moveTween.Kill();
            }
            moveTween = _camera.transform.DOMove(moveCheckObj.position, 0.5f);

            if (rotTween != null && rotTween.IsPlaying())
            {
                rotTween.Kill();
            }
            rotTween = _camera.transform.DOLocalRotate(moveCheckObj.transform.localRotation.eulerAngles, 0.5f);

        }

    }
}

