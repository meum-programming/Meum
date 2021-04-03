using System;
using System.Collections;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine.Assertions;
using UnityEngine.Networking;
using System.Collections.Generic;

namespace Core
{
    /*
     * @brief DB API 호출을 담당하는 컴포넌트, Singleton임
     */
    public class MeumDB : Singleton<MeumDB>
    {
        private TextureBuffer _textureBuffer = new TextureBuffer();
        private Object3DBuffer _object3DBuffer = new Object3DBuffer();
        private string _token = "";                                           // API 서버의 authorization token
        private string BASE_URL = "https://meum.me/nodeTest";                // API 서버의 BASE URL

        public RoomInfoData currentRoomInfo = null;
        public RoomInfoData myRoomInfo = null;

        private void Awake()
        {
#if dev
            BASE_URL = "https://dev.meum.me/nodeTest/";
#endif

            base.Awake();
            DontDestroyOnLoad(gameObject);
        }
        
        /*
         * @brief Authorization token 의 Set 함수
         * @details Web 환경에서 실행될 때 웹상의 로그인 정보를 전달받기위해 사용
         */
        public void SetToken(string token)
        {
            _token = token;
        }

        public int GetToken()
        {
            return int.Parse(_token);
        }

        /*
         * @brief 토큰이 존재한다면 true 반환
         */
        public bool TokenExist()
        {
            return _token != "";
        }
        
        /*
         * Texture Buffer 함수
         */
        public CoroutineWithData GetTextureCoroutine(string url)
        {
            return _textureBuffer.Get(this, url);
        }

        public Texture2D GetTexture(string url)
        {
            return _textureBuffer.Get(url);
        }

        public void ClearTextureBuffer()
        {
            _textureBuffer.Clear();
        }
        
        /*
         * 3D Object Buffer 함수
         */
        public CoroutineWithData GetObject3DCoroutine(string url)
        {
            return _object3DBuffer.Get(this, url);
        }

        public GameObject GetObject3D(string url)
        {
            return _object3DBuffer.Get(url);
        }

        public void ClearObject3DBuffer()
        {
            _object3DBuffer.Clear();
        }
        
        #region API functions 
        
        public IEnumerator Login(string email, string pwd)
        {
            LoginData loginData;
            loginData.email = email;
            loginData.password = pwd;

            var json = JsonConvert.SerializeObject(loginData);
            var cd = new CoroutineWithData(this, WebRequest(BASE_URL + "/login", "POST", json));
            yield return cd.coroutine;
            Assert.IsNotNull(cd.result);
            var data = cd.result as string;
            Assert.IsNotNull(data);

            var obj = JObject.Parse(data);
            Assert.IsNotNull(obj);
            if (ReferenceEquals(obj["token"], null))
                yield return false;
            else
            {
                _token = obj["token"].Value<string>();
                yield return true;
            }
        }

        public void Logout()
        {
            StartCoroutine(WebRequest(BASE_URL + "/logout", "POST"));
        }

        public IEnumerator GetUserInfo2(string nickname)
        {
            WWWForm form = new WWWForm();
            form.AddField("nickname", nickname);

            var cd = new CoroutineWithData(this, WebReques2(BASE_URL, "profileByNickname", form));
            yield return cd.coroutine;

            var data = cd.result as string;
            Assert.IsNotNull(data);
            UserInfoRespons resultData = GetData<UserInfoRespons>(data);
            yield return resultData.result;

        }

        public T GetData<T>(string jsonData)
        {
            T data = JsonUtility.FromJson<T>(jsonData);

            return data;
        }

        public IEnumerator GetUserInfo2()
        {
            WWWForm form = new WWWForm();
            form.AddField("uid", _token);

            var cd = new CoroutineWithData(this, WebReques2(BASE_URL, "user", form));
            yield return cd.coroutine;

            var data = cd.result as string;
            Assert.IsNotNull(data);
            UserInfoRespons resultData = GetData<UserInfoRespons>(data);
            yield return resultData.result;

        }

        public IEnumerator GetRoomInfoWithUser2(int uid)
        {

            WWWForm form = new WWWForm();
            form.AddField("uid", uid);

            var cd = new CoroutineWithData(this, WebReques2(BASE_URL, "roomOwner", form));
            yield return cd.coroutine;

            RoomInfoRespons resultData = GetData<RoomInfoRespons>(cd.result as string);
            yield return resultData.result;
        }

        public IEnumerator GetRoomInfo2(int roomId)
        {
            WWWForm form = new WWWForm();
            form.AddField("roomId", roomId);

            var cd = new CoroutineWithData(this, WebReques2(BASE_URL, "roomById", form));
            yield return cd.coroutine;

            RoomInfoRespons resultData = GetData<RoomInfoRespons>(cd.result as string);
            yield return resultData.result;

        }


        public IEnumerator GetIsTutorial()
        {
            var url = BASE_URL + "/tutorial";
            var cd = new CoroutineWithData(this, WebRequest(url, "GET"));
            yield return cd.coroutine;
            Assert.IsNotNull(cd.result);
            var data = cd.result as string;
            Assert.IsNotNull(data);
            
            var obj = JObject.Parse(data);

            var val = obj["is_tutorial"].Value<bool>();
            yield return val;
        }
        
        public IEnumerator PatchTutorialClear()
        {
            PatchTutorialClearJsonData data;
            data.is_tutorial = false;

            var json = JsonConvert.SerializeObject(data);
            var url = BASE_URL + "/tutorial";
            var cd = new CoroutineWithData(this, WebRequest(url, "PATCH", json));
            yield return cd.coroutine;
        }

        #endregion

        
        #region API json struct
        
        private struct LoginData
        {
            public string email;
            public string password;
        }
        
        private struct PatchTutorialClearJsonData
        {
            public bool is_tutorial;
        }

        #endregion
        
        /*
         * @breif API 호출을 위한 WebRequest를 보내는 함수 CoroutineWithData를 통해 실행해야함
         */
        private IEnumerator WebRequest(string url, string method, string json = "")
        {
            var uwr = new UnityWebRequest(url, method);
            if (json != "")
            {
                var jsonToSend = new System.Text.UTF8Encoding().GetBytes(json);
                uwr.uploadHandler = new UploadHandlerRaw(jsonToSend);
            }

            uwr.downloadHandler = new DownloadHandlerBuffer();
            uwr.SetRequestHeader("Content-Type", "application/json");
            if (TokenExist())
                uwr.SetRequestHeader("Authorization", "JWT " + _token);

            yield return uwr.SendWebRequest();

            if (uwr.isNetworkError)
            {
                Debug.LogError(url + ": Error: " + uwr.error);
                yield return null;
            }
            else if (uwr.isHttpError)
            {
                Debug.LogError(url + ": Error: " + uwr.responseCode);
                yield return null;
            }
            else
            {
                yield return uwr.downloadHandler.text;
            }
        }

        private IEnumerator WebReques2(string url, string method, WWWForm form)
        {
            var uwr = UnityWebRequest.Post(url+"/"+ method, form);
            uwr.downloadHandler = new DownloadHandlerBuffer();
            
            yield return uwr.SendWebRequest();

            if (uwr.isNetworkError)
            {
                Debug.LogError(url + ": Error: " + uwr.error);
                yield return null;
            }
            else if (uwr.isHttpError)
            {
                Debug.LogError(url + ": Error: " + uwr.responseCode);
                yield return null;
            }
            else
            {
                yield return uwr.downloadHandler.text;
            }
        }
    }
}