using System;
using System.Collections;
using System.Collections.Generic;
using UniRx.Async;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.UI;

/// <summary>
/// 정보 요청 최상위 클래스
/// </summary>
public class BaseRequest
{
    protected string baseUri = "https://dev.meum.me/nodeTest/";
    protected string classValue = string.Empty;
    protected string getValue = string.Empty;

    public WWWForm form = new WWWForm();
    public enum RequestType
    {
        POST,
        Get,
        Patch,
        Delete
    }
    public RequestType requestType = RequestType.Get;

    public UnityAction<BaseRespons> successOn = null;
    public int requestStatus = 0;
    public string type = string.Empty;
    public int page = 0;
    public string keyword = string.Empty;

    public BaseRespons responsData = null;

    public string GetUri()
    {
        return string.Format("{0}{1}{2}", baseUri, classValue, getValue);
    }

    public virtual void RequestOn()
    {
        Request();
    }

    async void Request()
    {
        try
        {
            var result = await GetTextAsync();
            ResponsOn(result);
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }

    /// <summary>
    /// UnityWebRequest를 async/await 에서 대기
    /// </summary>
    private async UniTask<string> GetTextAsync()
    {
        var uwr = requestType == RequestType.Get ? UnityWebRequest.Get(GetUri()):
                                                   UnityWebRequest.Post(GetUri(), form);

        if (requestType == RequestType.Patch)
        {
            uwr.method = "PATCH";
        }
        else if (requestType == RequestType.Delete)
        {
            uwr.method = "DELETE";
        }

        // SendWebRequest가 끝날 때까지 await 
        await uwr.SendWebRequest();

        if (uwr.isHttpError || uwr.isNetworkError)
        {
            // 실패시 예외 throw
            //throw new Exception(uwr.error + " uri = "+GetUri());
        }

        return uwr.downloadHandler.text;
    }
    
    public T GetData<T>(string jsonData)
    {
        T data = JsonUtility.FromJson<T>(jsonData);

        return data;
    }

    public virtual void ResponsOn(string jsonData)
    {

    }
}

public class BaseRespons
{
    public string success;
    public int id = 0;
    public string created_at = string.Empty;
    public string updated_at = string.Empty;
    public string deleted_at = string.Empty;
}

public class CommonRespons : BaseRespons
{
    public string result;
}

