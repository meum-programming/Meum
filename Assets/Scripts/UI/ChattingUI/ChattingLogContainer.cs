using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI.ChattingUI
{
    public class ChattingLogContainer : MonoBehaviour
    {
        [SerializeField] private GameObject logPrefab;
        [SerializeField] private float offset;

        private RectTransform _transform;
        private float _prefabHeight;

        private void Awake()
        {
            _transform = GetComponent<RectTransform>();
            _prefabHeight = logPrefab.GetComponent<RectTransform>().sizeDelta.y;
        }

        public void Add(string sender, string message)
        {
            var newLog = Instantiate(logPrefab, transform);
            newLog.GetComponent<Text>().text = String.Format("{0}: {1} ", sender, message);
            Align();
        }

        private void Align()
        {
            var childCount = _transform.childCount;

            for (var i = 0; i < childCount; ++i)
            {
                var child = _transform.GetChild(childCount - 1 - i);
                child.localPosition = new Vector3(0, (_prefabHeight + offset) * i, 0);
            }

            var sizeDelta = _transform.sizeDelta;
            sizeDelta.y = (_prefabHeight + offset) * childCount;
            _transform.sizeDelta = sizeDelta;
        }
    }
}
