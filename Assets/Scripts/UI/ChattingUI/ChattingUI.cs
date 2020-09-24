using System;
using System.Collections;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnitySocketIO.Events;

namespace UI.ChattingUI
{
    public class ChattingUI : MonoBehaviour
    {
        [Header("Log Container")]
        [SerializeField, Range(300, 1000)] private float maxHeight = 500;
        [SerializeField] public RectTransform containerView;
        [SerializeField] public ChattingLogContainer container;

        [Header("Input Area")] 
        [SerializeField] private InputField inputField;
        [SerializeField] private Button sendButton;
        [SerializeField] private Button toggleButton;
        [SerializeField] private float toggleTime = 0.5f;
        
        
        private float _minHeight;
        private bool _toggled = false;
        private IEnumerator _runningCo = null;
        
        #region Singleton
        private static ChattingUI _instance = null;
        private void Awake() {
            if(InstanceExist()) {
                Debug.LogError("UI.ChattingUI.ChattingUI - Awake : only one ChattingUI can be exist");
                Application.Quit(-1);
            }
            Init();
            _instance = this;
        }
        
        public static bool InstanceExist() {
            return _instance != null;
        }

        public static ChattingUI Get()
        {
            return _instance;
        }
        #endregion

        private void Init()
        {
            sendButton.onClick.RemoveAllListeners();
            sendButton.onClick.AddListener(Send);
            
            toggleButton.onClick.RemoveAllListeners();
            toggleButton.onClick.AddListener(Toggle);

            _minHeight = containerView.sizeDelta.y;
        }

        public bool InputFieldActivated()
        {
            return inputField.isFocused;
        }

        public void AddMessage(string sender, string message)
        {
            container.Add(sender, message);
        }

        private void Send()
        {
            SendWithStr(inputField.text);
        }

        private void SendWithStr(string str)
        {
            Global.Socket.MeumSocket.Get().BroadCastChatting(str);
            inputField.text = "";
        }

        private void Toggle()
        {
            if (_toggled)
            {
                if(_runningCo != null)
                    StopCoroutine(_runningCo);
                StartCoroutine(_runningCo = Shrink());
            }
            else
            {
                if(_runningCo != null)
                    StopCoroutine(_runningCo);
                StartCoroutine(_runningCo = Expand());
            }
        }

        private IEnumerator Shrink()
        {
            var sizeDelta = containerView.sizeDelta;
            var containerPosition = container.transform.localPosition;
            var startSizeDeltaY = sizeDelta.y;
            var startContainerY = containerPosition.y;
            var t = 0.0f;
            while (t < toggleTime)
            {
                sizeDelta.y = Mathf.Lerp(startSizeDeltaY, _minHeight, t / toggleTime);
                containerPosition.y = Mathf.Lerp(startContainerY, 0, t / toggleTime);
                t += Time.deltaTime;
                containerView.sizeDelta = sizeDelta;
                container.transform.localPosition = containerPosition;
                yield return null;
            }

            sizeDelta.y = _minHeight;
            containerPosition.y = 0;
            containerView.sizeDelta = sizeDelta;
            container.transform.localPosition = containerPosition;

            _toggled = false;
        }

        private IEnumerator Expand()
        {
            var sizeDelta = containerView.sizeDelta;
            var start = sizeDelta.y;
            var t = 0.0f;
            while (t < toggleTime)
            {
                sizeDelta.y = Mathf.Lerp(start, maxHeight, t / toggleTime);
                t += Time.deltaTime;
                containerView.sizeDelta = sizeDelta;
                yield return null;
            }

            sizeDelta.y = maxHeight;
            containerView.sizeDelta = sizeDelta;

            _toggled = true;
        }
        
    }
}
