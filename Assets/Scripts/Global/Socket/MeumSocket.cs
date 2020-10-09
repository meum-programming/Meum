using System;
using System.Collections;
using UnityEngine;
using UnitySocketIO;
using Newtonsoft.Json;
using UnitySocketIO.Events;

namespace Global.Socket
{
    public class MeumSocket : MonoBehaviour
    {
        
        [Header("Connection Info")] [SerializeField] private float sendInterval;
        private float _sendIntervalCounter = 0.0f;
    
        // [System.Serializable]
        // public struct GallerySceneInfo
        // {
        //     public string gallery;
        //     public string builder;
        // }
        // [Header("Gallery Scenes")] [SerializeField] private GallerySceneInfo[] sceneInfos;
        

        private SocketIOController _socket;
        private SceneState _state;
        private SceneLoader _loader;
        private SocketEventHandler _eventHandler;

        public MeumDB.UserInfo PlayerInfo { get; private set; } = null;

        // public string GetGallerySceneName(int type)
        // {
        //     return sceneInfos[type].gallery;
        // }
        //
        // public string GetBuilderSceneName(int type)
        // {
        //     return sceneInfos[type].builder;
        // }

        private void Init()
        {
            _socket = GetComponent<SocketIOController>();
            _state = new SceneState();
            _loader = new SceneLoader(this, _state);
            _eventHandler = new SocketEventHandler(_socket, _state, _loader);
        }

        #region Singleton
        private static MeumSocket _globalInstance = null;
        public static MeumSocket Get()
        {
            return _globalInstance;
        }

        private void Awake()
        {
            if (_globalInstance == null)
            {
                Init();
                DontDestroyOnLoad(gameObject);
                _globalInstance = this;
            }
            else
                Destroy(gameObject);
        }
        #endregion
        
        private void Start()
        {
            _socket.Connect();
            _eventHandler.AssignHandler();
        }

        private void Update()
        {
            if (!_state.IsNotInGalleryOrSquare())
            {
                _sendIntervalCounter += Time.deltaTime;
                if (_sendIntervalCounter > sendInterval)
                {
                    var userInfoData = DataSyncer.Get().GetPlayerInfoData();
                    _socket.Emit("broadCastUserInfo", JsonConvert.SerializeObject(userInfoData));
                    _sendIntervalCounter = 0.0f;
                }
            }
        }

        private IEnumerator UpdatePlayerInfo()
        {
            var cd = new CoroutineWithData(this, MeumDB.Get().GetUserInfo());
            yield return cd.coroutine;
            PlayerInfo = cd.result as MeumDB.UserInfo;
        }

        public void LeaveToShop()
        {
            _loader.LeaveToShop();
        }

        public void LeaveToEdit()
        {
            _loader.LeaveToEdit();
        }

        public void Return()
        {
            _loader.Return();
        }

        #region Socket Emitters
        public void EnterGallery(string nickname)
        {
            if (!_state.IsNotInGalleryOrSquare())
            {
                Debug.LogError("cannot go gallery When your in room or square");
                Application.Quit(-1);
            }

            StartCoroutine(EnterGalleryCoroutine(nickname));
        }
        private IEnumerator EnterGalleryCoroutine(string nickname)
        {
            var cd = new CoroutineWithData(this, MeumDB.Get().GetUserInfo(nickname));
            yield return cd.coroutine;
            var userInfo = cd.result as MeumDB.UserInfo;
            if (null != userInfo)
            {
                cd = new CoroutineWithData(this, MeumDB.Get().GetRoomInfoWithUser(userInfo.primaryKey));
                yield return cd.coroutine;
                var roomInfo = cd.result as MeumDB.RoomInfo;
                if (null != roomInfo)
                {
                    StartCoroutine(UpdatePlayerInfo());
                    EnteringGalleryData data;
                    data.roomId = roomInfo.primaryKey;
                    data.roomType = roomInfo.type_int;
                    data.maxN = roomInfo.max_people;
                    _socket.Emit("enteringGallery", JsonConvert.SerializeObject(data));
                }
            }
        }

        public void EnterSquare()
        {
            if (!_state.IsNotInGalleryOrSquare())
            {
                Debug.LogError("cannot go square When your in room or square");
                Application.Quit(-1);
            }

            StartCoroutine(EnterSquareCoroutine());
        }

        private IEnumerator EnterSquareCoroutine()
        {
            StartCoroutine(UpdatePlayerInfo());
            yield return null;
            _socket.Emit("enteringSquare");
        }

        public void Quit()
        {
            _socket.Emit("quitRoom");
            _state.Quit();
        }
        
        public void BroadCastAnimTrigger(string paramName)
        {
            BroadCastAnimTriggerData data;
            data.name = paramName;
            _socket.Emit("broadCastAnimTrigger", JsonConvert.SerializeObject(data));
        }

        public void BroadCastAnimBoolChange(string paramName, bool value)
        {
            BroadCastAnimBoolChangeData data;
            data.name = paramName;
            data.value = value;
            _socket.Emit("broadCastAnimBoolChange", JsonConvert.SerializeObject(data));
        }

        public void BroadCastAnimFloatChange(string paramName, float value)
        {
            BroadCastAnimFloatChangeData data;
            data.name = paramName;
            data.value = value;
            _socket.Emit("broadCastAnimFloatChange", JsonConvert.SerializeObject(data));
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

        public void BroadCastChatting(string message)
        {
            BroadCastChattingData data;
            data.message = message;
            _socket.Emit("broadCastChatting", JsonConvert.SerializeObject(data));
        }
        
        private void OnApplicationQuit()
        {
            _socket.Emit("disconnect");
        }
        #endregion
        
        #region Socket Emitter structs
        private struct EnteringGalleryData
        {
            public int roomId;
            public int roomType;
            public int maxN;
        }

        public struct BroadCastUserInfoData
        {
            public Vector3 position;
            public Vector3 rotation;
            public int userKey;
            public string nickname;
        }
        
        private struct BroadCastAnimTriggerData
        {
            public string name;
        }
        
        private struct BroadCastAnimBoolChangeData
        {
            public string name;
            public bool value;
        }
        
        private struct BroadCastAnimFloatChangeData
        {
            public string name;
            public float value;
        }
        
        private struct ChangeCharacterData
        {
            public int charId;
        }

        private struct BroadCastChattingData
        {
            public string message;
        }
        #endregion
    }
}
