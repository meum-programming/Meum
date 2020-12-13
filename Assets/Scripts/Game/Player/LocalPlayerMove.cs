using UnityEngine;
using UnityEngine.InputSystem;

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
        #region Constants
        
        private const float GRAVITY = 9.8f;
        
        #endregion
        
        #region SerializeFields
        
        [SerializeField] private float walkSpeed;
        [SerializeField] private float runSpeed;
        [SerializeField] private float jumpHeight;

        [Header("Ground Checking")] 
        [SerializeField] private float groundDetectDistance = 0.1f;
        [SerializeField] private float jumpEndDetectDistance = 0.7f;
        [SerializeField] private LayerMask checkingMask;
        
        #endregion

        #region PrivateFields
        
        private Vector3 _colliderCenterPos;
        private float _distFromColliderCenterToGround;

        private CharacterController _charController;
        private PlayerAnimationController _animController;

        private Vector3 _moveVector = Vector3.zero;
        private float _velocityY;
        private bool _running = false;
        
        #endregion
        
        private void Awake()
        {
            _charController = GetComponent<CharacterController>();
            _animController = GetComponent<PlayerAnimationController>();

            _colliderCenterPos = _charController.center;
            _distFromColliderCenterToGround = _charController.bounds.extents.y;
        }

        private void Update()
        {
            Move();
            ApplyGravity();
            
            _animController.SetIsJumpEnded(IsJumpEnded());
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
            if (UI.ChattingUI.ChattingUI.Get().InputFieldActivated())
            {
                _moveVector = Vector3.zero;
                return;
            }
            
            var selfTransform = transform;
            var direction = selfTransform.right * _moveVector.x + 
                            selfTransform.forward * _moveVector.y;
            direction.Normalize();

            var delta = Vector3.up * _velocityY + direction * walkSpeed;
            _charController.Move(delta * Time.deltaTime);
        }

        private bool IsGrounded()
        {
            return Physics.Raycast(
                transform.position + _colliderCenterPos,
                -Vector3.up,
                _distFromColliderCenterToGround + groundDetectDistance,
                checkingMask.value);
        }

        private bool IsJumpEnded()
        {
            return Physics.Raycast(
                transform.position + _colliderCenterPos,
                -Vector3.up,
                _distFromColliderCenterToGround + jumpEndDetectDistance,
                checkingMask.value);
        }

        public void OnMove(InputAction.CallbackContext ctx)
        {
            if (UI.ChattingUI.ChattingUI.Get().InputFieldActivated())
            {
                _animController.SetHorizontalSpeed(0.0f);
                _animController.SetVerticalSpeed(0.0f);
                return;
            }

            var value = ctx.ReadValue<Vector2>();
            _moveVector = value;

            // animation
            _animController.SetHorizontalSpeed(value.x);
            _animController.SetVerticalSpeed(value.y);
        }

        public void OnJump(InputAction.CallbackContext ctx)
        {
            if (UI.ChattingUI.ChattingUI.Get().InputFieldActivated())
                return;
            if (ctx.performed && IsGrounded())
            {
                _velocityY = Mathf.Sqrt(2.0f * GRAVITY * jumpHeight);
                _animController.SetJumpTrigger();
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
    }
}
