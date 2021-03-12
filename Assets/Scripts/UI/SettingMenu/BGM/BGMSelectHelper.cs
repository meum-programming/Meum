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

    int currentSelectID = -1;
    int currentSelectIndex = -1;
    List<BGMItem> bgmItemList = new List<BGMItem>();

    bool isOwnerRoom = false;

    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    void Init()
    {
        isOwnerRoom = MeumDB.Get().myRoomInfo.owner.id == MeumDB.Get().currentRoomInfo.owner.id;

        BGMDataSet();
        DropDownItemSet();

        currentSelectID = MeumDB.Get().currentRoomInfo.bgm_type_int;

        for (int i = 0; i < activeBGMList.Count; i++)
        {
            if (activeBGMList[i].bgmId == currentSelectID)
            {
                SelectOnDataSet(i);
                break;
            }
        }

        if (isOwnerRoom == false)
        {
            gameObject.GetComponent<Button>().interactable = false;
        }

    }

    void SelectOnDataSet(int index)
    {
        currentSelectIndex = index;
        currentSelectID = activeBGMList[index].bgmId;
        selectBGMName.text = activeBGMList[index].name;

        for (int i = 0; i < bgmItemList.Count; i++)
        {
            bgmItemList[i].SelectOnReset(i == index);
        }
        
        DropDownOpen(false);

        BGMPlay(currentSelectID);

        new RoomRequest()
        {
            requestStatus = 3,
            uid = MeumDB.Get().GetToken(),
            bgm_type_int = currentSelectID,
        }.RequestOn();

        MeumDB.Get().currentRoomInfo.bgm_type_int = currentSelectID;

    }


    void BGMDataSet()
    {
        List<int> hideIDList = new List<int>()
        {
            4
        };

        MeumSaveData meumSaveData = Resources.Load<MeumSaveData>("MeumSaveData");

        List<BGMSaveData> bgmSaveDataList = meumSaveData.bgmDataList;

        for (int i = 1; i < bgmSaveDataList.Count; i++)
        {
            int id = bgmSaveDataList[i].bgmId;

            if (hideIDList.Contains(id) == false)
            {
                activeBGMList.Add(bgmSaveDataList[i]);
            }
        }

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

    void DropDownItemSet() 
    {
        for (int i = 0; i < activeBGMList.Count; i++)
        {
            BGMItem bGMItem = Instantiate(itemIns, scrollRect.content);

            bGMItem.DataSet(activeBGMList[i] , i, SelectOnDataSet , PlayOnReset);

            bGMItem.gameObject.SetActive(true);

            bgmItemList.Add(bGMItem);
        
        }

        float itemSize_y = itemIns.GetComponent<RectTransform>().sizeDelta.y;

        RectTransform rect = scrollRect.GetComponent<RectTransform>();
        Vector2 pos = rect.sizeDelta;
        pos.y = itemSize_y * activeBGMList.Count + 10;
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

            BGMPlay(activeBGMList[index].bgmId);
        }
        else
        {
            bgmItemList[index].PlayOnReset(false);
            bgmItemList[currentSelectIndex].PlayOnReset(true);
            BGMPlay(activeBGMList[currentSelectIndex].bgmId);
        }
        
    }

    void BGMPlay(int index)
    {
        SoundManager.Instance.PlayBGM(index);
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
