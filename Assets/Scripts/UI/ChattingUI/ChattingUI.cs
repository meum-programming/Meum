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
        [SerializeField, Range(0, 1000)] private float maxWidth = 140;
        [SerializeField] public RectTransform containerView;
        [SerializeField] public ChattingLogContainer container;

        [Header("Input Area")] 
        [SerializeField] private InputField inputField;
        [SerializeField] private Button sendButton;
        [SerializeField] private Button toggleButton;
        [SerializeField] private float toggleTime = 0.5f;
        
        
        private float _minWidth;
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
            if (Input.GetKeyDown("return")) { 
                Send();
            }
            toggleButton.onClick.RemoveAllListeners();
            toggleButton.onClick.AddListener(Toggle);

            _minWidth = containerView.sizeDelta.x;
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

        private IEnumerator Shrink() //채팅로그창 수축
        {
            var sizeDelta = containerView.sizeDelta;
            var containerPosition = container.transform.localPosition;
            var startSizeDeltaX = sizeDelta.x;
            var startContainerX = containerPosition.x;
            var t = 0.0f;
            while (t < toggleTime)
            {
                sizeDelta.x = Mathf.Lerp(startSizeDeltaX, _minWidth, t / toggleTime);
                containerPosition.x = Mathf.Lerp(startContainerX, 0, t / toggleTime);
                t += Time.deltaTime;
                containerView.sizeDelta = sizeDelta;
                container.transform.localPosition = containerPosition;
                yield return null;
            }

            sizeDelta.x = _minWidth;
            containerPosition.x = 0;
            containerView.sizeDelta = sizeDelta;
            container.transform.localPosition = containerPosition;

            _toggled = false;
        }

        private IEnumerator Expand() //채팅로그창 확대
        {
            var sizeDelta = containerView.sizeDelta;
            var start = sizeDelta.x;
            var t = 0.0f;
            while (t < toggleTime)
            {
                sizeDelta.x = Mathf.Lerp(start, maxWidth, t / toggleTime);
                t += Time.deltaTime;
                containerView.sizeDelta = sizeDelta;
                yield return null;
            }

            sizeDelta.x = maxWidth;
            containerView.sizeDelta = sizeDelta;

            _toggled = true;
        }
        
    }
}
