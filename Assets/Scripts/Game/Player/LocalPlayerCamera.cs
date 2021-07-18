using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;
//using UnityEngine.InputSystem;
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
                ResetCameraZoom(posZ, tweenTime: time);
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

        private void OnRotateInput()
        {
            //마우스 오른쪽 버튼이 눌린게 아니라면
            if (!Input.GetMouseButton(1))
                return;
            
            Vector2 delta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
            RotateOn(delta * 10);
        }

        void RotateOn(Vector2 value)
        {
            float sensitivity = DataManager.Instance.GetMouseSensitivityValue();

            var test = value.x * sensitivity * 0.1f;

            Vector2 setValue = new Vector2(-value.y, value.x);
            setValue *= sensitivity * 0.1f;

            Vector2 rotValue = cameraPivot.localEulerAngles;

            float checkXValue = setValue.x + rotValue.x;

            //x값이 90 이상이면 180 이하 값이 되면, 270도 근처에서 화면이 반전된다.
            if (checkXValue <= 180 && checkXValue >= 90 || checkXValue <= 275 && checkXValue >= 265)
                return;
            
            rotValue += setValue;
            cameraPivot.localEulerAngles = rotValue;

            cameraPivot.transform.localPosition = Vector3.zero;

            if (firstViewOn)
            {
                localPlayerMove.ChaLookAtForward();
            }
        }

        /// <summary>
        /// 키보드 IJKL키를 누르면 호출
        /// </summary>
        void OnRotateKeybordInput()
        {
            if (UI.ChattingUI.ChattingUI.Get() != null && UI.ChattingUI.ChattingUI.Get().InputFieldActivated())
            {
                return;
            }

            carmeraRotValue = Vector2.zero;

            if (Input.GetKeyUp(KeyCode.I) || Input.GetKeyUp(KeyCode.J) || Input.GetKeyUp(KeyCode.K) || Input.GetKeyUp(KeyCode.L))
            {
                cameraRotFlag = false;
                carmeraRotValue = Vector2.zero;
            }
            else
            {
                if (Input.GetKey(KeyCode.I))
                {
                    cameraRotFlag = true;
                    carmeraRotValue.y += 10;
                }
                if (Input.GetKey(KeyCode.K))
                {
                    cameraRotFlag = true;
                    carmeraRotValue.y -= 10;
                }
                if (Input.GetKey(KeyCode.J))
                {
                    cameraRotFlag = true;
                    carmeraRotValue.x -= 10;
                }
                
                if (Input.GetKey(KeyCode.L))
                {
                    cameraRotFlag = true;
                    carmeraRotValue.x += 10;
                }
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
        /// 키보드 O,P키를 누르면 호출(카메라 줌)
        /// </summary>
        void OnCameraZoomKeybordInputCheck()
        {
            if (UI.ChattingUI.ChattingUI.Get().InputFieldActivated())
                return;

            if (Input.GetKeyUp(KeyCode.O) || Input.GetKeyUp(KeyCode.P))
            {
                cameraZoomFlag = false;
                carmeraZoomValue = 0;
            }
            else if (Input.GetKeyDown(KeyCode.O)) 
            {
                cameraZoomFlag = true;
                carmeraZoomValue = 10;
            }
            else if (Input.GetKeyDown(KeyCode.P))
            {
                cameraZoomFlag = true;
                carmeraZoomValue = -10;
            }
        }

        void CameraZoomFlagCheck()
        {
            if (cameraZoomFlag == false)
                return;

            if (CanZoomCheck(carmeraZoomValue))
            {
                float posZ = _camera.transform.localPosition.z;
                ResetCameraZoom(posZ + carmeraZoomValue , carmeraZoomValue > 0);
            }
        }

        void OnCameraZoomCheck()
        {
            if (UI.ChattingUI.ChattingUI.Get().InputFieldActivated())
                return;

            float wheelInputValue = Input.GetAxis("Mouse ScrollWheel");

            wheelInputValue *= 10;

            if (wheelInputValue != 0)
            {
                if (CanZoomCheck(wheelInputValue))
                {
                    float posZ = _camera.transform.localPosition.z;
                    ResetCameraZoom(posZ + wheelInputValue, wheelInputValue > 0);
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
        void ResetCameraZoom(float posZ,bool zoomIn = false , float tweenTime = 0.5f)
        {
            //20미터 보다 더 멀어지면 
            if (posZ < - 20)
            {
                return;
            }

            if (moveTween != null && moveTween.IsPlaying())
            {
                moveTween.Kill();
            }

            if (zoomIn && posZ > firstViewMinValue)
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
            InputEventCheck();
            
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

        void InputEventCheck()
        {
            OnCameraZoomCheck();
            OnCameraZoomKeybordInputCheck();
            OnRotateInput();
            OnRotateKeybordInput();
        }

        public float firstViewMinValue = -0.7f;
        public float firstViewOnDelay = 1;
        public bool firstViewOn = false;
        public bool firstViewReadyOn = false;
    }
}

