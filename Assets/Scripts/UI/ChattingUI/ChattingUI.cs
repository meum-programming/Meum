using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI.ChattingUI
{
    public class ChattingUI : MonoBehaviour
    {
        [Header("Log Container")] 
        [SerializeField] private RectTransform container;
        [SerializeField] private Image containerBackground;
        [SerializeField] private GameObject logPrefab;

        [Header("Toggle")] 
        [SerializeField] private Sprite toggleSprite;
        [SerializeField] private Sprite untoggleSprite;
        [SerializeField] private Button toggleButton;
        [SerializeField] private float toggledHeight;

        [Header("Input Area")] 
        [SerializeField] private InputField inputField;
        [SerializeField] private Button sendButton;
        
        private RectTransform _rectTransform;
        private bool _toggled = false;
        private float _defaultHeight;
        private AudioSource _audioSource;
        
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
            return !ReferenceEquals(_instance, null);
        }

        public static ChattingUI Get()
        {
            return _instance;
        }

        private void OnDestroy()
        {
            _instance = null;
        }

        #endregion

        private void Init()
        {
            sendButton.onClick.RemoveAllListeners();
            sendButton.onClick.AddListener(Send);
            
            toggleButton.onClick.RemoveAllListeners();
            toggleButton.onClick.AddListener(Toggle);

            _rectTransform = GetComponent<RectTransform>();
            _defaultHeight = _rectTransform.sizeDelta.y;

            _audioSource = GetComponent<AudioSource>();
        }

        private void FixedUpdate()
        {
            // if (!InputFieldActivated() && Input.GetKey(KeyCode.Return))
            // {
            //     Debug.Log("Return : activate input field");
            //     inputField.ActivateInputField();
            // }
            if (!InputFieldActivated() && Input.GetKeyDown(KeyCode.C))
            {
                toggleButton.onClick.Invoke();
            }
        }

        public void OnEndEdit()
        {
            if (!EventSystem.current.alreadySelecting)
            {
                Debug.Log("Submit");
                Send();
                inputField.ActivateInputField();
            }
            else
            {
                Debug.Log("Deactivate");
                inputField.GetComponent<WebGLSupport.WebGLInput>().DeactivateInputField();
            }
        }
        public bool InputFieldActivated()
        {
            return inputField.isFocused;
        }

        public void AddMessage(string sender, string message, bool isMe)
        {
            var newLog = Instantiate(logPrefab, container);
            var chattingLogCompo = newLog.GetComponent<ChattingLog>();
            chattingLogCompo.SetData(sender, message, isMe);
            newLog.transform.SetAsFirstSibling();

            var containerPos = container.anchoredPosition;
            containerPos.y = 0;
            container.anchoredPosition = containerPos;
            
            if(!isMe)
                _audioSource.PlayOneShot(_audioSource.clip);
        }

        private void Send()
        {
            SendWithStr(inputField.text);
            inputField.text = "";
        }

        private void SendWithStr(string str)
        {
            if (str == "") return;
            Global.Socket.MeumSocket.Get().BroadCastChatting(str);
        }

        private void Toggle()
        {
            if (_toggled)
            {
                var btnImageCompo = toggleButton.GetComponent<Image>();
                btnImageCompo.sprite = toggleSprite;
                
                _rectTransform.sizeDelta = new Vector2(_rectTransform.sizeDelta.x, _defaultHeight);

                _toggled = false;
            }
            else
            {
                var btnImageCompo = toggleButton.GetComponent<Image>();
                btnImageCompo.sprite = untoggleSprite;
                
                _rectTransform.sizeDelta = new Vector2(_rectTransform.sizeDelta.x, toggledHeight);
                
                _toggled = true;
            }
        }
    }
}
