using Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BGMSetController : MonoBehaviour
{
    [SerializeField] BGMWaveController bgmWaveController;

    MeumSaveData meumSaveData;
    List<BGMSaveData> activeBGMList = new List<BGMSaveData>();
    int bgmIndex = 1;

    BGMSaveData bGMSaveData = null;

    bool playOn = false;

    [SerializeField] Image playBtnImage;
    [SerializeField] List<Sprite> playBtnSpriteList = new List<Sprite>();

    AudioSource bgmAudioSource;

    [SerializeField] Text bgmNameText;
    [SerializeField] Text maxTimeText;
    [SerializeField] Text currentTimeText;

    bool listUpOn = false;

    [SerializeField] Image listUpBtnImage;

    [SerializeField] RectTransform bgmListPanel;
    [SerializeField] RectTransform bgmListItemParant;
    [SerializeField] BGMListItem bgmListItemPrefab;


    [SerializeField] Image slider_BG;
    [SerializeField] Image slider_FG;

    [SerializeField] Sprite sprite;

    private void Awake()
    {
        SoundManager.Instance.bgmAudioPlayOn += BGMAudioPlayOn;
        DataSet();
        BGMListCreate();
        SliderImageSet(sprite);
    }

    void DataSet()
    {
        meumSaveData = Resources.Load<MeumSaveData>("MeumSaveData");

        List<int> hideIDList = new List<int>()
        {
            4
        };

        List<BGMSaveData> bgmSaveDataList = meumSaveData.bgmDataList;

        for (int i = 1; i < bgmSaveDataList.Count; i++)
        {
            int id = bgmSaveDataList[i].bgmId;

            if (hideIDList.Contains(id) == false)
            {
                activeBGMList.Add(bgmSaveDataList[i]);
            }
        }

        if (MeumDB.Get() != null)
        {
            string[] splitData = MeumDB.Get().currentRoomInfo.bgm_addValue_string.Split(',');

            List<int> addTypeList = new List<int>();

            foreach (var data in splitData)
            {
                int addType = 0;

                int.TryParse(data, out addType);

                BGMSaveData bGMSaveData = meumSaveData.GetBGMData(addType);

                if (bGMSaveData != null && activeBGMList.Contains(bGMSaveData) == false)
                {
                    activeBGMList.Add(bGMSaveData);
                }
            }
        }
    }

    void SliderImageSet(Sprite sprite)
    {
        slider_BG.sprite = sprite;
        slider_FG.sprite = sprite;
    }

    void BGMListCreate() 
    {
        for (int i = 0; i < activeBGMList.Count; i++)
        {
            BGMListItem bgmListItem = Instantiate(bgmListItemPrefab, bgmListItemParant);
            bgmListItem.BGMDataSet(i,activeBGMList[i]);
            bgmListItem.clickOn += ListItemClickOn;
        }
        bgmListItemPrefab.gameObject.SetActive(false);
    }

    void ListItemClickOn(int index)
    {
        BGMSelectOn(index);
    }


    // Start is called before the first frame update
    void Start()
    {
        if (MeumDB.Get() != null) 
        {
            int currentBgmID = MeumDB.Get().currentRoomInfo.bgm_type_int;

            for (int i = 0; i < activeBGMList.Count; i++)
            {
                if (activeBGMList[i].bgmId == currentBgmID)
                {
                    bgmIndex = i;
                    break;
                }
            }
        }
        
        BGMSelectOn(bgmIndex);
    }

    // Update is called once per frame
    void Update()
    {
        CurrentTimeCheck();
    }

    void BGMSelectOn(int bgmIndex)
    {
        this.bgmIndex = bgmIndex;

        if (this.bgmIndex >= activeBGMList.Count )
        {
            this.bgmIndex = 1;
        }

        BGMDataSet(activeBGMList[this.bgmIndex]);

        ListUpOnChange(false);
    }

    void BGMDataSet(BGMSaveData bGMSaveData)
    {
        this.bGMSaveData = bGMSaveData;

        bgmNameText.text = bGMSaveData.name;

        maxTimeText.text = GetTimeText(bGMSaveData.maxTime);

        SoundManager.Instance.PlayBGM(bGMSaveData.bgmId);
    }

    string GetTimeText(int timeValue)
    {
        int min = timeValue / 60;
        int sec = timeValue % 60;

        return string.Format("{0}:{1:D2}", min, sec);
    }

    void BGMAudioPlayOn(BGMSaveData bgmSaveData, AudioSource audioSource)
    {
        if (bGMSaveData.bgmId != bgmSaveData.bgmId)
            return;

        bgmAudioSource = audioSource;

        bgmWaveController.AudioSet(audioSource);
        playOn = true;
        PlayBtnImageSet();

        maxTimeText.text = GetTimeText(bgmSaveData.maxTime);
    }

    public void PlayBtnClick()
    {
        playOn = !playOn;

        if (playOn)
        {
            bgmAudioSource.Play();
        }
        else 
        {
            bgmAudioSource.Pause();
        }

        PlayBtnImageSet();
    }

    void PlayBtnImageSet()
    {
        Sprite sprite = playOn ? playBtnSpriteList[0] : playBtnSpriteList[1];
        playBtnImage.sprite = sprite;
    }

    public void IndexChange(bool nextOn)
    {
        if (nextOn)
        {
            bgmIndex += 1;
        }
        else
        {
            bgmIndex -= 1;
        }
        BGMSelectOn(bgmIndex);
    }

    void CurrentTimeCheck()
    {
        if (bgmAudioSource == null || bgmAudioSource.isPlaying == false)
            return;

        currentTimeText.text = GetTimeText((int)bgmAudioSource.time);
    }

    public void ListUpBtnClickOn()
    {
        ListUpOnChange(!listUpOn);
    }

    void ListUpOnChange(bool listUpOn)
    {
        this.listUpOn = listUpOn;

        float zValue = listUpOn ? 180 : 0;

        listUpBtnImage.rectTransform.rotation = Quaternion.Euler(0, 0, zValue);

        bgmListPanel.gameObject.SetActive(listUpOn);
    }

    public void SaveBtnClick()
    {
        new RoomRequest()
        {
            requestStatus = 3,
            uid = MeumDB.Get().GetToken(),
            bgm_type_int = bGMSaveData.bgmId,
        }.RequestOn();

        MeumDB.Get().currentRoomInfo.bgm_type_int = bGMSaveData.bgmId;
    }

}
