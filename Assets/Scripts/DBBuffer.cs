using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Networking;

public abstract class DbBufferBase<T>
{
    private Dictionary<string, T> _buffer = new Dictionary<string, T>();

    public CoroutineWithData Get(MonoBehaviour caller, string url)
    {
        var cd = new CoroutineWithData(caller, Load(url));
        return cd;
    }

    public void Clear()
    {
        _buffer.Clear();
    }

    protected bool Contains(string url)
    {
        return _buffer.ContainsKey(url);
    }

    protected void Add(string url, T value)
    {
        _buffer.Add(url, value);
    }

    public T Get(string url)
    {
        if(_buffer.ContainsKey(url))
            return _buffer[url];
        return default(T);
    }
    
    protected abstract IEnumerator Load(string url);
}

public class TextureBuffer : DbBufferBase<Texture2D>
{
    protected override IEnumerator Load(string url)
    {
        if (Contains(url))
            yield return Get(url);
        else
        {
            UnityWebRequest www =  UnityWebRequestTexture.GetTexture(url);
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
                yield break;
            }

            var texture = ((DownloadHandlerTexture) www.downloadHandler).texture;
            Add(url, texture);
            yield return texture;
        }
    }
}

public class Object3DBuffer : DbBufferBase<GameObject>
{
    protected override IEnumerator Load(string url)
    {
        if (Contains(url))
            yield return Get(url);
        else
        {
            UnityWebRequest www = UnityWebRequestAssetBundle.GetAssetBundle(url);
            yield return www.SendWebRequest();
            
            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
                yield break;
            }

            var bundle = ((DownloadHandlerAssetBundle) www.downloadHandler).assetBundle;
            var allAssets = bundle.GetAllAssetNames();
            Assert.AreNotEqual(allAssets.Length, 0);
            var request = bundle.LoadAssetAsync(allAssets[0], typeof(GameObject));
            yield return request;

            var result = request.asset as GameObject;
            Add(url, result);
            yield return result;
        }
    }
}
