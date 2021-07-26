using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

/// <summary>
/// 사운드를 관리하는 클래스
/// </summary>
public class SoundManager : MonoBehaviour
{

	private static SoundManager instance;
	public static SoundManager Instance
	{
		get
		{
			if (instance == null)
			{
				instance = GameObject.FindObjectOfType<SoundManager>();

				if (instance == null)
				{
					instance = new GameObject("SoundManager").AddComponent<SoundManager>();
					GameObject.DontDestroyOnLoad(instance.gameObject);
				}
			}
			return instance;
		}
	}

	private AudioSource bgmAudio;
	private List<AudioSource> seAudioList = new List<AudioSource>();
	public UnityAction<BGMSaveData , AudioSource> bgmAudioPlayOn = null;

	public float bgmValue = 0;
	public float seValue = 0;

	int currentPlaybgmId = -1;

	MeumSaveData meumSaveData = null;

	public bool soundOff = false;

	void Awake()
	{
		Init();
	}

	public void Start()
	{
		//PlayBGM("Main");
	}

	/// <summary>
	/// 초기화
	/// </summary>
	private void Init()
	{
		bgmValue = PlayerPrefs.GetFloat("bgmValue", 0.5f);
		BgmVolumeReset();

		seValue = PlayerPrefs.GetFloat("seValue", 0.5f);
		SeVolumeReset();

		meumSaveData = Resources.Load<MeumSaveData>("MeumSaveData");
	}

	/// <summary>
	/// BGM의 출력값 수정
	/// </summary>
	/// <param name="value"></param>
	public void BgmValueReset(float value)
	{
		bgmValue = value;
		PlayerPrefs.SetFloat("bgmValue", bgmValue);
		BgmVolumeReset();
	}

	/// <summary>
	/// bgmAudio의 볼륨값 수정
	/// </summary>
	void BgmVolumeReset()
	{
		if (bgmAudio == null)
			bgmAudio = gameObject.AddComponent<AudioSource>();

		bgmAudio.volume = bgmValue;
	}

	/// <summary>
	/// SE의 출력값 수정
	/// </summary>
	/// <param name="value"></param>
	public void SeValueReset(float value)
	{
		seValue = value;
		PlayerPrefs.SetFloat("seValue", seValue);
		SeVolumeReset();
	}

	/// <summary>
	/// seAudio의 볼륨값 수정 
	/// </summary>
	void SeVolumeReset()
	{
		foreach (var seAudio in seAudioList)
		{
			seAudio.volume = seValue;
		}
	}

	/// <summary>
	/// BGM 실행
	/// </summary>
	/// <param name="path"></param>
	public void PlayBGM(string path)
	{
		bgmAudio.clip = Resources.Load<AudioClip>(path);
		bgmAudio.loop = true;
		bgmAudio.Play();
	}

	public void PlayBGM(int bgmID)
	{
		BGMSaveData bGMSaveData = meumSaveData.GetBGMData(bgmID);

		if (currentPlaybgmId == bgmID)
		{
			if (bgmAudioPlayOn != null)
			{
				bgmAudioPlayOn(bGMSaveData, bgmAudio);
			}
			return;
		}
			
		currentPlaybgmId = bgmID;

        if (bGMSaveData == null)
			return;
		
		//클립이 없으면 S3에서 다운로드
		if (bGMSaveData.audioClip == null)
		{
			StartCoroutine(DownLoadBgm(bGMSaveData));
			return;
		}
        else
		{
			PlayBGM(bGMSaveData);
		}
	}

	IEnumerator DownLoadBgm(BGMSaveData bGMSaveData)
	{
		string bgmName = string.Format("BGM_{0:d2}.mp3", bGMSaveData.bgmId);
		string baseUrl = "https://s3.ap-northeast-2.amazonaws.com/meum.test.addressable/BGM";
		string url = string.Format("{0}/{1}", baseUrl, bgmName);

		UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(url,AudioType.MPEG);

		yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
		{
			bGMSaveData.audioClip = DownloadHandlerAudioClip.GetContent(www);
			PlayBGM(bGMSaveData);
		}
        else
		{
			Debug.LogWarning("BGM DownLoad Error = "+www.error);
		}
	}

	public void PlayBGM(BGMSaveData bGMSaveData)
	{
		bgmAudio.clip = bGMSaveData.audioClip;
		bgmAudio.loop = true;
		bgmAudio.Play();

        if (bgmAudioPlayOn != null)
		{
			bgmAudioPlayOn(bGMSaveData , bgmAudio);
		}
	}


	/// <summary>
	/// SE 실행
	/// </summary>
	/// <param name="path"></param>
	public AudioSource PlaySe(string path)
	{
		AudioSource audioSource = null;

		foreach (var seAudio in seAudioList)
		{
			if (!seAudio.isPlaying)
			{
				audioSource = seAudio;
				break;
			}
		}

		if (audioSource == null)
		{
			audioSource = gameObject.AddComponent<AudioSource>();
			seAudioList.Add(audioSource);
			SeVolumeReset();
		}

		audioSource.clip = Resources.Load<AudioClip>("Se/" + path);
		audioSource.Play();

		return audioSource;
	}


}


