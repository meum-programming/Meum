using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnitySocketIO;
using UnitySocketIO.Events;

namespace Gallery.MultiPlay
{
    public class DataSyncer : MonoBehaviour
    {
        [Header("Connection Info")] [SerializeField]
        private float sendInterval;

        [SerializeField] private float receiveInterval;
        private float _sendIntervalCounter = 0.0f;

        [Header("Require Prefabs")] [SerializeField]
        private List<GameObject> playerPrefabs;

        [SerializeField] private List<GameObject> otherPlayerPrefabs;

        [Header("Scenes")] [SerializeField] private List<string> scenes;

        private Transform _player = null;
        private Animator _playerAnimator = null;
        private int _currentCharId = 0;

        public int currentCharId
        {
            get { return _currentCharId; }
        }

        private OtherPlayerController[] _others = null;

        private SocketIOController _socket;
        private int _id = -1;

        private void Start()
        {
            transform.position = Vector3.zero;
            transform.rotation = Quaternion.identity;
            transform.localScale = new Vector3(1, 1, 1);

            _socket = GetComponent<SocketIOController>();
            _socket.On("enteringSuccess", OnEnteringSuccess);
            _socket.On("userQuit", OnUserQuit);
            _socket.On("userInfo", OnUserInfo);
            _socket.On("animTrigger", OnAnimTrigger);
            _socket.On("changeCharacter", OnChangeCharacter);
            _socket.Connect();
        }

        private void Update()
        {
            if (_id != -1)
            {
                _sendIntervalCounter += Time.deltaTime;
                if (_sendIntervalCounter > sendInterval)
                {
                    BroadCastUserInfo();
                    _sendIntervalCounter = 0.0f;
                }
            }
        }

        public void ResetRoom()
        {
            _player = null;
            _playerAnimator = null;
            _id = -1;
            for (int i = 0; i < _others.Length; ++i)
            {
                if (_others[i] == null) continue;
                Destroy(_others[i].gameObject);
            }
        }

        #region Socket Event Emitters

        private struct BroadCastUserInfoData
        {
            public Vector3 position;
            public Vector3 rotation;
            public bool walking;
        }

        private void BroadCastUserInfo()
        {
            BroadCastUserInfoData data;
            data.position = _player.position;
            data.rotation = _player.eulerAngles;
            data.walking = _playerAnimator.GetBool(Animator.StringToHash("walking"));
            _socket.Emit("broadCastUserInfo", JsonConvert.SerializeObject(data));
        }

        private struct BroadCastAnimTriggerData
        {
            public string name;
        }

        public void BroadCastAnimTrigger(string paramName)
        {
            BroadCastAnimTriggerData data;
            data.name = paramName;
            _socket.Emit("broadCastAnimTrigger", JsonConvert.SerializeObject(data));
        }

        private struct ChangeCharacterData
        {
            public int charId;
        }

        public void ChangeCharacter(int charId)
        {
            ChangeCharacterData data;
            data.charId = charId;
            _socket.Emit("broadCastChangeCharacter", JsonConvert.SerializeObject(data));
        }

        #endregion

        #region Socket Event Handlers

        private struct EnteringSuccessEventData
        {
            public int id;
            public int roomId;
            public int roomType;
            public int maxN;
        }

        private IEnumerator WaitLoadingGallery(SocketIOEvent e)
        {
            var data = JsonConvert.DeserializeObject<EnteringSuccessEventData>(e.data);
            var sceneName = scenes[data.roomType];
            var loading = SceneManager.LoadSceneAsync(sceneName);
            while (!loading.isDone) yield return null;

            _id = data.id;
            _others = new OtherPlayerController[data.maxN];

            var spawnTransform = GameObject.Find("SpawnSite").transform;
            var spawnPos = spawnTransform.position;
            var spawnRot = spawnTransform.rotation;
            for (int i = 0; i < _others.Length; ++i)
            {
                if (i == _id) continue;
                _others[i] = Instantiate(otherPlayerPrefabs[0], transform).GetComponent<OtherPlayerController>();
                _others[i].gameObject.SetActive(false);
                _others[i].UpdateTransform(spawnPos, spawnRot.eulerAngles);
                _others[i].SetOriginalTransform(spawnPos, spawnRot);
            }

            Debug.Log("My id is " + _id);
            Debug.Log("maxN is " + _others.Length);
            Debug.Log("room id is " + data.roomId);

            _player = Instantiate(playerPrefabs[0]).transform;
            _playerAnimator = _player.GetComponent<Animator>();
            _currentCharId = 0;
            _player.position = spawnTransform.position;
            _player.rotation = spawnTransform.rotation;
        }

        private void OnEnteringSuccess(SocketIOEvent e)
        {
            StartCoroutine(WaitLoadingGallery(e));
        }

        private struct UserQuitEventData
        {
            public int id;
        }

        private void OnUserQuit(SocketIOEvent e)
        {
            var data = JsonConvert.DeserializeObject<UserQuitEventData>(e.data);
            Debug.Log(data.id + ", " + _id);
            if (data.id == _id)
            {
                SceneManager.LoadScene("Lobby");
                ResetRoom();
            }
            else
            {
                _others[data.id].gameObject.SetActive(false);
            }
        }

        private struct UserInfoEventData
        {
            public int id;
            public Vector3 position;
            public Vector3 rotation;
            public bool walking;
        }

        private void OnUserInfo(SocketIOEvent e)
        {
            var data = JsonConvert.DeserializeObject<UserInfoEventData>(e.data);
            if (data.id == _id) return;
            var obj = _others[data.id];
            obj.gameObject.SetActive(true);
            obj.UpdateTransform(data.position, data.rotation);
            obj.AnimBoolChange(Animator.StringToHash("walking"), data.walking);
        }

        private struct AnimTriggerEventData
        {
            public int id;
            public string name;
        }

        private void OnAnimTrigger(SocketIOEvent e)
        {
            var data = JsonConvert.DeserializeObject<AnimTriggerEventData>(e.data);
            if (data.id == _id) return;
            var obj = _others[data.id];
            if (obj.isActiveAndEnabled)
                obj.AnimTrigger(Animator.StringToHash(data.name));
        }

        private struct ChangeCharacterEventData
        {
            public int id;
            public int charId;
        }

        private void OnChangeCharacter(SocketIOEvent e)
        {
            var data = JsonConvert.DeserializeObject<ChangeCharacterEventData>(e.data);
            if (data.id == _id)
            {
                if (_currentCharId == data.charId) return;
                var playerOld = _player;
                var pPosition = playerOld.position;
                var pRotation = playerOld.rotation;
                _player = Instantiate(playerPrefabs[data.charId]).transform;
                _playerAnimator = _player.GetComponent<Animator>();
                _currentCharId = data.charId;
                _player.position = pPosition;
                _player.rotation = pRotation;
                Destroy(playerOld.gameObject);
            }
            else
            {
                var playerOld = _others[data.id].transform;
                var pPosition = playerOld.position;
                var pRotation = playerOld.rotation;
                _others[data.id] = Instantiate(otherPlayerPrefabs[data.charId], transform)
                    .GetComponent<OtherPlayerController>();
                _others[data.id].UpdateTransform(pPosition, pRotation.eulerAngles);
                _others[data.id].SetOriginalTransform(pPosition, pRotation);
                Destroy(playerOld.gameObject);
            }
        }

        #endregion
    }
}

