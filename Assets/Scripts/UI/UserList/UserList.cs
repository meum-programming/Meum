using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.UserList
{
    public class UserList : Core.Singleton<UserList>
    {
        #region SerializedFields
        
        [SerializeField] private TextMeshProUGUI ownerNameText;
        [SerializeField] private TextMeshProUGUI userCountText;
        [SerializeField] private Button toggleButton;
        [SerializeField] private int nameMaxLength;

        [Header("UserList")] 
        [SerializeField] private Transform userList;
        [SerializeField] private GameObject userListContentPrefab;
        [SerializeField] private float defaultExpandedHeight;
        
        #endregion

        #region PrivateFields
        
        private struct PlayerInfo
        {
            public string name;
            public GameObject display;
        }
        private Dictionary<int, PlayerInfo> _playerInfos = new Dictionary<int, PlayerInfo>();
        private bool _toggled = false;
        private float _userListContentHeight;
        private float _defaultHeight;
        private RectTransform _rectTransform;
        
        #endregion

        private void Start()
        {
            _rectTransform = GetComponent<RectTransform>();
            _defaultHeight = _rectTransform.sizeDelta.y;
            
            toggleButton.onClick.AddListener(Toggle);

            _userListContentHeight = userListContentPrefab.GetComponent<RectTransform>().sizeDelta.y;
        }

        private string FitNameMaxLength(string s)
        {
            if (s.Length > nameMaxLength)
                return s.Substring(0, nameMaxLength) + "...";
            else
                return s;
        }

        public void SetOwnerName(string ownerName)
        {
            ownerNameText.text = FitNameMaxLength(ownerName) + "의 믐";
        }

        public void AddUser(int id, string userName)
        {
            PlayerInfo info;
            userName = FitNameMaxLength(userName);
            info.name = userName;

            info.display = Instantiate(userListContentPrefab, userList);
            info.display.GetComponent<UserListContent>().Setup(userName);

            _playerInfos.Add(id, info);
            userCountText.text = _playerInfos.Count.ToString();
            
            RecalculateHeight();
        }

        public void RemoveUser(int id)
        {
            if (_playerInfos.ContainsKey(id))
            {
                Destroy(_playerInfos[id].display);
                _playerInfos.Remove(id);
                userCountText.text = _playerInfos.Count.ToString();
                
                RecalculateHeight();
            }
        }

        public bool HasUser(int id)
        {
            return _playerInfos.ContainsKey(id);
        }

        private void Toggle()
        {
            if (_toggled)
                userList.gameObject.SetActive(false);
            else
                userList.gameObject.SetActive(true);

            var buttonTransform = toggleButton.GetComponent<RectTransform>();
            var buttonLocalScale = buttonTransform.localScale;
            buttonLocalScale.y *= -1.0f;
            buttonTransform.localScale = buttonLocalScale;
            
            _toggled = !_toggled;
            RecalculateHeight();
        }

        private void RecalculateHeight()
        {
            if (_toggled)
            {
                var newHeight = defaultExpandedHeight + _userListContentHeight * _playerInfos.Count;
                _rectTransform.sizeDelta = new Vector2(_rectTransform.sizeDelta.x, newHeight);
            }
            else
            {
                _rectTransform.sizeDelta = new Vector2(_rectTransform.sizeDelta.x, _defaultHeight);
            }
        }
    }
}