using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BGMWaveController : MonoBehaviour
{
    private AudioSource audioSource;

    private float[] samples;

    [SerializeField] private RectTransform instanceObjParant;
    [SerializeField] private BGMWave instanceObj;

    List<BGMWave> bgmWaveList = new List<BGMWave>();

    float waveCnt = 0;

    [SerializeField] private Image currentPointObj;
    public int currentPointIndex = 0;
    public int beforePointIndex = 0;

    void Start()
    {
        WaveImageCreate();
    }

    void WaveImageCreate()
    {
        float areaWidth = instanceObjParant.sizeDelta.x;
        float waveWidth = instanceObj.rect.sizeDelta.x;

        waveCnt = areaWidth / (waveWidth * 2);

        for (int i = 0; i < waveCnt; i++)
        {
            BGMWave bgmWave = Instantiate(instanceObj, instanceObjParant);
            bgmWave.SetSize(0);
            bgmWave.ColorChange(false);
            bgmWaveList.Add(bgmWave);    
        }

        instanceObj.gameObject.SetActive(false);

        Debug.LogWarning("bgmWaveList = " + bgmWaveList.Count);
    }

    public void AudioSet(AudioSource audioSource)
    {
        this.audioSource = audioSource;
        WaveValueSet(audioSource.clip);
    }

    void WaveValueSet(AudioClip audioClip)
    {
        int numOfSamples = audioClip.samples * audioClip.channels;

        samples = new float[numOfSamples];

        if (audioClip.GetData(samples, 0))
        {
            var AveragedSound = new float[samples.Length / 10];
            var j = 0;
            for (var i = 0; i < samples.Length / 10 - 1; ++i)
            {
                j += 10;

                float value = Mathf.Abs(samples[j]) + Mathf.Abs(samples[j + 1]) + Mathf.Abs(samples[j + 2]) + Mathf.Abs(samples[j + 3]) + Mathf.Abs(samples[j + 4])
                            + Mathf.Abs(samples[j + 5]) + Mathf.Abs(samples[j + 6]) + Mathf.Abs(samples[j + 7]) + Mathf.Abs(samples[j + 8]) + Mathf.Abs(samples[j + 9]);

                AveragedSound[i] = (value / 10);
            }

            List<float> valueList = new List<float>();

            for (int i = 0; i < waveCnt; i++)
            {
                int index = 0;

                if (i > 0)
                {
                    index = (int)((i) / waveCnt * AveragedSound.Length);
                }

                float waveValue = Mathf.Min(AveragedSound[index] * 150, 50);

                valueList.Add(waveValue);
            }

            for (int i = 0; i < valueList.Count; i++)
            {
                float reslutValue = valueList[i];

                if (i > 0 && i < valueList.Count - 1)
                {
                    reslutValue = (valueList[i - 1] + valueList[i] + valueList[i + 1]) / 3;
                }

                bgmWaveList[i].SetSize(reslutValue);
            }
            IndexSet(true);
        }
        else 
        {
            Debug.LogWarning("fail");
        }
    }

    private void Update()
    {
        //SetCurrentIndex();
    }

    void SetCurrentIndex()
    {
        if (audioSource == null || audioSource.clip == null)
            return;

        IndexSet();
    }

    void IndexSet(bool init = false)
    {
        float timeRate = audioSource.time / audioSource.clip.length;

        int tempIndex = (int)(bgmWaveList.Count * timeRate);

        if (currentPointIndex != tempIndex)
        {
            if (tempIndex == 0)
                init = true;
            
            beforePointIndex = currentPointIndex;
            currentPointIndex = tempIndex;
            CurrentPointMove(init);
            ColorSet();
        }
    }

    void CurrentPointMove(bool init = false)
    {
        float duration = init ? 0 : 0.5f;

        if (tween != null && tween.IsPlaying())
            tween.Kill();

        tween = currentPointObj.rectTransform.DOAnchorPosX(bgmWaveList[currentPointIndex].rect.anchoredPosition3D.x, duration);
    }

    Tween tween = null;

    public void SliderClickOn(float value)
    {
        audioSource.time = audioSource.clip.length * value;
        IndexSet(true);
    }

    void ColorSet()
    {
        for (int i = 0; i < bgmWaveList.Count; i++)
        {
            bool colorActive = currentPointIndex >= i;

            bgmWaveList[i].ColorChange(colorActive);
        }
    }
}