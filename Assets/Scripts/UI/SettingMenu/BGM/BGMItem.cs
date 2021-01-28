using Game;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class BGMItem : MonoBehaviour
{
    [SerializeField] Text nameText;
    [SerializeField] Image iconImage;

    [SerializeField] Sprite playIconImage;
    [SerializeField] Sprite pauseIconImage;

    AudioClip currentAudioClip;
    int index = 0;
    UnityAction<int> choisOnEvent = null;
    UnityAction<int, bool> playOnEvent = null;

    bool selectOn = false;
    public bool playOn = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DataSet(BGMSaveData bGMSaveData, int index , UnityAction<int> choisOnEvent , UnityAction<int , bool> playOnEvent)
    {
        currentAudioClip = bGMSaveData.audioClip;
        nameText.text = bGMSaveData.name;
        this.index = index;
        this.choisOnEvent = choisOnEvent;
        this.playOnEvent = playOnEvent;
    }

    public void PlayAudio()
    {
        Debug.LogWarning(currentAudioClip.name);
    }

    public void ChoisBGMOn()
    {
        if (choisOnEvent != null)
        {
            choisOnEvent(index);
        }

    }

    public void SelectOnReset(bool selectOn)
    {
        this.selectOn = selectOn;

        Color32 cor = this.selectOn ? new Color32(55, 115, 55,255) : new Color32(255, 255, 255, 255);

        nameText.color = cor;
        iconImage.color = cor;
    }

    public void PlayOnBtnClick()
    {
        if (playOnEvent != null)
        {
            playOnEvent(index , playOn);
        }
    }

    public void PlayOnReset(bool playOn)
    {
         this.playOn = playOn;

        iconImage.sprite = this.playOn ? pauseIconImage : playIconImage;
    }


}
