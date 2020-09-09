using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI.Shop
{
    public class ContentsContainer : MonoBehaviour
    {
        [SerializeField] private Vector2 startPos;
        [SerializeField] private Vector2 offset;
        [SerializeField] private int horizontalN;
        [SerializeField] private float maskHeight;
        [SerializeField] private float loadingThreshold;
        [SerializeField] private GameObject contentPrefab;

        [SerializeField, HideInInspector] private RectTransform _transform;
        private Vector2 _contentSize;
        private bool _scrollToEndActivated = false;

        private void Awake()
        {
            _transform = GetComponent<RectTransform>();
            _contentSize = contentPrefab.GetComponent<RectTransform>().sizeDelta;
        }

        private void OnValidate()
        {
            _contentSize = contentPrefab.GetComponent<RectTransform>().sizeDelta;
            AlignChild();
        }

        private void Update()
        {
            if (_transform.anchoredPosition.y > _transform.sizeDelta.y - maskHeight + loadingThreshold)
            {
                if (_scrollToEndActivated == false)
                {
                    Debug.Log("scrolled to end");
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
                child.transform.localPosition = p;
            }

            var verticalN = childCount / horizontalN + (childCount % horizontalN == 0 ? 0 : 1);
            var containerSize = Vector2.zero;
            containerSize.x = _contentSize.x * horizontalN + offset.x * (horizontalN + 1);
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

