using UnityEngine;

namespace UI
{
    public class ContentsContainer : MonoBehaviour
    {
        [SerializeField] private Vector2 startPos;
        [SerializeField] private Vector2 offset;
        [SerializeField] private int horizontalN;
        [SerializeField] private float loadingThreshold;
        [SerializeField] private GameObject contentPrefab;

        [SerializeField, HideInInspector] private RectTransform _transform;
        private Vector2 _contentSize;
        private float _maskHeight;
        private bool _scrollToEndActivated = false;

        private void Awake()
        {
            _transform = GetComponent<RectTransform>();
            _contentSize = contentPrefab.GetComponent<RectTransform>().sizeDelta;
            _maskHeight = (transform.parent as RectTransform).rect.height;
        }

        private void OnValidate()
        {
            _contentSize = contentPrefab.GetComponent<RectTransform>().sizeDelta;
            AlignChild();
        }

        private void Update()
        {
            if (_transform.anchoredPosition.y > _transform.sizeDelta.y - _maskHeight + loadingThreshold)
            {
                if (_scrollToEndActivated == false)
                {
                    _scrollToEndActivated = true;
                }
            }
            else _scrollToEndActivated = false;
        }

        private void AlignChild()
        {
            if (_transform == null) _transform = GetComponent<RectTransform>();
            var childCount = _transform.childCount;
            for (var i = 0; i < childCount; ++i)
            {
                var child = _transform.GetChild(i);
                var x = i % horizontalN;
                var y = i / horizontalN;
                var p = startPos;
                p.x += (offset.x + _contentSize.x) * x;
                p.y -= (offset.y + _contentSize.y) * y;
                child.localPosition = p;
            }

            var verticalN = childCount / horizontalN + (childCount % horizontalN == 0 ? 0 : 1);
            var containerSize = Vector2.zero;
            containerSize.y = _contentSize.y * verticalN + offset.y * (verticalN + 1);
            _transform.sizeDelta = containerSize;
        }

        public void AddContent(ContentData data)
        {
            var newContentObj = Instantiate(contentPrefab, transform);
            newContentObj.GetComponent<Content>().data = data;
            AlignChild();
        }

        public void AddContents(ContentData[] data)
        {
            foreach(var v in data)
            {
                var newContentObj = Instantiate(contentPrefab, transform);
                newContentObj.GetComponent<Content>().data = v;
            }
            AlignChild();
        }
    }
}

