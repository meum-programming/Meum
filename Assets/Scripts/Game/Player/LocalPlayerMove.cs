using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;

namespace Game.Player
{
    /*
     *
     * @breif   LocalPlayer(클라이언트 주인 플레이어) 의 상하좌우 움직임, 점프를 제어
     * @details PlayerInput Component 를 위한 콜백들을 담고 있고 점프를 제어 <br>
     *          Animator의 isJumpEnded flag 업데이트
     * 
     */
    [RequireComponent(typeof(CharacterController), 
                     typeof(PlayerAnimationController))]
    public class LocalPlayerMove : MonoBehaviour
    {
        private const float GRAVITY = 9.8f;
        
        
        [SerializeField] private float walkSpeed;
        [SerializeField] private float runSpeed;
        [SerializeField] private float jumpHeight;

        [Header("Ground Checking")] 
        [SerializeField] private float groundDetectDistance = 0.1f;
        [SerializeField] private float jumpEndDetectDistance = 0.7f;
        [SerializeField] private LayerMask checkingMask;
        
        private Vector3 _colliderCenterPos;
        private float _distFromColliderCenterToGround;
        private CharacterController _charController;
        private PlayerAnimationController _animController;
        private Vector3 _moveVector = Vector3.zero;
        private float _velocityY;
        private bool _running = false;

        private Tween chaRotTween;

        bool isJumped = false;
        float jumpEndDeley = 0;

        [SerializeField] GroundChecker groundChecker;

        private void Awake()
        {
            _charController = GetComponent<CharacterController>();
            _animController = GetComponent<PlayerAnimationController>();

            _colliderCenterPos = _charController.center;
            _distFromColliderCenterToGround = _charController.bounds.extents.y;
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
            Move();
            ApplyGravity();

            _animController.SetIsJumpEnded(IsJumpEnded());
            //_animController.SetIsJumpEnded();
        }

        private void ApplyGravity()
        {
            if (IsGrounded())
                _velocityY = 0.0f;
            else
                _velocityY -= GRAVITY * Time.deltaTime;
        }

        private void Move()
        {
            if (UI.ChattingUI.ChattingUI.Get() != null && UI.ChattingUI.ChattingUI.Get().InputFieldActivated())
            {
                _moveVector = Vector3.zero;
               // return;
            }

            var selfTransform = transform;
            var direction = selfTransform.right * _moveVector.x + 
                            selfTransform.forward * _moveVector.y;
            direction.Normalize();

            var delta = Vector3.up * _velocityY + direction * (_running ? runSpeed : walkSpeed);
            _charController.Move(delta * Time.deltaTime);

            Transform chaTransform = _animController._animator.gameObject.transform;
            float value = 0;

            //이동 방향값을 인트값으로 변환
            float xValue = ((int)(_moveVector.x * 10)); //* 0.1f;
            float yValue = ((int)(_moveVector.y * 10)); //* 0.1f;

            //이동 방향에 따른 8방향 회전 세팅
            if (yValue == 10 || yValue == -10)
            {
                value = yValue == 10 ? 0 : 180;
            }
            else if (xValue == 10 || xValue == -10)
            {
                value = xValue == 10 ? 90 : -90;
            }
            else 
            {
                //왼쪽 + 앞
                if (xValue == -7 && yValue == 7)
                {
                    value = -45;
                }
                //오른쪽 + 앞
                else if (xValue == 7 && yValue == 7)
                {
                    value = 45;
                }
                //왼쪽 + 뒤
                else if (xValue == -7 && yValue == -7)
                {
                    value = 225;
                }
                //오른쪽 + 뒤
                else if (xValue == 7 && yValue == -7)
                {
                    value = 135;
                }
            }
            

            if (chaRotTween != null && chaRotTween.IsPlaying())
            {
                chaRotTween.Kill();
            }

            chaRotTween = chaTransform.transform.DOLocalRotate(new Vector3(0, value, 0), 0.3f);

            if (_moveVector != Vector3.zero && _animController._animator.GetCurrentAnimatorStateInfo(0).IsName("Human_Idle"))
            {
                _animController.SetVerticalSpeed(Mathf.Abs(_moveVector.x) + Mathf.Abs(_moveVector.y));
            }
        }

        private bool IsGrounded()
        {
            bool isValue = Physics.Raycast(
                transform.position + _colliderCenterPos,
                -Vector3.up,
                _distFromColliderCenterToGround + groundDetectDistance,
                checkingMask.value);

            isValue = isValue || _charController.isGrounded;

            return isValue;
        }

        private bool IsJumpEnded()
        {
            //땅에 닿았는지 체크
            bool isJumpEnd = Physics.Raycast(
                    transform.position + _colliderCenterPos,
                    -Vector3.up,
                    _distFromColliderCenterToGround + jumpEndDetectDistance,
                    checkingMask.value);

            return isJumpEnd || groundChecker.isGround;

            //return !;
        }

        public void OnMove(InputAction.CallbackContext ctx)
        {
            MoveVectorSet(ctx.ReadValue<Vector2>());
        }

        public void OnMoveJoystick(Vector2 value)
        {
            MoveVectorSet(value);
        }

        void MoveVectorSet(Vector2 value) 
        {
            if (UI.ChattingUI.ChattingUI.Get().InputFieldActivated())
            {
                if (IsGrounded())
                {
                    _animController.SetVerticalSpeed(0.0f);
                }
                return;
            }

            _moveVector = value;

            if (_moveVector == Vector3.zero)
            {
                _animController.SetVerticalSpeed(0);
            }
            else if (IsGrounded())
            {
                _animController.SetVerticalSpeed(Mathf.Abs(value.x) + Mathf.Abs(value.y));
            }
        }


        public void OnJump(InputAction.CallbackContext ctx)
        {
            if (UI.ChattingUI.ChattingUI.Get().InputFieldActivated())
                return;
            if (ctx.performed && IsGrounded())
            {
                _velocityY = Mathf.Sqrt(2.0f * GRAVITY * jumpHeight);
                _animController.SetJumpTrigger();

                isJumped = true;
                jumpEndDeley = 0.5f;
            }
        }

        public void OnRun(InputAction.CallbackContext ctx)
        {
            if (UI.ChattingUI.ChattingUI.Get().InputFieldActivated())
                return;
            var value = ctx.ReadValue<float>();
            _running = value > 0.5f;    // value is 1 or 0 (float)
            _animController.SetRunning(_running);
        }

        public void OnChat(InputAction.CallbackContext ctx)
        {
            if (ctx.action.phase == InputActionPhase.Started)
            {
                UI.ChattingUI.ChattingUI.Get().SetInputFieldActive();
            }
        }
    }
}
