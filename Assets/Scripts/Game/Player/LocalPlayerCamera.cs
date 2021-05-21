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
        [SerializeField] private Transform cameraPivot;
        [SerializeField] Transform moveCheckObj;
        [SerializeField] Camera _camera;
        [SerializeField] PlayerChaChange playerChaChange;
        [SerializeField] LocalPlayerMove localPlayerMove;

        private bool _isRotateEnabled;

        private Tween moveTween = null;

        bool cameraRotFlag = false;
        Vector2 carmeraRotValue = Vector2.zero;

        bool cameraZoomFlag = false;
        float carmeraZoomValue = 0;

        private void Start()
        {
            playerChaChange.SkinnedMeshRendererActiveSet(false);
        }

        private void FixedUpdate()
        {
            IsColliderOnCheck();
        }

        /// <summary>
        /// 콜라이더 충돌 여부 체크
        /// </summary>
        void IsColliderOnCheck()
        {
            float hitDistance = RayCastCheck(transform.position);

            if (hitDistance != 0)
            {
                float posZ = -(hitDistance - 1);
                ResetCameraZoom(posZ, time);
            }
        }

        public float time = 0.1f;


        /// <summary>
        /// 넘겨 받은 위치값에 콜라이더가 있는지 체크
        /// </summary>
        /// <param name="checkPos"></param>
        /// <returns></returns>
        float RayCastCheck(Vector3 checkPos)
        {
            RaycastHit hit;

            float distance = Vector3.Distance(cameraPivot.position, checkPos) + 1;

            var rayDir = (checkPos - cameraPivot.position).normalized;
            var layerMask = 1 << LayerMask.NameToLayer("LocalPlayer") | 1 << LayerMask.NameToLayer("RemotePlayer");
            layerMask = ~layerMask;
            Physics.Raycast(cameraPivot.position, rayDir, out hit, distance, layerMask);

            return hit.distance;
        }

        /// <summary>
        /// 마우스 오른쪽 버튼을 누르면 호출
        /// </summary>
        /// <param name="ctx"></param>
        public void OnRotateEnable(InputAction.CallbackContext ctx)
        {
            var value = ctx.ReadValue<float>();
            _isRotateEnabled = value == 1; // value is 1 or 0 (float)
        }

        /// <summary>
        /// 마우스 오른쪽 버튼을 드레그 하면 호출
        /// </summary>
        /// <param name="ctx"></param>
        public void OnRotate(InputAction.CallbackContext ctx)
        {
            if (!_isRotateEnabled) 
            {
                return;
            }

            var value = ctx.ReadValue<Vector2>();
            RotateOn(value);
        }

        void RotateOn(Vector2 value)
        {
            float sensitivity = DataManager.Instance.GetMouseSensitivityValue();

            var test = value.x * sensitivity * 0.1f;

            Vector2 setValue = new Vector2(-value.y, value.x);
            setValue *= sensitivity * 0.1f;

            Vector2 rotValue = cameraPivot.rotation.eulerAngles;
            rotValue += setValue;
            cameraPivot.rotation = Quaternion.Euler(rotValue);

            cameraPivot.transform.localPosition = Vector3.zero;

            if (firstViewOn)
            {
                localPlayerMove.ChaLookAtForward();
            }
        }


        /// <summary>
        /// 키보드 IJKL키를 누르면 호출
        /// </summary>
        /// <param name="ctx"></param>
        public void OnRotateKeybordInput(InputAction.CallbackContext ctx)
        {
            if (UI.ChattingUI.ChattingUI.Get() != null && UI.ChattingUI.ChattingUI.Get().InputFieldActivated())
            {
                return;
            }

            float addValue = 4;

            switch (ctx.action.phase)
            {
                case InputActionPhase.Started:
                    cameraRotFlag = true;
                    carmeraRotValue = ctx.ReadValue<Vector2>() * addValue;
                    break;
                case InputActionPhase.Performed:
                    cameraRotFlag = true;
                    carmeraRotValue = ctx.ReadValue<Vector2>() * addValue;
                    break;
                case InputActionPhase.Canceled:
                    cameraRotFlag = false;
                    carmeraRotValue = Vector2.zero;
                    break;
            }
        }

        /// <summary>
        /// update에서 cameraRotFlag값이 true일때 계속 호출
        /// </summary>
        void CameraRotFlagCheck()
        {
            if (cameraRotFlag == false)
                return;

            RotateOn(carmeraRotValue);
        }

        /// <summary>
        /// 키보드 IJKL키를 누르면 호출
        /// </summary>
        /// <param name="ctx"></param>
        public void OnCameraZoomResetKeybordInput(InputAction.CallbackContext ctx)
        {
            if (UI.ChattingUI.ChattingUI.Get().InputFieldActivated())
                return;

            float addValue = 120;

            switch (ctx.action.phase)
            {
                case InputActionPhase.Started:
                    cameraZoomFlag = true;
                    carmeraZoomValue = ctx.ReadValue<float>(); 
                    break;
                case InputActionPhase.Performed:
                    cameraZoomFlag = true;
                    carmeraZoomValue = ctx.ReadValue<float>();
                    break;
                case InputActionPhase.Canceled:
                    cameraZoomFlag = false;
                    carmeraZoomValue = 0;
                    break;
            }
        }

        void CameraZoomFlagCheck()
        {
            if (cameraZoomFlag == false)
                return;

            if (CanZoomCheck(carmeraZoomValue))
            {
                float posZ = _camera.transform.localPosition.z;
                ResetCameraZoom(posZ + carmeraZoomValue);
            }   
        }

        /// <summary>
        /// 마우스 휠을 조정할때 호출
        /// </summary>
        /// <param name="ctx"></param>
        public void OnCameraZoomReset(InputAction.CallbackContext ctx)
        {
            if (UI.ChattingUI.ChattingUI.Get().InputFieldActivated())
                return;

            if (!ctx.performed) return;

            var value = ctx.ReadValue<float>();    // scroll raw value: [-120, 120]
            
#if UNITY_EDITOR
            value *= 0.01f;
#endif

            if (Mathf.Abs(value) > 0)
            {
                //if (CanZoomCheck(value))
                {
                    float posZ = _camera.transform.localPosition.z;
                    ResetCameraZoom(posZ + value);
                    //ResetCameraZoom(posZ);
                }
            }
        }

        /// <summary>
        /// 줌이 가능할지 체크
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        bool CanZoomCheck(float value)
        {
            moveCheckObj.transform.position = _camera.transform.position;

            Vector3 pos = moveCheckObj.transform.localPosition;
            pos.z += value;
            moveCheckObj.transform.localPosition = pos;

            Vector3 checkPos = moveCheckObj.transform.position;

            return RayCastCheck(checkPos) == 0;
        }

        /// <summary>
        /// 카메라 줌 인(아웃)
        /// </summary>
        /// <param name="posZ"></param>
        /// <param name="tweenTime"></param>
        void ResetCameraZoom(float posZ, float tweenTime = 0.5f)
        {
            if (moveTween != null && moveTween.IsPlaying())
            {
                moveTween.Kill();
            }

            if (posZ > firstViewMinValue)
            {

                if (firstViewReadyOn) 
                {
                    if (firstViewOn)
                    {
                        _camera.transform.localPosition = new Vector3(0, 0, 0f);
                        playerChaChange.SkinnedMeshRendererActiveSet(false);
                    }
                    else
                    {
                        moveTween = _camera.transform.DOLocalMoveZ(firstViewMinValue, tweenTime);
                    }
                }
                else
                {
                    firstViewReadyOn = true;
                    firstViewOnDelay = 0.5f;
                    moveTween = _camera.transform.DOLocalMoveZ(firstViewMinValue, tweenTime);
                }
                
            }
            
            else
            {
                playerChaChange.SkinnedMeshRendererActiveSet(true);
                firstViewOn = false;
                firstViewReadyOn = false;
                moveTween = _camera.transform.DOLocalMoveZ(posZ, tweenTime);
            }
        }

        private void Update()
        {
            if (firstViewReadyOn)
            {
                firstViewOnDelay -= Time.deltaTime;

                if (firstViewOnDelay < 0 )
                {
                    firstViewOnDelay = 0;
                    firstViewOn = true;

                    localPlayerMove.ChaLookAtForward();
                }

            }

            CameraZoomFlagCheck();
            CameraRotFlagCheck();
        }

        public float firstViewMinValue = -0.7f;
        public float firstViewOnDelay = 1;
        public bool firstViewOn = false;
        public bool firstViewReadyOn = false;
    }
}

