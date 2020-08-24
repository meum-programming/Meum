using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

[Serializable]
public struct PaintData
{
    public string url;
    public Vector3 position;
    public Vector3 eulerAngle;
    public Vector3 scale;

    public PaintData(string url, Vector3 position, Vector3 eulerAngle, Vector3 scale)
    {
        this.url = url;
        this.position = position;
        this.eulerAngle = eulerAngle;
        this.scale = scale;
    }
}


public class PaintInfo : MonoBehaviour
{
    [SerializeField] public Renderer paintRenderer;
    [SerializeField] public Material mat;
    
    private IEnumerator _applying = null;
    private string _url = "";
    public string URL
    {
        get { return _url; }
        set
        {
            _url = value;
            if (_applying != null)
            {
                StopCoroutine(_applying);
                _applying = null;
            }
            StartCoroutine(_applying = ApplyTexture());
        }
    }

    public PaintData GetData()
    {
        var selfTransform = transform;
        return new PaintData(this.URL, 
                            selfTransform.position, selfTransform.eulerAngles, selfTransform.localScale);
    }

    public void SetUpWithData(PaintData data)
    {
        var selfTransform = transform;
        selfTransform.position = data.position;
        selfTransform.eulerAngles = data.eulerAngle;
        selfTransform.localScale = data.scale;

        URL = data.url;
    }
    
    private IEnumerator ApplyTexture()
    {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(_url);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
            yield break;
        }
        
        var texture = ((DownloadHandlerTexture) www.downloadHandler).texture;
        paintRenderer.material = new Material(mat);
        paintRenderer.material.mainTexture = texture;

        var scale = transform.localScale;
        scale.x = scale.y * ((float)texture.width / texture.height);
        transform.localScale = scale;
    } 
}
