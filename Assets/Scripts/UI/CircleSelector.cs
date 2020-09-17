using System;
using System.Collections;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UI
{
    public class CircleSelector : MonoBehaviour
    {
        [Header("Layout")]
        [SerializeField] private GameObject linePrefab;
        [SerializeField] private GameObject iconPrefab;
        [SerializeField, Range(2, 10)] private int numOptions = 2;
        [SerializeField] private float iconDistanceFromCenter = 300.0f;

        [Header("Expand, Shrink")] 
        [SerializeField] private float chagingTime;
        
        [Header("Active")]
        [SerializeField] private float holdTimeToExpand;
        [SerializeField] private float cancelRadius;

        [System.Serializable]
        public class OptionData
        {
            public string name = "Dummy";
            public Sprite sprite;
            public UnityEvent onSelect;
        }

        [SerializeField] private OptionData[] options;

        [SerializeField, HideInInspector] private Transform[] _lines = null;
        [SerializeField, HideInInspector] private Transform _linesParent;
        [SerializeField, HideInInspector] private Image[] _optionIcons = null;
        [SerializeField, HideInInspector] private Transform _iconsParent;

        private Transform _visibles = null;
        private Vector3 _originalScale = Vector3.zero;
        private IEnumerator _expanding = null;
        private IEnumerator _shrinking = null;

        private Vector3 _dragStartPosition;
        private bool _expanded = false;

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

        private bool CheckMouseInCancelArea(Vector3 mousePos)
        {
            Vector2 delta = mousePos - _dragStartPosition;
            return delta.sqrMagnitude < cancelRadius * cancelRadius;
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
                StopCo(ref _shrinking);
                StopCo(ref _expanding);
                
                _dragStartPosition = Input.mousePosition;
                StartCoroutine(_expanding = Expand());
            }

            if (_expanded)
            {
                foreach (var icon in _optionIcons)
                    icon.color = Color.white;

                var mousePos = Input.mousePosition;
                if (!CheckMouseInCancelArea(mousePos))
                {
                    var selected = FindSelected(mousePos);
                    _optionIcons[selected].color = Color.blue;
                }
            }

            if (Input.GetMouseButtonUp(0))
            {
                StopCo(ref _shrinking);
                StopCo(ref _expanding);
                
                var selected = -1;
                var mousePos = Input.mousePosition;
                if (!CheckMouseInCancelArea(mousePos))
                    selected = FindSelected(mousePos);

                StartCoroutine(_shrinking = Shrink(_expanded && (selected >= 0) ? options[selected].onSelect : null));
            }
        }

        private IEnumerator Expand()
        {
            var t = 0.0f;
            while (t < holdTimeToExpand)
            {
                if (!Input.GetMouseButton(0))
                    yield break;
                t += Time.deltaTime;
                yield return null;
            }

            _expanded = true;
            
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
            _expanded = false;
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
            if(evt != null) 
                evt.Invoke();
            _shrinking = null;
        }

        private void OnValidate()
        {
            #if UNITY_EDITOR
            if (!Application.isEditor || EditorApplication.isPlayingOrWillChangePlaymode)
                return;
            #else
            if (!Application.isEditor)
                return;
            #endif

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
            if (_iconsParent == null)
                _iconsParent = transform.Find("visibles").Find("icons");

            var angleInterval = 360.0f / numOptions;

            if (_optionIcons == null)
            {
                _optionIcons = new Image[numOptions];
                for (var i = 0; i < numOptions; ++i)
                {
                    _optionIcons[i] = Instantiate(iconPrefab, _iconsParent.position, Quaternion.identity, _iconsParent)
                        .GetComponent<Image>();
                    _optionIcons[i].transform.position =
                        _iconsParent.position + Quaternion.AngleAxis(i * angleInterval, Vector3.forward) *
                        Vector3.up * iconDistanceFromCenter;
                }
            }
            else if (_optionIcons.Length != numOptions)
            {
                for (var i = 0; i < _optionIcons.Length; ++i)
                {
                    if (null != _optionIcons[i])
                        DestroyInEditor(_optionIcons[i].gameObject);
                    _optionIcons[i] = null;
                }

                Array.Resize(ref _optionIcons, numOptions);
                for (var i = 0; i < numOptions; ++i)
                {
                    if (_optionIcons[i] == null)
                        _optionIcons[i] =
                            Instantiate(iconPrefab, _iconsParent.position, Quaternion.identity, _iconsParent)
                                .GetComponent<Image>();
                    _optionIcons[i].transform.position =
                        _iconsParent.position + Quaternion.AngleAxis(i * angleInterval, Vector3.forward) *
                        Vector3.up * iconDistanceFromCenter;
                }
            }

            for (var i = 0; i < numOptions; ++i)
            {
                _optionIcons[i].sprite = options[i].sprite;
            }
        }

        private void GenerateLines()
        {
            if (linePrefab == null) return;
            
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
