using Core.Socket;
using UnityEngine;

namespace Game.Player
{
    /*
     * @brief 플레이어의 애니메이션 제어를 Wrap 해 놓음 (Socket.io 이벤트를 위함)
     */
    public class PlayerAnimationController : MonoBehaviour
    {
        public Animator _animator;

        private struct AnimInfo
        {
            public bool isJumpEnded;
            public float horizontalSpeed;
            public float verticalSpeed;
            public bool running;
        }

        private AnimInfo _prev;

        private void Awake()
        {
            //_animator =  GetComponent<Animator>();
        }

        private void Start()
        {
            _prev.isJumpEnded = _animator.GetBool(Animator.StringToHash("isJumpEnded"));
            _prev.horizontalSpeed = _animator.GetFloat(Animator.StringToHash("horizontalSpeed"));
            _prev.verticalSpeed = _animator.GetFloat(Animator.StringToHash("verticalSpeed"));
            _prev.running = _animator.GetBool(Animator.StringToHash("running"));
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

            if (value != 0)
            {
                _animator.Play("Human_Walking_Forward");
                MeumSocket.Get().BroadCastAnimGesture("Human_Walking_Forward");
            }

            _animator.SetFloat(Animator.StringToHash("horizontalSpeed"), value);
            _prev.horizontalSpeed = value;
            MeumSocket.Get().BroadCastAnimFloatChange("horizontalSpeed", value);
        }

        public void SetVerticalSpeed(float value)
        {
            if (Equal(value, _prev.verticalSpeed)) return;

            if (value != 0)
            {
                _animator.Play("Human_Walking_Forward");
                MeumSocket.Get().BroadCastAnimGesture("Human_Walking_Forward");
            }

            _animator.SetFloat(Animator.StringToHash("verticalSpeed"), value);
            _prev.verticalSpeed = value;
            MeumSocket.Get().BroadCastAnimFloatChange("verticalSpeed", value);
        }

        public void SetRunning(bool value)
        {
            if (value == _prev.running) return;
            _animator.SetBool(Animator.StringToHash("running"), value);
            _prev.running = value;
            MeumSocket.Get().BroadCastAnimBoolChange("running", value);
        }

        private static bool Equal(float a, float b)
        {
            const float threshold = 1e-5f;
            return Mathf.Abs(a - b) < threshold;
        }


        public void PlayGestureAnim(string animName)
        {
            if (animName != string.Empty)
            {
                _animator.Play(animName);
                MeumSocket.Get().BroadCastAnimGesture(animName);
            }
        }


    }
}
