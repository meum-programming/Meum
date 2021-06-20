using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

namespace UI.UserList
{
    public class UserList : Core.Singleton<UserList>
    {
        [SerializeField] private Text ownerNameText;
        [SerializeField] private Text userCountText;
        [SerializeField] private Button toggleButton;
        [SerializeField] private int nameMaxLength;

        [Header("UserList")] 
        [SerializeField] private ScrollRect userList;
        [SerializeField] private GameObject userListContentPrefab;
        [SerializeField] private float defaultExpandedHeight;

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

            info.display = Instantiate(userListContentPrefab, userList.content);
            info.display.GetComponent<UserListContent>().Setup(userName);

            _playerInfos.Add(id, info);
            userCountText.text = _playerInfos.Count.ToString();

            UserListHeightSet();
        }

        public void RemoveUser(int id)
        {
            if (_playerInfos.ContainsKey(id))
            {
                Destroy(_playerInfos[id].display);
                _playerInfos.Remove(id);
                userCountText.text = _playerInfos.Count.ToString();

                UserListHeightSet();
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

            float scaleYValue = _toggled ? -1 : 1;
            toggleButton.transform.DOScaleY(scaleYValue, 0);

            _toggled = !_toggled;

            if (_toggled) 
            {
                UserListHeightSet();
            }
        }

        private void UserListHeightSet() 
        {
            //오브젝트 1개당 높이
            float height = userListContentPrefab.GetComponent<RectTransform>().sizeDelta.y;
            //오브젝트들의 갯수
            int cnt = userList.content.childCount;
            //오브젝트들마다 벌어진 거리
            float spacing = userList.content.GetComponent<VerticalLayoutGroup>().spacing;
            //전체 높이
            float scrollRectY = (height * cnt) + (spacing * cnt);

            scrollRectY = Mathf.Min(420, scrollRectY);

            RectTransform scrollRect = userList.GetComponent<RectTransform>();
            Vector2 scrollSizeDelta = scrollRect.sizeDelta;
            scrollSizeDelta.y = scrollRectY;
            scrollRect.sizeDelta = scrollSizeDelta;
        }

        public void OnEndDrag(BaseEventData eventData)
        {
            PointerEventData peData = (PointerEventData)eventData;

            userList.OnEndDrag(peData);

            //아트워크 설명창이 안나오도록 세팅
            EventSystem.current.SetSelectedGameObject(this.gameObject, eventData);
        }
        public void OnPointUp(BaseEventData eventData)
        {
            //아트워크 설명창이 안나오도록 세팅
            EventSystem.current.SetSelectedGameObject(this.gameObject, eventData);
        }

    }
}