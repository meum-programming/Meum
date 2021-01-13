using System;
using System.Collections;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine.Assertions;
using UnityEngine.Networking;

namespace Core
{
    /*
     * @brief DB API 호출을 담당하는 컴포넌트, Singleton임
     */
    public class MeumDB : Singleton<MeumDB>
    {
        #region PrivateFields
        
        private TextureBuffer _textureBuffer = new TextureBuffer();
        private Object3DBuffer _object3DBuffer = new Object3DBuffer();
        private string _token = "";                                           // API 서버의 authorization token
        private const string BASE_URL = "https://api.meum.me";                // API 서버의 BASE URL  
        
        #endregion
        
        private void Awake()
        {
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

        public IEnumerator GetUserInfo(string nickname)
        {
            var url = BASE_URL + "/profile/nickname/" + nickname;
            var cd = new CoroutineWithData(this, WebRequest(url, "GET"));
            yield return cd.coroutine;
            Assert.IsNotNull(cd.result);
            var data = cd.result as string;
            Assert.IsNotNull(data);

            var jarray = JArray.Parse(data);
            Assert.IsNotNull(jarray);
            if (jarray.Count == 0)
                yield return null;
            else
            {
                var json = jarray[0]["user"];
                yield return ParseUserInfo(json);
            }
        }

        public IEnumerator GetUserInfo()
        {
            var url = BASE_URL + "/user";
            var cd = new CoroutineWithData(this, WebRequest(url, "GET"));
            yield return cd.coroutine;
            Assert.IsNotNull(cd.result);
            var data = cd.result as string;
            Assert.IsNotNull(data);

            var output = new UserInfo();
            var json = JObject.Parse(data);
            Assert.IsNotNull(json);
            yield return ParseUserInfo(json);
        }

        public IEnumerator GetRoomInfoWithUser(int userPK)
        {
            var url = BASE_URL + "/room/owner/" + userPK;
            var cd = new CoroutineWithData(this, WebRequest(url, "GET"));
            yield return cd.coroutine;
            Assert.IsNotNull(cd.result);
            var data = cd.result as string;
            Assert.IsNotNull(data);

            var output = new RoomInfo();
            var jarray = JArray.Parse(data);
            Assert.IsNotNull(jarray);
            if (jarray.Count == 0)
                yield return null;
            else
            {
                var json = jarray[0];
                yield return ParseRoomInfo(json);
            }
        }

        public IEnumerator GetRoomInfo(int roomPK)
        {
            var url = BASE_URL + "/room/" + roomPK;
            var cd = new CoroutineWithData(this, WebRequest(url, "GET"));
            yield return cd.coroutine;
            Assert.IsNotNull(cd.result);
            var data = cd.result as string;
            Assert.IsNotNull(data);

            var output = new RoomInfo();
            var json = JObject.Parse(data);
            Assert.IsNotNull(json);
            yield return ParseRoomInfo(json);
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

        public IEnumerator GetArtworks()
        {
            var url = BASE_URL + "/artwork";
            var cd = new CoroutineWithData(this, WebRequest(url, "GET"));
            yield return cd.coroutine;
            Assert.IsNotNull(cd.result);
            var data = cd.result as string;
            Assert.IsNotNull(data);

            var jarray = JArray.Parse(data);
            Assert.IsNotNull(jarray);
            var output = new ArtworkInfo[jarray.Count];
            for (var i = 0; i < jarray.Count; ++i)
            {
                var obj = jarray[i];
                output[i] = ParseArtworkInfo(obj);
            }

            yield return output;
        }

        public IEnumerator GetArtwork(int primaryKey)
        {
            var url = BASE_URL + "/artwork/" + primaryKey;
            var cd = new CoroutineWithData(this, WebRequest(url, "GET"));
            yield return cd.coroutine;
            Assert.IsNotNull(cd.result);
            var data = cd.result as string;
            Assert.IsNotNull(data);
            
            var json = JObject.Parse(data);
            Assert.IsNotNull(json);
            yield return ParseArtworkInfo(json);
        }

        public IEnumerator GetPurchasedArtworks()
        {
            var url = BASE_URL + "/shop/buy";
            var cd = new CoroutineWithData(this, WebRequest(url, "GET"));
            yield return cd.coroutine;
            Assert.IsNotNull(cd.result);
            var data = cd.result as string;
            Assert.IsNotNull(data);
            
            var jarray = JArray.Parse(data);
            Assert.IsNotNull(jarray);
            var output = new ArtworkInfo[jarray.Count];
            for (var i = 0; i < jarray.Count; ++i)
            {
                var obj = jarray[i]["artwork"];
                output[i] = ParseArtworkInfo(obj);
            }

            yield return output;
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

        public IEnumerator GetGuestBooks(int roomPk)
        {
            var url = BASE_URL + "/guestbook/" + roomPk;
            var cd = new CoroutineWithData(this, WebRequest(url, "GET"));
            yield return cd.coroutine;
            Assert.IsNotNull(cd.result);
            var data = cd.result as string;
            Assert.IsNotNull(data);
            
            var jarray = JArray.Parse(data);
            Assert.IsNotNull(jarray);
            var output = new GuestBookInfo[jarray.Count];
            for (var i = 0; i < jarray.Count; ++i)
            {
                var obj = jarray[i];
                output[i] = ParseGuestBookInfo(obj);
            }

            yield return output;
        }

        public IEnumerator GetGuestBookStampCount(int roomPk)
        {
            var url = BASE_URL + "/guestbook/count/" + roomPk;
            var cd = new CoroutineWithData(this, WebRequest(url, "GET"));
            yield return cd.coroutine;
            Assert.IsNotNull(cd.result);
            var data = cd.result as string;
            Assert.IsNotNull(data);

            var json = JObject.Parse(data);
            Assert.IsNotNull(json);

            yield return ParseGuestBookStampCountInfo(json);
        }

        public IEnumerator PostGuestBookCreate(int roomPk, int stampType, string content)
        {
            GuestBookCreateInfo data;
            data.stamp_type = stampType;
            data.content = content;

            var json = JsonConvert.SerializeObject(data);
            var url = BASE_URL + "/guestbook/" + roomPk;
            var cd = new CoroutineWithData(this, WebRequest(url, "POST", json));
            yield return cd.coroutine;
        }

        public IEnumerator DeleteGuestBook(int guestbookPk)
        {
            var url = BASE_URL + "/guestbook/detail/" + guestbookPk;
            var cd = new CoroutineWithData(this, WebRequest(url, "DELETE"));
            yield return cd.coroutine;
        }

        public IEnumerator GetComments(int artworkPk)
        {
            var url = BASE_URL + "/comment/artwork/" + artworkPk;
            var cd = new CoroutineWithData(this, WebRequest(url, "GET"));
            yield return cd.coroutine;
            Assert.IsNotNull(cd.result);
            var data = cd.result as string;
            Assert.IsNotNull(data);

            var json = JObject.Parse(data);
            Assert.IsNotNull(json);

            var jarray = json["results"] as JArray;
            Assert.IsNotNull(jarray);

            var output = new CommentInfo[jarray.Count];
            for (var i = 0; i < jarray.Count; ++i)
            {
                output[i] = ParseCommentInfo(jarray[i]);
            }

            yield return output;
        }

        public IEnumerator PostCommentCreate(int artworkPk, string content)
        {
            CommentCreateInfo data;
            data.content = content;

            var json = JsonConvert.SerializeObject(data);
            var url = BASE_URL + "/comment/artwork/" + artworkPk;
            var cd = new CoroutineWithData(this, WebRequest(url, "POST", json));
            yield return cd.coroutine;
        }

        public IEnumerator DeleteComment(int commentPk)
        {
            var url = BASE_URL + "/comment/" + commentPk;
            var cd = new CoroutineWithData(this, WebRequest(url, "DELETE"));
            yield return cd.coroutine;
        }
        
        #endregion
        
        #region Parsing functions

        private static UserInfo ParseUserInfo(JToken obj)
        {
            var output = new UserInfo();
            output.primaryKey = obj["pk"].Value<int>();
            output.email = obj["email"].Value<string>();
            output.nickname = obj["profile"]["nickname"].Value<string>();
            output.phone = obj["profile"]["phone"].Value<string>();
            return output;
        }

        private static RoomInfo ParseRoomInfo(JToken obj)
        {
            var output = new RoomInfo();
            output.primaryKey = obj["pk"].Value<int>();
            output.max_people = obj["max_people"].Value<int>();
            output.type_int = obj["type_int"].Value<int>();
            output.data_json = obj["data_json"].Value<string>();
            output.owner = ParseUserInfo(obj["owner"]);
            return output;
        }

        private static ArtworkInfo ParseArtworkInfo(JToken obj)
        {
            var output = new ArtworkInfo();
            output.primaryKey = obj["pk"].Value<int>();
            output.author = obj["owner"]["profile"]["nickname"].Value<string>();
            output.title = obj["title"].Value<string>();
            if(obj["size_w"].Type != JTokenType.Null)
                output.size_w = obj["size_w"].Value<int>() / 100.0f;
            if(obj["size_h"].Type != JTokenType.Null)
                output.size_h = obj["size_h"].Value<int>() / 100.0f;
            if(obj["year"].Type != JTokenType.Null)
                output.year = obj["year"].Value<int>();
            if(obj["author"].Type != JTokenType.Null)
                output.author = obj["author"].Value<string>();
            if(obj["object_file"].Type != JTokenType.Null)
                output.object_file = BASE_URL + obj["object_file"].Value<string>();
            if (obj["image_file"].Type != JTokenType.Null)
                output.image_file = BASE_URL + obj["image_file"].Value<string>();
            if (obj["thumbnail"].Type != JTokenType.Null)
                output.thumbnail = BASE_URL + obj["thumbnail"].Value<string>();
            if(obj["instruction"].Type != JTokenType.Null)
                output.instruction = obj["instruction"].Value<string>();
            output.like = obj["like"].Value<int>();
            output.hate = obj["hate"].Value<int>();
            output.type_artwork = obj["type_artwork"].Value<int>();
            return output;
        }

        private static GuestBookInfo ParseGuestBookInfo(JToken obj)
        {
            var output = new GuestBookInfo();
            output.pk = obj["pk"].Value<int>();
            output.stamp_type = obj["stamp_type"].Value<int>();
            output.content = obj["content"].Value<string>();
            output.writer = ParseUserInfo(obj["writer"]);
            output.created_at = obj["created_at"].Value<string>();
            return output;
        }

        private static GuestBookStampCountInfo ParseGuestBookStampCountInfo(JToken obj)
        {
            var output = new GuestBookStampCountInfo();
            output.one = obj["one_queryset"].Value<int>();
            output.two = obj["two_queryset"].Value<int>();
            output.three = obj["three_queryset"].Value<int>();
            output.four = obj["four_queryset"].Value<int>();
            return output;
        }

        private static CommentInfo ParseCommentInfo(JToken obj)
        {
            var output = new CommentInfo();
            output.pk = obj["pk"].Value<int>();
            output.artwork = ParseArtworkInfo(obj["artwork"]);
            output.content = obj["content"].Value<string>();
            output.writer = ParseUserInfo(obj["writer"]);
            output.created_at = obj["created_at"].Value<string>();
            return output;
        }
        
        #endregion
        
        #region API json struct
        
        private struct LoginData
        {
            public string email;
            public string password;
        }
        
        public class UserInfo
        {
            public int primaryKey;
            public string email;
            public string nickname;
            public string phone;
        }
        
        public class RoomInfo
        {
            public int primaryKey;
            public int max_people;
            public int type_int;
            public string data_json;
            public UserInfo owner;
        }
        
        private struct PatchRoomJsonData
        {
            public string data_json;
        }
        
        public class ArtworkInfo
        {
            public int primaryKey;
            public string author;
            public string title;
            public float size_w;
            public float size_h;
            public int year;
            public string object_file;
            public string image_file;
            public string thumbnail;
            public string instruction;
            public int like;
            public int hate;
            public int type_artwork;
        }
        
        private struct PatchTutorialClearJsonData
        {
            public bool is_tutorial;
        }

        public class GuestBookInfo
        {
            public int pk;
            public int stamp_type;
            public string content;
            public UserInfo writer;
            public string created_at;
        }

        public class GuestBookStampCountInfo
        {
            public int one;
            public int two;
            public int three;
            public int four;
        }

        private struct GuestBookCreateInfo
        {
            public int stamp_type;
            public string content;
        }

        public struct CommentInfo
        {
            public int pk;
            public ArtworkInfo artwork;
            public string content;
            public UserInfo writer;
            public string created_at;
        }

        private struct CommentCreateInfo
        {
            public string content;
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
    }
}