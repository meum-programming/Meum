using System.Collections;
using UnityEngine;

namespace UI
{
    public class TogglePosition : MonoBehaviour
    {
        [SerializeField] private Vector3 moveOffset;
        [SerializeField] private float moveTime;

        private Vector3 _originalPos;
        private bool _toggled = true;

        public bool toggled
        {
            get 
            {
                return _toggled;
            }
        }

        private IEnumerator _runningCo = null;

        private void Awake()
        {
            _originalPos = transform.position;
        }

        public void Toggle()
        {
            if (_runningCo != null)
                StopCoroutine(_runningCo);
            if (_toggled)
            {
                StartCoroutine(_runningCo = MoveTo(_originalPos + moveOffset));
                _toggled = false;
            }
            else
            {
                StartCoroutine(_runningCo = MoveTo(_originalPos));
                _toggled = true;
            }
        }

        private IEnumerator MoveTo(Vector3 to)
        {
            var t = 0.0f;
            var selfTransform = transform;
            var from = selfTransform.position;

            while (t < 1.0f)
            {
                t += Time.deltaTime / moveTime;
                selfTransform.position = Vector3.Lerp(from, to, t);
                yield return null;
            }

            selfTransform.position = to;
        }
    }
}