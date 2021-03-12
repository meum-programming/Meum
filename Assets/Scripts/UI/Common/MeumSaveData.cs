using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MeumSaveData : ScriptableObject
{
    public List<BGMSaveData> bgmDataList = new List<BGMSaveData>();
    public List<SkyBoxSaveData> skyDataList = new List<SkyBoxSaveData>();
    public List<GestureModel> gestureModelList = new List<GestureModel>();

    public BGMSaveData GetBGMData(int bgmId)
    {
        foreach (var bgmData in bgmDataList)
        {
            if (bgmData.bgmId == bgmId)
            {
                return bgmData;
            }
        }

        return null;
    }
    public SkyBoxSaveData GetSKYData(SkyBoxEnum skyEnumValue)
    {
        foreach (var skyData in skyDataList)
        {
            if (skyData.skyBoxEnum == skyEnumValue)
            {
                return skyData;
            }
        }

        return null;
    }

}

[Serializable]
public class BGMSaveData
{
    public int bgmId;
    public string name;
    public AudioClip audioClip;
}

public enum BGMEnum
{
    None = -1,
    BGM_0,
    BGM_1,
    BGM_2,
    BGM_3,
}


[Serializable]
public class SkyBoxSaveData
{
    public SkyBoxEnum skyBoxEnum;
    public string name;
    public Material material;
}

public enum SkyBoxEnum
{
    None = -1,
    SkyBox_0,
    SkyBox_1,
    SkyBox_2,
    SkyBox_3,
    SkyBox_4,
}


[Serializable]
public class GestureModel
{
    public int id = 0;
    public string name = string.Empty;
    public Sprite sprite = null;


    public GestureModel() { }
    public GestureModel(int id, string name)
    {
        this.id = id;
        this.name = name;
    }

}
