using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.Video;

public class TestMp4 : MonoBehaviour
{
    public VideoPlayer videoPlayer;

    public InputField inputField;
    public Text text;

    public bool autoPlay = false;

    // Start is called before the first frame update
    void Start()
    {
        text.text = Application.streamingAssetsPath;

        if (autoPlay)
        {
            VideoPlay();

            //videoPlayer.url = System.IO.Path.Combine(Application.streamingAssetsPath, "1024p-1.mp4");
            //videoPlayer.url = "https://meumbucket.s3.ap-northeast-2.amazonaws.com/MP4Test/Test.mp4";
            //videoPlayer.url = "https://www.youtube.com/watch?v=iQZobAhgayA";
            //videoPlayer.Play();
        }
        
    }

    IEnumerator GetRequest(string uri)
    {
        Debug.LogWarning("Play ON!");

        UnityWebRequest www = new UnityWebRequest(uri);
        www.downloadHandler = new DownloadHandlerBuffer();
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
        }
        else
        {
            // Show results as text
            Debug.Log(www.downloadHandler.text);

            // Or retrieve results as binary data
            byte[] results = www.downloadHandler.data;

            //www.downloadHandler.ve

            Debug.LogWarning("results = " + results.Length);

        }

    }

    public void VideoPlay()
    {
        string url = inputField.text;

        Debug.LogWarning("url = " + url);

        videoPlayer.url = url;
        videoPlayer.Play();

        //StartCoroutine(GetRequest(url));

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
