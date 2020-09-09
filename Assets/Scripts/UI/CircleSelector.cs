using System;
using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UI
{
    public class CircleSelector : MonoBehaviour
    {
        [Header("Layout")] [SerializeField] private GameObject linePrefab;
        [SerializeField] private GameObject textPrefab;
        [SerializeField, Range(2, 10)] private int numOptions = 2;
        [SerializeField] private float textDistanceFromCenter = 300.0f;

        [Header("Expand, Shrink")] [SerializeField]
        private float chagingTime;

        [System.Serializable]
        public class OptionData
        {
            public string name = "Dummy";
            public UnityEvent onSelect;
        }

        [SerializeField] private OptionData[] options;

        [SerializeField, HideInInspector] private Transform[] _lines = null;
        [SerializeField, HideInInspector] private Transform _linesParent;
        [SerializeField, HideInInspector] private Text[] _optionTexts = null;
        [SerializeField, HideInInspector] private Transform _textsParent;

        private Transform _visibles = null;
        private Vector3 _originalScale = Vector3.zero;
        private IEnumerator _expanding = null;
        private IEnumerator _shrinking = null;

        private Vector3 _dragStartPosition;

        public void OptionTest(int idx)
        {
            Debug.Log(options[idx].name + " invoked");
        }

        private void Start()
        {
            _visibles = transform.Find("visibles");
            _visibles.gameObject.SetActive(false);
            _originalScale = _visibles.transform.localScale;
            _visibles.transform.localScale = Vector3.zero;
        }

        private void StopCo(ref IEnumerator v)
        {
            if (v != null)
            {
                StopCoroutine(v);
                v = null;
            }
        }

        private int FindSelected(Vector3 mousePos)
        {
            Vector2 delta = mousePos - _dragStartPosition;
            delta.Normalize();
            var angle = Mathf.Acos(Vector2.Dot(delta, Vector2.up)) / 3.14f * 180.0f;
            if (delta.x > 0.0f)
                angle = 360.0f - angle;

            var angleInterval = 360.0f / numOptions;
            var initialAngle = angleInterval * 0.5f;
            for (var i = 0; i < numOptions - 1; ++i)
            {
                var start = initialAngle + angleInterval * i;
                if (angle < start + angleInterval && angle > start)
                    return i + 1;
            }

            return 0;
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                _dragStartPosition = Input.mousePosition;
                StopCo(ref _shrinking);
                StopCo(ref _expanding);
                StartCoroutine(_expanding = Expand());
            }

            if (Input.GetMouseButton(0))
            {
                foreach (var text in _optionTexts)
                    text.color = Color.black;
                var selected = FindSelected(Input.mousePosition);
                _optionTexts[selected].color = Color.blue;
            }

            if (Input.GetMouseButtonUp(0))
            {
                var selected = FindSelected(Input.mousePosition);
                StopCo(ref _shrinking);
                StopCo(ref _expanding);
                StartCoroutine(_shrinking = Shrink(options[selected].onSelect));
            }
        }

        private IEnumerator Expand()
        {
            _visibles.gameObject.SetActive(true);
            var v = 0.0f;
            var selfTransform = _visibles.transform;
            var startScale = selfTransform.localScale;
            while (v < 1.0f)
            {
                selfTransform.localScale = Vector3.Lerp(startScale, _originalScale, v);
                v += Time.deltaTime / chagingTime;
                yield return null;
            }

            selfTransform.localPosition = _originalScale;
            _expanding = null;
        }

        private IEnumerator Shrink(UnityEvent evt)
        {
            var v = 0.0f;
            var selfTransform = _visibles.transform;
            var startScale = selfTransform.localScale;
            while (v < 1.0f)
            {
                selfTransform.localScale = Vector3.Lerp(startScale, Vector3.zero, v);
                v += Time.deltaTime / chagingTime;
                yield return null;
            }

            selfTransform.localPosition = Vector3.zero;
            _visibles.gameObject.SetActive(false);
            evt.Invoke();
            _shrinking = null;
        }

        private void OnValidate()
        {
            if (!Application.isEditor || EditorApplication.isPlayingOrWillChangePlaymode)
                return;

            if (options == null)
                options = new OptionData[numOptions];
            else if (options.Length != numOptions)
                Array.Resize(ref options, numOptions);
            for (var i = 0; i < options.Length; ++i)
            {
                if (options[i] == null)
                    options[i] = new OptionData();
            }

            GenerateLines();
            GenerateOptionTexts();
        }

        private void GenerateOptionTexts()
        {
            if (_textsParent == null)
                _textsParent = transform.Find("visibles").Find("texts");

            var angleInterval = 360.0f / numOptions;

            if (_optionTexts == null)
            {
                _optionTexts = new Text[numOptions];
                for (var i = 0; i < numOptions; ++i)
                {
                    _optionTexts[i] = Instantiate(textPrefab, _textsParent.position, Quaternion.identity, _textsParent)
                        .GetComponent<Text>();
                    _optionTexts[i].transform.position =
                        _textsParent.position + Quaternion.AngleAxis(i * angleInterval, Vector3.forward) *
                        Vector3.up * textDistanceFromCenter;
                }
            }
            else if (_optionTexts.Length != numOptions)
            {
                for (var i = 0; i < _optionTexts.Length; ++i)
                {
                    if (null != _optionTexts[i])
                        DestroyInEditor(_optionTexts[i].gameObject);
                    _optionTexts[i] = null;
                }

                Array.Resize(ref _optionTexts, numOptions);
                for (var i = 0; i < numOptions; ++i)
                {
                    if (_optionTexts[i] == null)
                        _optionTexts[i] =
                            Instantiate(textPrefab, _textsParent.position, Quaternion.identity, _textsParent)
                                .GetComponent<Text>();
                    _optionTexts[i].transform.position =
                        _textsParent.position + Quaternion.AngleAxis(i * angleInterval, Vector3.forward) *
                        Vector3.up * textDistanceFromCenter;
                }
            }

            for (var i = 0; i < numOptions; ++i)
            {
                _optionTexts[i].text = options[i].name;
            }
        }

        private void GenerateLines()
        {
            if (_linesParent == null)
                _linesParent = transform.Find("visibles").Find("lines");
            var angleInterval = 360.0f / numOptions;
            var initialAngle = -angleInterval * 0.5f;
            if (_lines == null)
            {
                _lines = new Transform[numOptions];
                for (var i = 0; i < numOptions; ++i)
                {
                    _lines[i] = Instantiate(linePrefab, _linesParent.position, Quaternion.identity, _linesParent)
                        .transform;
                    _lines[i].Rotate(Vector3.forward, initialAngle + angleInterval * i);
                }
            }
            else if (_lines.Length != numOptions)
            {
                for (var i = 0; i < _lines.Length; ++i)
                {
                    if (null != _lines[i])
                        DestroyInEditor(_lines[i].gameObject);
                    _lines[i] = null;
                }

                Array.Resize(ref _lines, numOptions);
                for (var i = 0; i < numOptions; ++i)
                {
                    if (_lines[i] == null)
                        _lines[i] = Instantiate(linePrefab, _linesParent.position, Quaternion.identity, _linesParent)
                            .transform;
                    _lines[i].rotation = Quaternion.identity;
                    _lines[i].Rotate(Vector3.forward, initialAngle + angleInterval * i);
                }
            }
        }

        private void DestroyInEditor(GameObject obj)
        {
            StartCoroutine(DestroyInEditorCoroutine(obj));
        }

        private IEnumerator DestroyInEditorCoroutine(GameObject obj)
        {
            yield return new WaitForEndOfFrame();
            DestroyImmediate(obj);
        }
    }
}
