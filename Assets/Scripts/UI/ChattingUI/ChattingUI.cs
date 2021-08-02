using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

namespace UI.ChattingUI
{
    public class ChattingUI : Core.Singleton<ChattingUI>
    {
        [Header("Log Container")] 
        [SerializeField] private RectTransform container;
        [SerializeField] private GameObject logPrefab;

        [Header("Toggle")] 
        [SerializeField] private Sprite toggleSprite;
        [SerializeField] private Sprite untoggleSprite;
        [SerializeField] private float toggledHeight;

        [Header("Input Area")] 
        //[SerializeField] private TMP_InputField inputField;
        [SerializeField] private InputField inputField;
        [SerializeField] private Button sendButton;
        
        private RectTransform _rectTransform;
        private bool _toggled = false;
        private float _defaultHeight;
        private AudioSource _audioSource;

        public bool showOn = false;

        float enterDelay = 0;

        private void Awake() {
            base.Awake();
            Init();
        }

        private void Init()
        {
            sendButton.onClick.RemoveAllListeners();
            sendButton.onClick.AddListener(Send);
            
            _rectTransform = GetComponent<RectTransform>();
            _defaultHeight = _rectTransform.sizeDelta.y;

            _audioSource = GetComponent<AudioSource>();
        }

        public void OnEndEdit()
        {
            enterDelay = 0.1f;

            if (!EventSystem.current.alreadySelecting)
            {
                Send();
                //inputField.ActivateInputField();
            }
            else
            {
                inputField.GetComponent<WebGLSupport.WebGLInput>().DeactivateInputField();
            }

            //방향키를 계속 입력시 인풋필드가 가끔 활성화 될때가 있어서 처리
            if (EventSystem.current.currentSelectedGameObject == inputField.transform.gameObject)
            {
                EventSystem.current.SetSelectedGameObject(null, new BaseEventData(EventSystem.current));
            }
        }
        public bool InputFieldActivated()
        {
            return inputField.isFocused;
        }

        public void SetInputFieldActive()
        {
            if (InputFieldActivated() || !showOn)
                return;
            
            inputField.ActivateInputField();
            inputField.Select();
        }

        public void AddMessage(string sender, string message, bool isMe, int type)
        {
            var newLog = Instantiate(logPrefab, container);
            var chattingLogCompo = newLog.GetComponent<ChattingLog>();
            chattingLogCompo.SetData(sender, message, type);
            newLog.transform.SetAsLastSibling();

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
            str = str.Trim();
            if (str.Equals("")) return;

            if (str[0] == '/')
            {
                var nickname = str.Split(' ')[0];
                nickname = nickname.Substring(1);

                var target = Core.Socket.DataSynchronizer.Get().Nickname2Id(nickname);
                if (target != -1)
                {
                    var startIdx = str.IndexOf(' ') + 1;
                    var content = str.Substring(startIdx);
                    Debug.Log(content);
                    Core.Socket.MeumSocket.Get().BroadCastChatting(1, target, content);
                }
                else
                {
                    Debug.Log("User " + nickname + " not exist");
                }
            }
            else
                Core.Socket.MeumSocket.Get().BroadCastChatting(0, -1, str);
        }

        public void ShowOn(bool showOn)
        {
            this.showOn = showOn;

            float xMoveValue = showOn ? 0 : -425;
            _rectTransform.DOAnchorPosX(xMoveValue, 0.5f);
        }

        private void Update()
        {
            OnChat();
        }

        void OnChat()
        {
            if (enterDelay > 0)
            {
                enterDelay -= Time.deltaTime;
                return;
            }

            if (Input.GetKeyDown(KeyCode.Return))
            {
                if (showOn == false)
                {
                    ShowOn(true);
                }

                ChattingUI chattingUI = ChattingUI.Get();

                if (chattingUI.InputFieldActivated() == false)
                {
                    chattingUI.SetInputFieldActive();
                }
            }
        }

    }
}
