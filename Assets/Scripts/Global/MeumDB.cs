using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine.Assertions;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace Global
{
    public class MeumDB : MonoBehaviour
    {
        private TextureBuffer _textureBuffer = new TextureBuffer();
        private Object3DBuffer _object3DBuffer = new Object3DBuffer();

        private string _token = "";

        private static MeumDB _globalInstance;

        private const string BASE_URL = "https://api.meum.me";

        public void SetToken(string token)
        {
            _token = token;
        }

        public bool TokenExist()
        {
            return _token != "";
        }

        private void Awake()
        {
            if (_globalInstance == null)
            {
                DontDestroyOnLoad(gameObject);
                _globalInstance = this;
            }
            else
                Destroy(gameObject);
        }

        public static MeumDB Get()
        {
            return _globalInstance;
        }

        [System.Serializable]
        private struct LoginData
        {
            public string email;
            public string password;
        }

        public IEnumerator Login(string email, string pwd)
        {
            LoginData loginData;
            loginData.email = email;
            loginData.password = pwd;

            var json = JsonConvert.SerializeObject(loginData);
            var cd = new CoroutineWithData(this, WebRequest(BASE_URL + "/login", "POST", json));
            yield return cd.coroutine;
            if (cd.result == null)
                yield break;
            var data = cd.result as string;
            var obj = JObject.Parse(data);
            if (obj["token"] == null)
                yield return false;
            else
            {
                _token = obj["token"].Value<string>();
                Debug.Log(_token);
                yield return true;
            }
        }

        public void Logout()
        {
            StartCoroutine(WebRequest(BASE_URL + "/logout", "POST"));
        }

        public class UserInfo
        {
            public int primaryKey;
            public string email;
            public string nickname;
            public string phone;
        }

        public IEnumerator GetUserInfo(string nickname)
        {
            var url = BASE_URL + "/profile/nickname/" + nickname;
            var cd = new CoroutineWithData(this, WebRequest(url, "GET"));
            yield return cd.coroutine;
            if (cd.result == null)
                yield break;
            var data = cd.result as string;

            var output = new UserInfo();
            var jarray = JArray.Parse(data);
            if (jarray.Count == 0)
            {
                Debug.LogError("user not exist, nickname: " + nickname);
                yield return null;
            }
            else
            {
                var json = jarray[0];
                output.primaryKey = json["user"]["pk"].Value<int>();
                output.email = json["user"]["email"].Value<string>();
                output.nickname = json["user"]["profile"]["nickname"].Value<string>();
                output.phone = json["user"]["profile"]["phone"].Value<string>();
                yield return output;
            }
        }

        public IEnumerator GetUserInfo()
        {
            var url = BASE_URL + "/user";
            var cd = new CoroutineWithData(this, WebRequest(url, "GET"));
            yield return cd.coroutine;
            if (cd.result == null)
                yield break;
            var data = cd.result as string;

            var output = new UserInfo();
            var json = JObject.Parse(data);
            output.primaryKey = json["pk"].Value<int>();
            output.email = json["email"].Value<string>();
            output.nickname = json["profile"]["nickname"].Value<string>();
            output.phone = json["profile"]["phone"].Value<string>();
            yield return output;
        }

        public class RoomInfo
        {
            public int primaryKey;
            public int max_people;
            public int type_int;
            public string data_json;
            public UserInfo owner;
        }

        public IEnumerator GetRoomInfoWithUser(int userPK)
        {
            var url = BASE_URL + "/room/owner/" + userPK;
            var cd = new CoroutineWithData(this, WebRequest(url, "GET"));
            yield return cd.coroutine;
            if (cd.result == null)
                yield break;
            var data = cd.result as string;

            var output = new RoomInfo();
            var jarray = JArray.Parse(data);
            if (jarray.Count == 0)
            {
                Debug.Log("room not exist, userPK: " + userPK);
                yield return null;
            }
            else
            {
                var json = jarray[0];
                output.primaryKey = json["pk"].Value<int>();
                output.max_people = json["max_people"].Value<int>();
                output.type_int = json["type_int"].Value<int>();
                output.data_json = json["data_json"].Value<string>();
                
                var ownerObj = json["owner"];
                output.owner = new UserInfo();
                output.owner.primaryKey = ownerObj["pk"].Value<int>();
                output.owner.email = ownerObj["email"].Value<string>();
                output.owner.nickname = ownerObj["profile"]["nickname"].Value<string>();
                output.owner.phone = ownerObj["profile"]["phone"].Value<string>();
                yield return output;
            }
        }

        public IEnumerator GetRoomInfo(int roomPK)
        {
            var url = BASE_URL + "/room/" + roomPK;
            var cd = new CoroutineWithData(this, WebRequest(url, "GET"));
            yield return cd.coroutine;
            if (cd.result == null)
                yield break;
            var data = cd.result as string;

            var output = new RoomInfo();
            var json = JObject.Parse(data);
            output.primaryKey = json["pk"].Value<int>();
            output.max_people = json["max_people"].Value<int>();
            output.type_int = json["type_int"].Value<int>();
            output.data_json = json["data_json"].Value<string>();
            
            var ownerObj = json["owner"];
            output.owner = new UserInfo();
            output.owner.primaryKey = ownerObj["pk"].Value<int>();
            output.owner.email = ownerObj["email"].Value<string>();
            output.owner.nickname = ownerObj["profile"]["nickname"].Value<string>();
            output.owner.phone = ownerObj["profile"]["phone"].Value<string>();
            yield return output;
        }

        [Serializable]
        private struct PatchRoomJsonData
        {
            public string data_json;
        }

        public IEnumerator PatchRoomJson(string jsonData)
        {
            PatchRoomJsonData data;
            data.data_json = jsonData;

            var json = JsonConvert.SerializeObject(data);
            var url = BASE_URL + "/room/owner";
            var cd = new CoroutineWithData(this, WebRequest(url, "PATCH", json));
            yield return cd.coroutine;
            // var response = cd.result as string;
        }

        public class ArtworkInfo
        {
            public int primaryKey;
            public string author;
            public string title;
            public float size_w;
            public float size_h;
            public int year;
            public string material;
            public string object_file;
            public string image_file;
            public string thumbnail;
            public string instruction;
            public int like;
            public int hate;
            public int type_artwork;
        }

        public IEnumerator GetArtworks()
        {
            var url = BASE_URL + "/artwork";
            var cd = new CoroutineWithData(this, WebRequest(url, "GET"));
            yield return cd.coroutine;
            if (ReferenceEquals(cd.result, null))
                yield break;
            var data = cd.result as string;

            var jarray = JArray.Parse(data);
            var output = new ArtworkInfo[jarray.Count];
            for (var i = 0; i < jarray.Count; ++i)
            {
                var obj = jarray[i];
                output[i] = new ArtworkInfo();
                output[i].primaryKey = obj["pk"].Value<int>();
                output[i].author = obj["owner"]["profile"]["nickname"].Value<string>();
                output[i].title = obj["title"].Value<string>();
                if(obj["size_w"].Type != JTokenType.Null)
                    output[i].size_w = obj["size_w"].Value<int>() / 100.0f;
                if(obj["size_h"].Type != JTokenType.Null)
                    output[i].size_h = obj["size_h"].Value<int>() / 100.0f;
                if(obj["year"].Type != JTokenType.Null)
                    output[i].year = obj["year"].Value<int>();
                if(obj["material"].Type != JTokenType.Null)
                    output[i].material = obj["material"].Value<string>();
                if(obj["object_file"].Type != JTokenType.Null)
                    output[i].object_file = BASE_URL + obj["object_file"].Value<string>();
                if (obj["image_file"].Type != JTokenType.Null)
                    output[i].image_file = BASE_URL + obj["image_file"].Value<string>();
                if (obj["thumbnail"].Type != JTokenType.Null)
                    output[i].thumbnail = BASE_URL + obj["thumbnail"].Value<string>();
                if(obj["instruction"].Type != JTokenType.Null)
                    output[i].instruction = obj["instruction"].Value<string>();
                output[i].like = obj["like"].Value<int>();
                output[i].hate = obj["hate"].Value<int>();
                output[i].type_artwork = obj["type_artwork"].Value<int>();
            }

            yield return output;
        }

        public IEnumerator GetArtwork(int primaryKey)
        {
            var url = BASE_URL + "/artwork/" + primaryKey;
            var cd = new CoroutineWithData(this, WebRequest(url, "GET"));
            yield return cd.coroutine;
            if (ReferenceEquals(cd.result, null))
                yield break;
            var data = cd.result as string;

            var json = JObject.Parse(data);
            var output = new ArtworkInfo();
            output.primaryKey = json["pk"].Value<int>();
            output.author = json["owner"]["profile"]["nickname"].Value<string>();
            output.title = json["title"].Value<string>();
            if(json["size_w"].Type != JTokenType.Null)
                output.size_w = json["size_w"].Value<int>() / 100.0f;
            if(json["size_h"].Type != JTokenType.Null)
                output.size_h = json["size_h"].Value<int>() / 100.0f;
            if(json["year"].Type != JTokenType.Null)
                output.year = json["year"].Value<int>();
            if(json["material"].Type != JTokenType.Null)
                output.material = json["material"].Value<string>();
            if(json["object_file"].Type != JTokenType.Null)
                output.object_file = BASE_URL + json["object_file"].Value<string>();
            if (json["image_file"].Type != JTokenType.Null)
                output.image_file = BASE_URL + json["image_file"].Value<string>();
            if (json["thumbnail"].Type != JTokenType.Null)
                output.thumbnail = BASE_URL + json["thumbnail"].Value<string>();
            if(json["instruction"].Type != JTokenType.Null)
                output.instruction = json["instruction"].Value<string>();
            output.like = json["like"].Value<int>();
            output.hate = json["hate"].Value<int>();
            output.type_artwork = json["type_artwork"].Value<int>();

            yield return output;
        }

        public IEnumerator GetPurchasedArtworks()
        {
            var url = BASE_URL + "/shop/buy";
            var cd = new CoroutineWithData(this, WebRequest(url, "GET"));
            yield return cd.coroutine;
            Assert.IsNotNull(cd.result);
            var data = cd.result as string;
            
            var jarray = JArray.Parse(data);
            Assert.IsNotNull(jarray);
            var output = new ArtworkInfo[jarray.Count];
            for (var i = 0; i < jarray.Count; ++i)
            {
                var obj = jarray[i]["artwork"];
                output[i] = new ArtworkInfo();
                output[i].primaryKey = obj["pk"].Value<int>();
                output[i].author = obj["owner"]["profile"]["nickname"].Value<string>();
                output[i].title = obj["title"].Value<string>();
                if(obj["size_w"].Type != JTokenType.Null)
                    output[i].size_w = obj["size_w"].Value<int>() / 100.0f;
                if(obj["size_h"].Type != JTokenType.Null)
                    output[i].size_h = obj["size_h"].Value<int>() / 100.0f;
                if(obj["year"].Type != JTokenType.Null)
                    output[i].year = obj["year"].Value<int>();
                if(obj["material"].Type != JTokenType.Null)
                    output[i].material = obj["material"].Value<string>();
                if(obj["object_file"].Type != JTokenType.Null)
                    output[i].object_file = BASE_URL + obj["object_file"].Value<string>();
                if (obj["image_file"].Type != JTokenType.Null)
                    output[i].image_file = BASE_URL + obj["image_file"].Value<string>();
                if (obj["thumbnail"].Type != JTokenType.Null)
                    output[i].thumbnail = BASE_URL + obj["thumbnail"].Value<string>();
                if(obj["instruction"].Type != JTokenType.Null)
                    output[i].instruction = obj["instruction"].Value<string>();
                output[i].like = obj["like"].Value<int>();
                output[i].hate = obj["hate"].Value<int>();
                output[i].type_artwork = obj["type_artwork"].Value<int>();
            }

            yield return output;
        }

        public IEnumerator GetIsTutorial()
        {
            var url = BASE_URL + "/tutorial";
            var cd = new CoroutineWithData(this, WebRequest(url, "GET"));
            yield return cd.coroutine;
            if (cd.result == null)
                yield break;
            var data = cd.result as string;
            
            var obj = JObject.Parse(data);

            var val = obj["is_tutorial"].Value<bool>();
            yield return val;
        }

        [Serializable]
        private struct PatchTutorialClearJsonData
        {
            public bool is_tutorial;
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
            else
            {
                yield return uwr.downloadHandler.text;
            }
        }
    }
}