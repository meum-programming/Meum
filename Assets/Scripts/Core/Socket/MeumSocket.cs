using System.Collections;
using UnityEngine;
using UnitySocketIO;
using Newtonsoft.Json;
using UnityEngine.Assertions;
using UnityEngine.SocialPlatforms;

namespace Core.Socket
{
    /*
     * @brief Socket.io 서버와의 통신을 담당
     * @details 소켓.io 이벤트를 받고 보내는 기능을 수행
     * Gallery -> Edit, Edit -> Gallery Scene이동도 이 컴포넌트가 수행 (socket.io 이벤트가 동반됨)
     */
    public class MeumSocket : Singleton<MeumSocket>
    {
        [Header("Connection Info")] 
        [SerializeField] private float sendInterval;

        /*
         * @brief LocalPlayer의 UserInfo(DB)를 담고있음
         */
        public UserData LocalPlayerInfo { get; private set; } = null;

        private float _sendIntervalCounter = 0.0f;
        private SocketIOController _socket;
        private SceneState _state;
        private SceneLoader _loader;
        private SocketEventHandler _eventHandler;

        private void Init()
        {
            _socket = GetComponent<SocketIOController>();
            _state = new SceneState();
            _loader = new SceneLoader(this, _state);
            _eventHandler = new SocketEventHandler(_socket, _state, _loader);
        }

        private void Awake()
        {
            base.Awake();
            DontDestroyOnLoad(gameObject);
            Init();
        }
        
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
                    var userInfoData = DataSynchronizer.Get().GetPlayerInfoData();
                    _socket.Emit("broadCastUserInfo", JsonConvert.SerializeObject(userInfoData));
                    _sendIntervalCounter = 0.0f;
                }
            }
        }
        
        /*
         * LocalPlayer의 Socket.io 서버 방에서의 id를 반환, UserInfo(DB)의 primaryKey와는 다름
         */
        public int GetPlayerId()
        {
            return _eventHandler.GetLocalPlayerId();
        }
        
        /*
         * LocalPlayer의 UserInfo(DB)의 primaryKey를 반환
         */
        public int GetPlayerPk()
        {
            return _loader.GetPlayerPk();
        }
        
        public int GetRoomId()
        {
            return _loader.GetRoomId();
        }

        public bool IsInRoomOwn()
        {
            return _state.IsSubOfGalleryOwn();
        }

        public void GoToChaEditScene()
        {
            _loader.LoadChaEditScene();
        }

        public void GoToEditScene()
        {
            //Assert.IsTrue(_state.IsSubOfGalleryOwn());
            //if (!_state.IsSubOfGalleryOwn())
              //  return;
            _loader.LoadEditScene();
        }

        public void ReturnToGalleryScene()
        {
            _loader.Return();
        }

        /*
         * @brief LocalPlayer의 UserInfo 를 불러옴
         */
        private IEnumerator LoadLocalPlayerInfo2()
        {
            if (!ReferenceEquals(LocalPlayerInfo, null)) yield break;

            var cd = new CoroutineWithData(this, MeumDB.Get().GetUserInfo2());
            yield return cd.coroutine;

            UserData userData = cd.result as UserData;
            LocalPlayerInfo = userData;
        }

        /*
         * 이벤트를 Emit하는 함수들 
         */
        #region Socket Emitters
        public void EnterGallery(string nickname)
        {
            StartCoroutine(EnterGalleryCoroutine2(nickname));
        }

        /// <summary>
        /// 룸 ID로 정보 로딩
        /// </summary>
        /// <param name="roomId"></param>
        public void EnterGallery(int roomId)
        {
            StartCoroutine(EnterGalleryCoroutine(roomId));
        }

        private IEnumerator EnterGalleryCoroutine(int roomId)
        {
            bool nextOn = false;

            RoomRequest roomRequest = new RoomRequest()
            {
                requestStatus = 0,
                id = roomId,
                successOn = ResultData =>
                {
                    RoomInfoRespons data = (RoomInfoRespons)ResultData;
                    nextOn = true;
                    RoomDataSet(data.result);
                }
            };
            roomRequest.RequestOn();

            yield return new WaitUntil(() => nextOn);
        }

        private IEnumerator EnterGalleryCoroutine2(string nickname)
        {
            bool nextOn = false;

            UserData userInfo = null;

            UserInfoRequest userInfoRequest = new UserInfoRequest()
            {
                requestStatus = 1,
                nickName = nickname,
                successOn = ResultData =>
                {
                    UserInfoRespons data = (UserInfoRespons)ResultData;
                    userInfo = data.result;
                    nextOn = true;
                }
            };
            userInfoRequest.RequestOn();

            yield return new WaitUntil(() => nextOn);

            if (userInfo.user_id == 0)
            {
                yield break;
            }

            if (null != userInfo)
            {
                RoomRequest roomRequest = new RoomRequest()
                {
                    requestStatus = 1,
                    uid = userInfo.user_id,
                    successOn = ResultData =>
                    {
                        RoomInfoRespons data = (RoomInfoRespons)ResultData;
                        nextOn = true;
                        RoomDataSet(data.result);
                    }
                };
                roomRequest.RequestOn();

                yield return new WaitUntil(() => nextOn);
            }
        }

        void RoomDataSet(RoomInfoData roomInfo)
        {
            if (null != roomInfo)
            {
                MeumDB.Get().currentRoomInfo = roomInfo;

                StartCoroutine(LoadLocalPlayerInfo2());
                EnteringGalleryData data;
                data.roomId = roomInfo.id;
                data.roomType = roomInfo.type_int;
                data.maxN = roomInfo.max_people;
                _socket.Emit("enteringGallery", JsonConvert.SerializeObject(data));
            }
        }

        public void EnterSquare()
        {
            Assert.IsTrue(_state.IsNotInGalleryOrSquare());
            StartCoroutine(EnterSquareCoroutine());
        }

        private IEnumerator EnterSquareCoroutine()
        {
            StartCoroutine(LoadLocalPlayerInfo2());
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

        public void BroadCastAnimGesture(string paramName)
        {
            BroadCastAnimTriggerData data;
            data.name = paramName;
            _socket.Emit("broadCastAnimGesture", JsonConvert.SerializeObject(data));
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

        public void BroadCastChatting(int type, int target, string message)
        {
            BroadCastChattingData data;
            data.type = type;
            data.target = target;
            data.message = message;
            _socket.Emit("broadCastChatting", JsonConvert.SerializeObject(data));
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

            public int hairIndex;
            public int maskIndex;
            public int dressIndex;
            public int skinIndex;
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
            public int type;
            public int target;
            public string message;
        }
        #endregion
    }
}
