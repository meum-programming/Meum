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
                Debug.Log("Lobby");
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

        private struct CreateRoomEventData
        {
            public int roomType;
            public int roomId;
            public int maxN;
        }

        private void Enter()
        {
            // TODO : implement MeumDB and use DB to enter room
            StartCoroutine(EnterCoroutine());


            // EnteringRoomEventData data;
            // data.roomId = Convert.ToInt32(nickname.text);
            // _socket.Emit("enteringRoom", JsonConvert.SerializeObject(data));
        }

        private IEnumerator EnterCoroutine()
        {
            var cd = new CoroutineWithData(this, MeumDB.Get().GetUserInfo(nickname.text));
            yield return cd.coroutine;
            var userInfo = cd.result as MeumDB.UserInfo;
            if (null != userInfo)
            {
                cd = new CoroutineWithData(this, MeumDB.Get().GetRoomInfo(userInfo.primaryKey));
                yield return cd.coroutine;
                var roomInfo = cd.result as MeumDB.RoomInfo;
                if (null != roomInfo)
                {
                    Debug.Log(roomInfo.type_int);
                    Debug.Log(roomInfo.primaryKey);
                    Debug.Log(roomInfo.data_json);
                }
            }
        }

        public void Quit()
        {
            _socket.Emit("quitRoom");
        }
    }
}
