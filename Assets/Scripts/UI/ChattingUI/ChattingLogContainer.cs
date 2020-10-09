using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI.ChattingUI
{
    public class ChattingLogContainer : MonoBehaviour
    {
        [SerializeField] private GameObject logPrefabMe;
        [SerializeField] private GameObject logPrefabYou;
        [SerializeField] private float offset;

       private RectTransform _transform;

        private void Awake()
        {
            _transform = GetComponent<RectTransform>();
            var sizeDelta = _transform.sizeDelta;
            sizeDelta.x = _transform.parent.GetComponent<RectTransform>().rect.width;
            _transform.sizeDelta = sizeDelta;
        }

        public ChattingLog Add(string sender, string message, bool isMe)
        {
            var newLog = Instantiate(isMe ? logPrefabMe : logPrefabYou, transform);
            var chattingLogCompo = newLog.GetComponent<ChattingLog>();
            chattingLogCompo.SetData(sender, message, isMe);
            Align();
            return chattingLogCompo;
        }

        private void Align()
        {
            if(_transform == null)
                _transform = GetComponent<RectTransform>();
            
            var childCount = _transform.childCount;
            var height = 0.0f;

            for (var i = 0; i < childCount; ++i)
            {
                var child = _transform.GetChild(childCount - 1 - i);
                var chattingLog = child.GetComponent<ChattingLog>();
                
                child.localPosition = new Vector3(chattingLog.isMe ? _transform.sizeDelta.x : 0,
                                                  height, 0);
                height += chattingLog.height + offset;
            }

            var sizeDelta = _transform.sizeDelta;
            sizeDelta.y = height;
            _transform.sizeDelta = sizeDelta;
        }
    }
}
