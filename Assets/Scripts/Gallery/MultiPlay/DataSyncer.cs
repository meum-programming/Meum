using System;
using System.Collections;
using System.Collections.Generic;
using Gallery.Builder;
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
        private float _sendIntervalCounter = 0.0f;

        [Header("Require Prefabs")] 
        [SerializeField] private List<GameObject> playerPrefabs;
        [SerializeField] private List<GameObject> otherPlayerPrefabs;

        private Transform _player = null;
        private Animator _playerAnimator = null;
        public int currentCharId { get; private set; }

        private OtherPlayerController[] _others = null;

        private SocketIOController _socket;
        private int _id = -1;

        private static DataSyncer _globalInstance = null;
        public static DataSyncer Get()
        {
            return _globalInstance;
        }
        
        #region Unity Functions
        
        private void Awake()
        {
            if (_globalInstance == null)
            {
                var selfTransform = transform;
                selfTransform.position = Vector3.zero;
                selfTransform.rotation = Quaternion.identity;
                selfTransform.localScale = new Vector3(1, 1, 1);
                
                DontDestroyOnLoad(gameObject);
                _globalInstance = this;
            }
            else
                Destroy(gameObject);
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
        
        #endregion

        #region Other Functions

        public bool IsInRoomOrSquare()
        {
            return _id != -1;
        }

        public void TemporaryLeaveRoom()
        {
            for (var i = 0; i < _others.Length; ++i)
            {
                if(_others[i] != null)
                    _others[i].SetRendererEnabled(false);
            }
            _player.gameObject.SetActive(false);
        }

        public void ReturnToRoom()
        {
            for (var i = 0; i < _others.Length; ++i)
            {
                if(_others[i] != null)
                    _others[i].SetRendererEnabled(true);
            }
            _player.gameObject.SetActive(true);
        }

        public void Reset()
        {
            for (var i = 0; i < _others.Length; ++i)
            {
                if(_others[i] != null)
                    Destroy(_others[i].gameObject);
            }
            Destroy(_player.gameObject);

            _others = null;
            _player = null;
            _playerAnimator = null;

            _id = -1;
            currentCharId = -1;
        }

        public void SetupRoom(int playerID, int maxN, SocketIOController socket)
        {
            // setting other players
            _id = playerID;
            _others = new OtherPlayerController[maxN];

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
            
            // setting player
            _player = Instantiate(playerPrefabs[0], transform).transform;
            _playerAnimator = _player.GetComponent<Animator>();
            currentCharId = 0;
            _player.position = spawnTransform.position;
            _player.rotation = spawnTransform.rotation;
            
            // setting socket.io
            _socket = socket;
            _socket.On("userQuit", OnUserQuit);
            _socket.On("userInfo", OnUserInfo);
            _socket.On("animTrigger", OnAnimTrigger);
            _socket.On("changeCharacter", OnChangeCharacter);
            _socket.On("updateArtworks", OnUpdateArtworks);
        }
        
        #endregion

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

        public void BroadCastChangeCharacter(int charId)
        {
            ChangeCharacterData data;
            data.charId = charId;
            _socket.Emit("broadCastChangeCharacter", JsonConvert.SerializeObject(data));
        }

        public void BroadCastUpdateArtworks()
        {
            _socket.Emit("broadcastUpdateArtworks");
        }

        #endregion

        #region Socket Event Handlers

        private struct UserQuitEventData
        {
            public int id;
        }

        private void OnUserQuit(SocketIOEvent e)
        {
            var data = JsonConvert.DeserializeObject<UserQuitEventData>(e.data);
            if (!IsInRoomOrSquare())
            { 
                MeumDB.Get().ClearTextureBuffer();
                SceneManager.LoadScene("Lobby");
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
            if (!IsInRoomOrSquare()) return;
            var data = JsonConvert.DeserializeObject<UserInfoEventData>(e.data);
            var obj = _others[data.id];
            if (data.id == _id || obj == null) return;
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
                if (currentCharId == data.charId) return;
                var playerOld = _player;
                var pPosition = playerOld.position;
                var pRotation = playerOld.rotation;
                _player = Instantiate(playerPrefabs[data.charId]).transform;
                _playerAnimator = _player.GetComponent<Animator>();
                currentCharId = data.charId;
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

        private void OnUpdateArtworks(SocketIOEvent e)
        {
            // if _player deactivated then player is in builder scene
            if(_player.gameObject.activeSelf)        
                MeumSocket.Get().SerializeArtworks();
        }

        #endregion
    }
}

