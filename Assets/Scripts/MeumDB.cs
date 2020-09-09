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
    public IEnumerator Login(string email, string pwd)
    {
        LoginData loginData;
        loginData.email = email;
        loginData.password = pwd;

        var json = JsonConvert.SerializeObject(loginData);
        var cd = new CoroutineWithData(this, WebRequest("http://52.78.99.172:8000/login", "POST", json));
        yield return cd.coroutine;
        var data = cd.result as string;
        var obj = JObject.Parse(data);
        if (obj["token"] == null) 
            yield return false;
        else
        {
            _token = obj["token"].Value<string>();
            yield return true;
        }
    }

    public void Logout()
    {
        StartCoroutine(WebRequest("http://52.78.99.172:8000/logout", "POST"));
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
        var cd = new CoroutineWithData(this, WebRequest(url, "GET"));
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
    public IEnumerator GetUserInfo()
    {
        var url = "http://52.78.99.172:8000/user";
        var cd = new CoroutineWithData(this, WebRequest(url, "GET"));
        yield return cd.coroutine;
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
    }
    public IEnumerator GetRoomInfoWithUser(int userPK)
    {
        var url = "http://52.78.99.172:8000/room/owner/" + userPK;
        var cd = new CoroutineWithData(this, WebRequest(url, "GET"));
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
    public IEnumerator GetRoomInfo(int roomPK)
    {
        var url = "http://52.78.99.172:8000/room/" + roomPK;
        var cd = new CoroutineWithData(this, WebRequest(url, "GET"));
        yield return cd.coroutine;
        var data = cd.result as string;

        var output = new RoomInfo();
        var json = JObject.Parse(data);
        output.primaryKey = json["pk"].Value<int>();
        output.max_people = json["max_people"].Value<int>();
        output.type_int = json["type_int"].Value<int>();
        output.data_json = json["data_json"].Value<string>();
        yield return output;
    }

    [System.Serializable]
    private struct PatchRoomJsonData
    {
        public string data_json;
    }
    public IEnumerator PatchRoomJson(string jsonData)
    {
        PatchRoomJsonData data;
        data.data_json = jsonData;

        var json = JsonConvert.SerializeObject(data);
        var url = "http://52.78.99.172:8000/room/owner";
        var cd = new CoroutineWithData(this, WebRequest(url, "PATCH", json));
        yield return cd.coroutine;
        // var response = cd.result as string;
    }

    private IEnumerator WebRequest(string url, string method, string json="")
    {
        var uwr = new UnityWebRequest(url, method);
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
}
