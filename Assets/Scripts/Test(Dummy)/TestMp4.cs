using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class TestMp4 : MonoBehaviour
{
    public VideoPlayer videoPlayer;


    // Start is called before the first frame update
    void Start()
    {
        videoPlayer.url = System.IO.Path.Combine(Application.streamingAssetsPath, "1024p-1 1.mp4");
        videoPlayer.Play();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
