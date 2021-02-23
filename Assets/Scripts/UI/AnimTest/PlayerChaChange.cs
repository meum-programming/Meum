using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerChaChange : MonoBehaviour
{
    [SerializeField] List<Transform> cha_0_part = new List<Transform>();
    [SerializeField] List<Transform> cha_1_part = new List<Transform>();
    [SerializeField] List<Transform> cha_2_part = new List<Transform>();
    [SerializeField] List<Transform> cha_3_part = new List<Transform>();
    [SerializeField] List<Transform> cha_4_part = new List<Transform>();

    List<List<Transform>> chaPartList = new List<List<Transform>>();

    [SerializeField] List<Transform> hairObjList = new List<Transform>();
    [SerializeField] List<Transform> maskObjList = new List<Transform>();

    public PlayerChaStatus currentChaStatus;

    
    public int hairIndex = 0;
    public int maskIndex = 0;
    public int dressIndex = 0;

    [SerializeField] Text hairName;
    [SerializeField] Text maskName;
    [SerializeField] Text dressName;

    

    // Start is called before the first frame update
    void Awake()
    {
        Init();
    }

    public void Init()
    {
        chaPartList = new List<List<Transform>>() 
        {
            cha_0_part,
            cha_1_part,
            cha_2_part,
            cha_3_part,
            cha_4_part,

        };

        
    }

    public void AllChangeData(ChaCustomizingSaveData data)
    {
        hairIndex = data.hairIndex;
        maskIndex = data.maskIndex;
        dressIndex = data.dressIndex;

        PlayerHairChangeOn(hairIndex);
        PlayerMaskChangeOn(maskIndex);
        PlayerChaChangeOn(dressIndex);
    }



    // Update is called once per frame
    void Update()
    {
           
    }

    public void DrassIndexChangeBtnClick(bool next)
    {
        if (next)
        {
            dressIndex++;

            if (dressIndex == chaPartList.Count)
            {
                dressIndex = 0;
            }
        }
        else
        {
            dressIndex--;

            if (dressIndex < 0)
            {
                dressIndex = chaPartList.Count - 1;
            }

        }

        PlayerChaChangeOn(dressIndex);
    }

    public void PlayerChaChangeOn(int status) 
    {
        PlayerChaChangeOn((PlayerChaStatus)status);
    }

    public void PlayerChaChangeOn(PlayerChaStatus playerChaStatus)
    {
        currentChaStatus = playerChaStatus;

        for (int i = 0; i < chaPartList.Count; i++)
        {
            List<Transform> cha_partList = chaPartList[i];

            for (int z = 0; z < cha_partList.Count; z++)
            {
                cha_partList[z].gameObject.SetActive(i == (int)currentChaStatus);
            }
            
        }


        string dressStr = "";

        switch ((int)currentChaStatus)
        {
            case 0:
                dressStr = "단정한 복장";
                break;
            case 1:
                dressStr = "작업자 복장";
                break;
            case 2:
                dressStr = "따뜻한 복장";
                break;
            case 3:
                dressStr = "정중한 복장";
                break;
            case 4:
                dressStr = "시원한 복장";
                break;
        }
        dressName.text = dressStr;
    }



    public void HairIndexChangeBtnClick(bool next)
    {
        if (next)
        {
            hairIndex++;

            if (hairIndex == hairObjList.Count)
            {
                hairIndex = 0;
            }
        }
        else
        {
            hairIndex--;

            if (hairIndex < 0)
            {
                hairIndex = hairObjList.Count - 1;
            }

        }

        PlayerHairChangeOn(hairIndex);
    }

    public void PlayerHairChangeOn(int status)
    {
        hairIndex = status;

        for (int i = 0; i < hairObjList.Count; i++)
        {
            hairObjList[i].gameObject.SetActive(i == hairIndex);
        }

        string hairStr = "";

        switch (hairIndex)
        {
            case 0: 
                hairStr = "곱슬머리 헤어"; 
                break;
            case 1:
                hairStr = "꽁지머리 헤어";
                break;
            case 2:
                hairStr = "깔끔한 헤어";
                break;
            case 3:
                hairStr = "단정한 헤어";
                break;
            case 4:
                hairStr = "포니 헤어";
                break;
            case 5:
                hairStr = "러프 헤어";
                break;
            case 6:
                hairStr = "정열적인 헤어";
                break;
        }
        hairName.text = hairStr;
    }

    public void MaskIndexChangeBtnClick(bool next)
    {
        if (next)
        {
            maskIndex++;

            if (maskIndex == maskObjList.Count)
            {
                maskIndex = 0;
            }
        }
        else
        {
            maskIndex--;

            if (maskIndex < 0)
            {
                maskIndex = maskObjList.Count - 1;
            }

        }

        PlayerMaskChangeOn(maskIndex);
    }
    public void PlayerMaskChangeOn(int status)
    {
        maskIndex = status;

        for (int i = 0; i < maskObjList.Count; i++)
        {
            maskObjList[i].gameObject.SetActive(i == maskIndex);
        }

        string maskStr = "";

        switch (maskIndex)
        {
            case 0:
                maskStr = "늑대 가면";
                break;
            case 1:
                maskStr = "토끼 가면";
                break;
        }
        maskName.text = maskStr;
    }




}

public enum PlayerChaStatus
{
    PlayerCha_0 = 0,
    PlayerCha_1,
    PlayerCha_2,
    PlayerCha_3,
    PlayerCha_4,
}
