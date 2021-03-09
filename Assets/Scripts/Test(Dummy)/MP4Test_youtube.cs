using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MP4Test_youtube : MonoBehaviour
{
    public bool autoPlay = false;
    public string url;
    [SerializeField] private YoutubePlayer player;
    [SerializeField] private Text loadingText;
    [SerializeField] private InputField inputField;

    // Start is called before the first frame update
    void Start()
    {
        player.autoPlayOnStart = false;
        player.videoQuality = YoutubePlayer.YoutubeVideoQuality.STANDARD;

        if (autoPlay)
        {
            PlayBtnClick();
        }

    }

    // Update is called once per frame
    void Update()
    {
    }

    public void PlayBtnClick()
    {
        StartCoroutine(LoadingVideo());
    }

    IEnumerator LoadingVideo()
    {
        int waitTime = 0;
        
        int.TryParse(inputField.text,out waitTime);

        if (waitTime > 0)
        {
            for (int i = waitTime; i >= 0; i--)
            {
                loadingText.text = string.Format("Loading .. " + i);
                yield return new WaitForSeconds(1);
            }
        }

        loadingText.text = string.Empty;

        player.Play(url);
        player.videoPlayer.GetTargetAudioSource(0).mute = true;
        player.videoPlayer.SetDirectAudioMute(0,true);
    }

}
