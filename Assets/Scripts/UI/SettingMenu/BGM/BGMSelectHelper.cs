using Core;
using Game;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BGMSelectHelper : MonoBehaviour
{
    [SerializeField] Text selectBGMName;
    [SerializeField] Image arrowImage;

    [SerializeField] ScrollRect scrollRect;
    [SerializeField] RectTransform scrollRectCloseBG;
    [SerializeField] BGMItem itemIns;

    string bgmPath = "BGM/Active/";

    List<BGMSaveData> activeBGMList = new List<BGMSaveData>();

    int currentSelectIndex = -1;
    List<BGMItem> bgmItemList = new List<BGMItem>();

    bool isOwnerRoom = false;

    int defaultMaxValue = 2;

    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    void Init()
    {
        isOwnerRoom = MeumDB.Get().myRoomInfo.owner.primaryKey == MeumDB.Get().currentRoomInfo.owner.primaryKey;

        BGMDataSet();
        DropDownItemSet();

        currentSelectIndex = MeumDB.Get().currentRoomInfo.bgm_type_int;

        SelectOnDataSet(currentSelectIndex);

        if (isOwnerRoom == false)
        {
            gameObject.GetComponent<Button>().interactable = false;
        }

    }

    void SelectOnDataSet(int index)
    {
        currentSelectIndex = index;
        selectBGMName.text = activeBGMList[index].name;

        for (int i = 0; i < bgmItemList.Count; i++)
        {
            bgmItemList[i].SelectOnReset(i == currentSelectIndex);
        }
        
        DropDownOpen(false);

        BGMPlay(currentSelectIndex);

        new RoomRequest()
        {
            requestStatus = 3,
            uid = MeumDB.Get().GetToken(),
            bgm_type_int = currentSelectIndex,
        }.RequestOn();

        MeumDB.Get().currentRoomInfo.bgm_type_int = index;

    }


    void BGMDataSet()
    {
        MeumSaveData meumSaveData = Resources.Load<MeumSaveData>("MeumSaveData");

        List<BGMSaveData> bgmSaveDataList = meumSaveData.bgmDataList;

        for (int i = 1; i <= defaultMaxValue +1; i++)
        {
            activeBGMList.Add(bgmSaveDataList[i]);
        }

        string[] splitData = MeumDB.Get().currentRoomInfo.bgm_addValue_string.Split(',');

        List<int> addTypeList = new List<int>();

        foreach (var data in splitData)
        {
            int addType = 0;

            int.TryParse(data, out addType);

            if (addType > defaultMaxValue)
            {
                BGMSaveData bGMSaveData = meumSaveData.GetBGMData((BGMEnum)addType);

                if (bGMSaveData != null && activeBGMList.Contains(bGMSaveData) == false)
                {
                    activeBGMList.Add(bGMSaveData);
                }
            }
        }
    }

    void DropDownItemSet() 
    {
        for (int i = 0; i < activeBGMList.Count; i++)
        {
            BGMItem bGMItem = Instantiate(itemIns, scrollRect.content);

            bGMItem.DataSet(activeBGMList[i] , i, SelectOnDataSet , PlayOnReset);

            bGMItem.gameObject.SetActive(true);

            bgmItemList.Add(bGMItem);
        }

        RectTransform rect = scrollRect.GetComponent<RectTransform>();
        Vector2 pos = rect.sizeDelta;
        pos.y = 30 * activeBGMList.Count;
        rect.sizeDelta = pos;
    }

    void PlayOnReset(int index, bool playOn)
    {
        if (playOn == false)
        {
            for (int i = 0; i < bgmItemList.Count; i++)
            {
                bgmItemList[i].PlayOnReset(i == index);
            }

            BGMPlay(index);
        }
        else
        {
            bgmItemList[index].PlayOnReset(false);
            bgmItemList[currentSelectIndex].PlayOnReset(true);
            BGMPlay(currentSelectIndex);
        }
        
    }

    void BGMPlay(int index)
    {
        SoundManager.Instance.PlayBGM((BGMEnum)index);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DropDownOpen(bool active)
    {
        if (active && scrollRect.gameObject.activeSelf)
        {
            active = false;
        }

        scrollRect.gameObject.SetActive(active);
        scrollRectCloseBG.gameObject.SetActive(active);

        for (int i = 0; i < bgmItemList.Count; i++)
        {
            bgmItemList[i].PlayOnReset(i == currentSelectIndex);
        }

        Quaternion rot = active ? Quaternion.Euler(180, 0, 0) : Quaternion.Euler(0, 0, 0);
        arrowImage.transform.rotation = rot;

    }

    public void BGMValueReset(float value)
    {
        SoundManager.Instance.BgmValueReset(value);
    }

}
