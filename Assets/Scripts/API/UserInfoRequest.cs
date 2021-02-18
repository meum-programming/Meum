using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 유저 정보
/// </summary>
public class UserInfoRequest : BaseRequest
{
    public int uid;
    public string nickName;

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
}