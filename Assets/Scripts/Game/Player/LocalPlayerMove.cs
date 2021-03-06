using UnityEngine;
using DG.Tweening;
using UI.ChattingUI;
using System.Collections;

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
        [SerializeField] private Transform cameraPivot;

        public bool portalMoveOn = false;

        float portalMoveNotDelay = 0;

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
            if (portalMoveOn)
            {
                _moveVector = Vector3.zero;
                portalMoveOn = false;
                return;
            }

            CenterCheck();
            Move();
            ApplyGravity();

            _animController.SetIsJumpEnded(IsJumpEnded());

            InputEventCheck();
        }

        void CenterCheck()
        {
            if (_charController.center != _colliderCenterPos)
            {
                _charController.center = _colliderCenterPos;
            }
        }

        private void ApplyGravity()
        {
            if (IsGrounded())
                _velocityY = 0.0f;
            else
                _velocityY -= GRAVITY * Time.deltaTime;
        }

        public void ChaLookAtForward()
        {
            float y = cameraPivot.localRotation.eulerAngles.y;
            transform.DOLocalRotate(new Vector3(0, y, 0), 0.5f);
        }

        void InputEventCheck()
        {
            OnJump();
            OnRun();
            OnMove();
        }

        void OnJump()
        {
            if (DataManager.Instance.NotActiveKey())
                return;

            if (!Input.GetKeyDown(KeyCode.Space))
                return;
            
            if(IsGrounded())
            {
                _velocityY = Mathf.Sqrt(2.0f * GRAVITY * jumpHeight);
                _animController.SetJumpTrigger();

                isJumped = true;
                jumpEndDeley = 0.5f;
            }
        }

        void OnRun()
        {
            if (DataManager.Instance.NotActiveKey())
                return;

            _running = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
            _animController.SetRunning(_running);
        }

        void OnMove()
        {
            if (DataManager.Instance.NotActiveKey())
                return;

            Vector2 moveValue = Vector2.zero;

            if (Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.S) || Input.GetKeyUp(KeyCode.D) || Input.GetKeyUp(KeyCode.UpArrow) || Input.GetKeyUp(KeyCode.DownArrow))
            {
                moveValue = Vector2.zero;
            }
            else
            {
                if (Input.GetKey(KeyCode.W))
                {
                    moveValue.y += 1;
                }
                if (Input.GetKey(KeyCode.S))
                {
                    moveValue.y -= 1;
                }
                if (Input.GetKey(KeyCode.A))
                {
                    moveValue.x -= 1;
                }
                if (Input.GetKey(KeyCode.D))
                {
                    moveValue.x += 1;
                }
                if (Input.GetKey(KeyCode.UpArrow))
                {
                    moveValue.y += 1;
                }
                if (Input.GetKey(KeyCode.DownArrow))
                {
                    moveValue.y -= 1;
                }
            }

            //조이스틱 방향이 대각선이라면
            if(moveValue.x != 0 && moveValue.y != 0)
            {
                moveValue = new Vector2(moveValue.x * 0.74f, moveValue.y * 0.74f);
            }

            MoveVectorSet(moveValue);
        }

        public void OnMoveJoystick(Vector2 value)
        {
            MoveVectorSet(value);
        }

        void MoveVectorSet(Vector2 value)
        {
            if (DataManager.Instance.NotActiveKey())
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

        private void Move()
        {
            if (DataManager.Instance.NotActiveKey())
            {
                _moveVector = Vector3.zero;
               // return;
            }

            if (_moveVector != Vector3.zero)
            {
                ChaLookAtForward();
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

        void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Portal"))
            {
                Teleporting teleporting = other.GetComponent<Teleporting>();

                if (teleporting == null)
                    return;

                portalMoveOn = true;

                Transform target = teleporting.teleportTarget;
                //위치 이동
                transform.position = target.position;

                //회전값 수정
                transform.rotation = target.rotation;
                cameraPivot.rotation = target.rotation;
            }
        }

    }
}
