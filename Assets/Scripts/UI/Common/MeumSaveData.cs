using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MeumSaveData : ScriptableObject
{
    public List<BGMSaveData> bgmDataList = new List<BGMSaveData>();
    public List<SkyBoxSaveData> skyDataList = new List<SkyBoxSaveData>();

    public BGMSaveData GetBGMData(BGMEnum bgmEnumValue)
    {
        foreach (var bgmData in bgmDataList)
        {
            if (bgmData.bgmEnum == bgmEnumValue)
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
    public BGMEnum bgmEnum;
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
