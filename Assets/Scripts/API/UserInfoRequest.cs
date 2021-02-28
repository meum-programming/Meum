using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 유저 정보
/// </summary>
public class UserInfoRequest : BaseRequest
{
    public int uid = 0;
    public string nickName = string.Empty;
    public int hairIndex = 0;
    public int maskIndex = 0;
    public int dressIndex = 0;
    public int skinIndex = 0;

    public override void RequestOn()
    {
        requestType = RequestType.Get;
        
        //uid로 유저정보 검색
        if (requestStatus == 0)
        {
            classValue = "user";
            requestType = RequestType.POST;
            
            form.AddField("uid", uid);
        }
        //닉네임으로 유저정보 검색
        else if (requestStatus == 1)
        {
            classValue = "profileByNickname";
            requestType = RequestType.POST;

            form.AddField("nickname", nickName);
        }
        //캐릭터 정보 수정
        else if (requestStatus == 2)
        {
            classValue = "updateChaData";
            requestType = RequestType.POST;

            form.AddField("uid", uid);
            form.AddField("hairIndex", hairIndex);
            form.AddField("maskIndex", maskIndex);
            form.AddField("dressIndex", dressIndex);
            form.AddField("skinIndex", skinIndex);
        }

        base.RequestOn();
    }

    public override void ResponsOn(string jsonData)
    {
        BaseRespons data = null;

        data = GetData<UserInfoRespons>(jsonData);

        if (successOn != null)
        {
            successOn(data);
        }
    }
}

public class UserInfoRespons : BaseRespons
{
    public UserData result = new UserData();
}

[System.Serializable]
public class UserData : BaseRespons
{
    public int user_id = 0;
    public string nickname = string.Empty;
    public string email = string.Empty;
    public string phone = string.Empty;
    public int cash = 0;
    public int subscribe = 0;

    public int is_tutorial = 0;
    public int is_admin = 0;
    public int max_inventory = 0;

    public int hairIndex = 0;
    public int maskIndex = 0;
    public int dressIndex = 0;
    public int skinIndex = 0;
}