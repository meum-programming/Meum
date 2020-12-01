using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI.ChattingUI
{
    public class ChattingUI : Core.Singleton<ChattingUI>
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
        
        private void Awake() {
            base.Awake();
            Init();
        }

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
            if (str == "") return;
            Core.Socket.MeumSocket.Get().BroadCastChatting(str);
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
