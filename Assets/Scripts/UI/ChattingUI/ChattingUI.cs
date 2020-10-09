using System;
using System.Collections;
using System.Collections.Generic;
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
        [SerializeField] public ChattingLogContainer container;
        [SerializeField] private float scrollDownAnimTime;

        [Header("Toggle")] 
        [SerializeField] private Sprite visibleSprite;
        [SerializeField] private Sprite invisibleSprite;
        [SerializeField] private GameObject toggledObjs;
        [SerializeField] private Button toggleButton;
        [SerializeField] private float toggleButtonScaleFactor;

        [Header("Badge")] 
        [SerializeField] private GameObject badge;
        [SerializeField] private Text badgeText;

        [Header("Input Area")] 
        [SerializeField] private InputField inputField;
        [SerializeField] private Button sendButton;
        
        private bool _toggled = false;
        private int _notReadCnt = 0;
        private List<ChattingLog> _badgeBeenSet = new List<ChattingLog>();

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

            _audioSource = GetComponent<AudioSource>();
        }

        private void OnValidate()
        {
            // set togglebutton scale
            var btnImageCompo = toggleButton.GetComponent<Image>();
            var btnTransform = toggleButton.transform as RectTransform;
            btnImageCompo.sprite = invisibleSprite;
            btnImageCompo.SetNativeSize();
            btnTransform.sizeDelta *= toggleButtonScaleFactor;
        }

        private void FixedUpdate()
        {
            if (_toggled && Input.GetKeyDown(KeyCode.Return))
            {
                if (inputField.isFocused)
                    Send();
                inputField.ActivateInputField();
            }

            if (Input.GetKeyDown(KeyCode.C))
            {
                toggleButton.onClick.Invoke();
            }
        }

        public bool InputFieldActivated()
        {
            return inputField.isFocused;
        }

        public void AddMessage(string sender, string message, bool isMe)
        {
            var chattingLog = container.Add(sender, message, isMe);
            
            if(!isMe)
                _audioSource.PlayOneShot(_audioSource.clip);

            if (!_toggled)
            {
                _notReadCnt += 1;
                badge.SetActive(true);
                badgeText.text = _notReadCnt > 999 ? "999+" : _notReadCnt.ToString();

                if (!isMe)
                {
                    chattingLog.EnableNotReadBadge();
                    _badgeBeenSet.Add(chattingLog);
                }
            }
        }

        private void Send()
        {
            Debug.Log("Send called");
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
                toggledObjs.SetActive(false);
                var btnImageCompo = toggleButton.GetComponent<Image>();
                var btnTransform = toggleButton.transform as RectTransform;
                btnImageCompo.sprite = invisibleSprite;
                btnImageCompo.SetNativeSize();
                btnTransform.sizeDelta *= toggleButtonScaleFactor;
                
                // remove badges
                foreach(var chattingLog in _badgeBeenSet)
                    chattingLog.DisableNotReadBadge();
                _badgeBeenSet.Clear();
                
                _toggled = false;
            }
            else
            {
                toggledObjs.SetActive(true);
                var btnImageCompo = toggleButton.GetComponent<Image>();
                var btnTransform = toggleButton.transform as RectTransform;
                btnImageCompo.sprite = visibleSprite;
                btnImageCompo.SetNativeSize();
                btnTransform.sizeDelta *= toggleButtonScaleFactor;
                
                badge.SetActive(false);
                badgeText.text = "";
                _notReadCnt = 0;
                _toggled = true;
            }
        }

        private IEnumerator ScrollDownContainerCoroutine()
        {
            var t = 0.0f;
            var containerTransform = container.transform;
            var originalPos = containerTransform.localPosition;
            var targetPos = new Vector3(originalPos.x, 0, originalPos.z);
            while (t < 1.0f)
            {
                t += Time.deltaTime / scrollDownAnimTime;
                containerTransform.localPosition = Vector3.Lerp(originalPos, targetPos, t);
                yield return null;
            }

            containerTransform.localPosition = targetPos;
        }
        public void ScrollDownContainer()
        {
            StartCoroutine(ScrollDownContainerCoroutine());
        }
    }
}
