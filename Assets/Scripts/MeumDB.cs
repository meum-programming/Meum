using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine.Networking;

public class MeumDB : MonoBehaviour
{
    private string _token = "";

    private static MeumDB _globalInstance;
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
    public void Login(string email, string pwd)
    {
        LoginData data;
        data.email = email;
        data.password = pwd;

        var json = JsonConvert.SerializeObject(data);
        StartCoroutine(LoginCoroutine(json));
    }
    private IEnumerator LoginCoroutine(string json)
    {
        var cd = new CoroutineWithData(this, PostRequest("http://52.78.99.172:8000/login", json));
        yield return cd.coroutine;
        var data = cd.result as string;
        var obj = JObject.Parse(data);
        _token = obj["token"].Value<string>();
        Debug.Log(_token);
    }

    public void Logout()
    {
        StartCoroutine(PostRequest("http://52.78.99.172:8000/logout", ""));
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
        var url = "http://52.78.99.172:8000/profile/nickname/" + nickname;
        var cd = new CoroutineWithData(this, GetRequest(url));
        yield return cd.coroutine;
        var data = cd.result as string;

        var output = new UserInfo();
        var jarray = JArray.Parse(data);
        if (jarray.Count == 0)
        {
            Debug.Log("user not exist, nickname: " + nickname);
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

    public class RoomInfo
    {
        public int primaryKey;
        public int max_people;
        public int type_int;
        public string data_json;
    }
    public IEnumerator GetRoomInfo(int userPK)
    {
        var url = "http://52.78.99.172:8000/room/owner/" + userPK;
        var cd = new CoroutineWithData(this, GetRequest(url));
        yield return cd.coroutine;
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
            yield return output;
        }
    }
    
    private delegate void RequestCallback(string data);
    private IEnumerator PostRequest(string url, string json)
    {
        var uwr = new UnityWebRequest(url, "POST");
        if (json != "")
        {
            var jsonToSend = new System.Text.UTF8Encoding().GetBytes(json);
            uwr.uploadHandler = new UploadHandlerRaw(jsonToSend);
        }
        uwr.downloadHandler = new DownloadHandlerBuffer();
        uwr.SetRequestHeader("Content-Type", "application/json");
        if(_token != "")
            uwr.SetRequestHeader("Authorization", "JWT " + _token);

        yield return uwr.SendWebRequest();
        
        if (uwr.isNetworkError)
        {
            Debug.Log(url + ": Error: " + uwr.error);
        }
        else
        {
            yield return uwr.downloadHandler.text;
        }
    }
    
    private IEnumerator GetRequest(string url)
    {
        UnityWebRequest uwr = UnityWebRequest.Get(url);
        if(_token != "")
            uwr.SetRequestHeader("Authorization", "JWT " + _token);
        uwr.downloadHandler = new DownloadHandlerBuffer();
        
        yield return uwr.SendWebRequest();
        
        if (uwr.isNetworkError)
        {
            Debug.Log(url + ": Error: " + uwr.error);
        }
        else
        {
            yield return uwr.downloadHandler.text;
        }
    }
}
