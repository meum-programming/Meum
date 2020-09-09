using System;
using System.Collections;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnitySocketIO;

namespace Gallery.MultiPlay
{
    public class ServerSelector : MonoBehaviour
    {
        private InputField nickname;
        private SocketIOController _socket;

        private static ServerSelector _globalInstance = null;

        private void Awake()
        {
            if (_globalInstance == null)
            {
                _socket = GetComponent<SocketIOController>();
                DontDestroyOnLoad(gameObject);
                _globalInstance = this;
                SceneManager.sceneLoaded += Load;
            }
            else
                Destroy(gameObject);
        }

        private void Load(Scene scene, LoadSceneMode mode)
        {
            if (scene.name == "Lobby")
            {
                nickname = GameObject.Find("InputField_Nickname").GetComponent<InputField>();
                GameObject.Find("Enter").GetComponent<Button>().onClick.AddListener(Enter);
            }
        }

        private struct EnteringRoomEventData
        {
            public int roomType;
            public int roomId;
            public int maxN;
        }
        private void Enter()
        {
            // TODO : implement MeumDB and use DB to enter room
            StartCoroutine(EnterCoroutine());
        }
        private IEnumerator EnterCoroutine()
        {
            var cd = new CoroutineWithData(this, MeumDB.Get().GetUserInfo(nickname.text));
            yield return cd.coroutine;
            var userInfo = cd.result as MeumDB.UserInfo;
            if (null != userInfo)
            {
                cd = new CoroutineWithData(this, MeumDB.Get().GetRoomInfoWithUser(userInfo.primaryKey));
                yield return cd.coroutine;
                var roomInfo = cd.result as MeumDB.RoomInfo;
                if (null != roomInfo)
                {
                    EnteringRoomEventData data;
                    data.roomType = roomInfo.type_int;
                    data.roomId = roomInfo.primaryKey;
                    data.maxN = roomInfo.max_people;
                    _socket.Emit("enteringRoom", JsonConvert.SerializeObject(data));
                }
            }
        }

        public void Quit()
        {
            _socket.Emit("quitRoom");
        }
    }
}
