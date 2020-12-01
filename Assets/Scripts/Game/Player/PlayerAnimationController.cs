using Core.Socket;
using UnityEngine;

namespace Game.Player
{
    public class PlayerAnimationController : MonoBehaviour
    {
        private Animator _animator;

        private struct AnimInfo
        {
            public bool isJumpEnded;
            public float horizontalSpeed;
            public float verticalSpeed;
        }

        private AnimInfo _prev;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        private void Start()
        {
            _prev.isJumpEnded = _animator.GetBool(Animator.StringToHash("isJumpEnded"));
            _prev.horizontalSpeed = _animator.GetFloat(Animator.StringToHash("horizontalSpeed"));
            _prev.verticalSpeed = _animator.GetFloat(Animator.StringToHash("verticalSpeed"));
        }

        public void SetJumpTrigger()
        {
            _animator.SetTrigger(Animator.StringToHash("jump"));
            MeumSocket.Get().BroadCastAnimTrigger("jump");
        }

        public void SetIsJumpEnded(bool value)
        {
            if (value == _prev.isJumpEnded) return;
            _animator.SetBool(Animator.StringToHash("isJumpEnded"), value);
            _prev.isJumpEnded = value;
            MeumSocket.Get().BroadCastAnimBoolChange("isJumpEnded", value);
        }

        public void SetHorizontalSpeed(float value)
        {
            if (Equal(value, _prev.horizontalSpeed)) return;
            _animator.SetFloat(Animator.StringToHash("horizontalSpeed"), value);
            _prev.horizontalSpeed = value;
            MeumSocket.Get().BroadCastAnimFloatChange("horizontalSpeed", value);
        }

        public void SetVerticalSpeed(float value)
        {
            if (Equal(value, _prev.verticalSpeed)) return;
            _animator.SetFloat(Animator.StringToHash("verticalSpeed"), value);
            _prev.verticalSpeed = value;
            MeumSocket.Get().BroadCastAnimFloatChange("verticalSpeed", value);
        }

        private static bool Equal(float a, float b)
        {
            const float threshold = 1e-5f;
            return Mathf.Abs(a - b) < threshold;
        }
    }
}
