using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 룸 정보
/// </summary>
public class RoomRequest : BaseRequest
{
    public int id;
    public int uid;
    public string nickName;
    public int sky_type_int;
    public int bgm_type_int;
    public string data_json = string.Empty;
    public override void RequestOn()
    {
        requestType = RequestType.POST;

        //id로 룸 정보 검색
        if (requestStatus == 0)
        {
            classValue = "roomById";
            form.AddField("roomId", id);
        }
        //uid로 룸 정보 검색
        else if (requestStatus == 1)
        {
            classValue = "roomOwner";
            form.AddField("uid", uid);
        }
        //skyBox 수정
        else if (requestStatus == 2)
        {
            classValue = "roomSkyChange";
            form.AddField("uid", uid);
            form.AddField("sky_type_int", sky_type_int);
        }
        //BGM 수정
        else if (requestStatus == 3)
        {
            classValue = "roomBgmChange";
            form.AddField("uid", uid);
            form.AddField("bgm_type_int", bgm_type_int);
        }
        //artwork 수정
        else if (requestStatus == 4)
        {
            classValue = "roomArtWorkChange";
            form.AddField("uid", uid);
            form.AddField("data_json", data_json);
        }


        base.RequestOn();
    }

    public override void ResponsOn(string jsonData)
    {
        BaseRespons data = null;

        data = GetData<RoomInfoRespons>(jsonData);

        if (successOn != null)
        {
            successOn(data);
        }
    }

}

public class RoomInfoRespons : BaseRespons
{
    public RoomInfoData result = new RoomInfoData();
}


[System.Serializable]
public class RoomInfoData : BaseRespons
{
    public int max_people = 0;

    public int type_int = 0;
    public int sky_type_int = 0;
    public string sky_addValue_string = string.Empty;
    public int bgm_type_int = 0;
    public string bgm_addValue_string = string.Empty;
    public string data_json = string.Empty;
    public UserData owner = null;
    public float startPos_x = -10;
    public float startPos_z = 0;
}