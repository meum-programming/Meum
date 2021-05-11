using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerChaChange : MonoBehaviour
{
    private List<Transform> cha_0_part = new List<Transform>();
    private List<Transform> cha_1_part = new List<Transform>();
    private List<Transform> cha_2_part = new List<Transform>();
    private List<Transform> cha_3_part = new List<Transform>();
    private List<Transform> cha_4_part = new List<Transform>();
    private List<Transform> cha_5_part = new List<Transform>();
    private List<Transform> cha_6_part = new List<Transform>();
    private List<Transform> cha_7_part = new List<Transform>();

    List<List<Transform>> chaPartList = new List<List<Transform>>();

    private List<Transform> hairObjList = new List<Transform>();
    private List<Transform> maskObjList = new List<Transform>();
    private List<SkinnedMeshRenderer> skinMesh = new List<SkinnedMeshRenderer>();
    private List<SkinnedMeshRenderer> hairMesh = new List<SkinnedMeshRenderer>();

    public PlayerChaStatus currentChaStatus;

    public int hairIndex = 0;
    public int maskIndex = 0;
    public int dressIndex = 0;
    int skinIndex = 1;

    public ChaCustomizingSaveData chaCustomizingSaveData = null;

    public bool localPlayer = false;

    private bool meshActiveOn = true;

    // Start is called before the first frame update
    void Awake()
    {
        Init();
    }

    public void Init()
    {
        SkinMetarialSet();
        GetChaCustomizingSaveData();
    }

    /// <summary>
    /// 피부색을 담당하는 메테리얼 세팅
    /// </summary>
    void SkinMetarialSet()
    {
        skinMesh = new List<SkinnedMeshRenderer>();

        cha_0_part = new List<Transform>();
        cha_1_part = new List<Transform>();
        cha_2_part = new List<Transform>();
        cha_3_part = new List<Transform>();
        cha_4_part = new List<Transform>();
        cha_5_part = new List<Transform>();
        cha_6_part = new List<Transform>();
        cha_7_part = new List<Transform>();

        hairObjList = new List<Transform>();
        hairMesh = new List<SkinnedMeshRenderer>();
        maskObjList = new List<Transform>();

        for (int i = 0; i < transform.childCount; i++)
        {
            Transform childObj = transform.GetChild(i);

            if (childObj.name.Contains("clothes_1"))
            {
                cha_0_part.Add(childObj);
            }
            else if (childObj.name.Contains("clothes_2"))
            {
                cha_1_part.Add(childObj);
            }
            else if (childObj.name.Contains("clothes_3"))
            {
                cha_2_part.Add(childObj);
            }
            else if (childObj.name.Contains("clothes_4"))
            {
                cha_3_part.Add(childObj);
            }
            else if (childObj.name.Contains("clothes_5"))
            {
                cha_4_part.Add(childObj);
            }
            else if (childObj.name.Contains("clothes_6"))
            {
                cha_5_part.Add(childObj);
            }
            else if (childObj.name.Contains("clothes_7"))
            {
                cha_6_part.Add(childObj);
            }
            else if (childObj.name.Contains("clothes_8"))
            {
                cha_7_part.Add(childObj);
            }
            else if (childObj.name.Contains("hair_"))
            {
                hairObjList.Add(childObj);
                hairMesh.Add(childObj.GetComponent<SkinnedMeshRenderer>());
            }
            else if (childObj.name.Contains("face_"))
            {
                maskObjList.Add(childObj);
            }

            else if (childObj.name.Contains("person_model_"))
            {
                skinMesh.Add(childObj.GetComponent<SkinnedMeshRenderer>());
            }
        }

        chaPartList = new List<List<Transform>>()
        {
            cha_0_part,
            cha_1_part,
            cha_2_part,
            cha_3_part,
            cha_4_part,
            cha_5_part,
            cha_6_part,
            cha_7_part,
        };
    }

    public void GetChaCustomizingSaveData()
    {
        chaCustomizingSaveData = DataManager.Instance.chaCustomizingSaveData;
        AllChangeData(chaCustomizingSaveData);
    }

    public void AllChangeData(ChaCustomizingSaveData data)
    {
        SkinColorSet(data.skinIndex);
        PlayerHairChangeOn(data.hairIndex);
        PlayerMaskChangeOn(data.maskIndex);
        PlayerChaChangeOn(data.dressIndex);
    }



    // Update is called once per frame
    void Update()
    {
           
    }

    /// <summary>
    /// 피부색 변경
    /// </summary>
    public void SkinColorSet(int currentSkinStatus)
    {
        //if (this.skinIndex == currentSkinStatus)
          //  return;
        
        this.skinIndex = currentSkinStatus;

        for (int i = 0; i < skinMesh.Count; i++)
        {
            skinMesh[i].material.color = GetSkinColor();
        }


        for (int i = 0; i < hairMesh.Count; i++)
        {
            hairMesh[i].material.color = GetHairColor();
        }
    }

    private Color GetHairColor()
    {
        string hexCode = "#625F5E";

        switch (skinIndex)
        {
            case 0:
                hexCode = "#625F5E";
                break;
            case 1:
                hexCode = "#35434F";
                break;
            case 2:
                hexCode = "#101010";
                break;
            case 3:
                hexCode = "#FFFEF4";
                break;
            case 4:
                hexCode = "#9ABBAD";
                break;
            case 5:
                hexCode = "#EFE6E1";
                break;
            case 6:
                hexCode = "#BED0D9";
                break;
            case 7:
                hexCode = "#AE9ABB";
                break;
        }

        Color color;
        ColorUtility.TryParseHtmlString(hexCode, out color);

        return color;

    }

    private Color GetSkinColor()
    {
        string hexCode = "#F6EBE5";

        switch (skinIndex)
        {
            case 0:
                hexCode = "#F6EBE5";
                break;
            case 1:
                hexCode = "#E4C6B5";
                break;
            case 2:
                hexCode = "#B38B7D";
                break;
            case 3:
                hexCode = "#4C3530";
                break;
            case 4:
                hexCode = "#E7D3E4";
                break;
            case 5:
                hexCode = "#7C292B";
                break;
            case 6:
                hexCode = "#969EB0";
                break;
            case 7:
                hexCode = "#6652CA";
                break;
        }

        Color color;
        ColorUtility.TryParseHtmlString(hexCode, out color);

        return color;

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
        //if (currentChaStatus == playerChaStatus)
          //  return;
        
        currentChaStatus = playerChaStatus;

        for (int i = 0; i < chaPartList.Count; i++)
        {
            List<Transform> cha_partList = chaPartList[i];

            for (int z = 0; z < cha_partList.Count; z++)
            {
                cha_partList[z].gameObject.SetActive(i == (int)currentChaStatus);
            }
            
        }
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
        //if (hairIndex == status)
          //  return;
        
        hairIndex = status;

        for (int i = 0; i < hairObjList.Count; i++)
        {
            hairObjList[i].gameObject.SetActive(i == hairIndex);
        }
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
        //if (maskIndex == status)
          //  return;
       
        maskIndex = status;

        for (int i = 0; i < maskObjList.Count; i++)
        {
            maskObjList[i].gameObject.SetActive(i == maskIndex);
        }   
    }
    
    public void SkinnedMeshRendererActiveSet(bool meshActiveOn)
    {
        if (this.meshActiveOn == meshActiveOn)
            return;
        
        this.meshActiveOn = meshActiveOn;

        foreach (var smr in GetComponentsInChildren<SkinnedMeshRenderer>())
        {
            smr.enabled = meshActiveOn;
        }
        

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
