using System.Collections;
using UnityEngine;
using DG.Tweening;

namespace UI
{
    public class TogglePosition : MonoBehaviour
    {
        [SerializeField] private Vector3 moveOffset;
        [SerializeField] private float moveTime;

        private Vector3 _originalPos;
        private bool _toggled = false;

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
            RectTransform rect = transform.GetComponent<RectTransform>();
            _originalPos = rect.anchoredPosition3D;
        }

        public void Toggle(bool _toggled)
        {
            this._toggled = _toggled;

            RectTransform rect = transform.GetComponent<RectTransform>();

            Vector2 targetPos = _toggled ? _originalPos + moveOffset : _originalPos;

            rect.DOAnchorPos3D(targetPos, moveTime);

            /*
            if (_runningCo != null)
                StopCoroutine(_runningCo);

            if (_toggled)
            {
                StartCoroutine(_runningCo = MoveTo(_originalPos + moveOffset));
            }
            else
            {
                StartCoroutine(_runningCo = MoveTo(_originalPos));
            }
            */
        }

        private IEnumerator MoveTo(Vector3 to)
        {
            RectTransform rect = transform.GetComponent<RectTransform>();

            var t = 0.0f;
            //var selfTransform = transform;
            var from = rect.anchoredPosition3D;

            while (t < 1.0f)
            {
                t += Time.deltaTime / moveTime;
                rect.anchoredPosition3D = Vector3.Lerp(from, to, t);
                yield return null;
            }

            rect.anchoredPosition3D = to;
        }
    }
}