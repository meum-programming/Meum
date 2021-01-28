using Core;
using Game;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ThemaSection : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI selectSkyName;

    [SerializeField] Image arrowImage;

    [SerializeField] ScrollRect scrollRect;
    [SerializeField] RectTransform scrollRectCloseBG;

    [SerializeField] ThemaItem itemIns;

    List<SkyBoxSaveData> skyDataList = new List<SkyBoxSaveData>();

    List<ThemaItem> themaItemList = new List<ThemaItem>();

    int currentSelectIndex = 0;

    bool isOwnerRoom = false;

    int defaultMaxValue = 3;

    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    void Init()
    {
        isOwnerRoom = MeumDB.Get().myRoomInfo.owner.primaryKey == MeumDB.Get().currentRoomInfo.owner.primaryKey;

        SKYDataSet();
        DropDownItemSet();

        currentSelectIndex = MeumDB.Get().currentRoomInfo.sky_type_int;

        SelectOnDataSet(currentSelectIndex);

        if (isOwnerRoom == false)
        {
            gameObject.GetComponent<Button>().interactable = false;
        }

    }

    void SKYDataSet() 
    {
        MeumSaveData meumSaveData = Resources.Load<MeumSaveData>("MeumSaveData");

        List<SkyBoxSaveData> skySaveDataList = meumSaveData.skyDataList;

        skyDataList = new List<SkyBoxSaveData>();

        for (int i = 1; i <= defaultMaxValue + 1; i++)
        {
            skyDataList.Add(skySaveDataList[i]);
        }

        string[] splitData = MeumDB.Get().currentRoomInfo.sky_addValue_string.Split(',');

        List<int> addTypeList = new List<int>();

        foreach (var data in splitData)
        {
            int addType = 0;

            int.TryParse(data, out addType);

            if (addType > defaultMaxValue)
            {
                SkyBoxSaveData skyBoxSaveData = meumSaveData.GetSKYData((SkyBoxEnum)addType);

                if (skyBoxSaveData != null && skyDataList.Contains(skyBoxSaveData) == false)
                {
                    skyDataList.Add(skyBoxSaveData);
                }
            }
        }
    }

    void DropDownItemSet()
    {
        for (int i = 0; i < skyDataList.Count; i++)
        {
            ThemaItem themaItem = Instantiate(itemIns, scrollRect.content);

            themaItem.DataSet(skyDataList[i].name, i, SelectOnDataSet);

            themaItem.gameObject.SetActive(true);

            themaItemList.Add(themaItem);
        }

        RectTransform rect = scrollRect.GetComponent<RectTransform>();
        Vector2 pos = rect.sizeDelta;
        pos.y = 30 * skyDataList.Count;
        rect.sizeDelta = pos;
    }


    void SelectOnDataSet(int index)
    {
        currentSelectIndex = index;
        selectSkyName.text = skyDataList[index].name;

        for (int i = 0; i < themaItemList.Count; i++)
        {
            themaItemList[i].SelectOnReset(i == currentSelectIndex);
        }

        DropDownOpen(false);

        StartCoroutine(MeumDB.Get().PatchRoomSKY(index));


        MeumDB.Get().currentRoomInfo.sky_type_int = currentSelectIndex;

        if (FindObjectOfType<ProceduralGalleryBuilder>() != null)
        {
            FindObjectOfType<ProceduralGalleryBuilder>().SkyBoxSet();
        }
        
    }

    public void DropDownOpen(bool active)
    {
        if (active && scrollRect.gameObject.activeSelf)
        {
            active = false;
        }

        scrollRect.gameObject.SetActive(active);
        scrollRectCloseBG.gameObject.SetActive(active);

        Quaternion rot = active ? Quaternion.Euler(180, 0, 0) : Quaternion.Euler(0, 0, 0);
        arrowImage.transform.rotation = rot;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ThemaSectionOn(int index)
    {
        StartCoroutine(MeumDB.Get().PatchRoomSKY(index));

        Debug.LogWarning("index = " + index);
    }

}
