using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 제스쳐 정보
/// </summary>
public class GestureRequest : BaseRequest
{
    public int uid = 0;
    public string userSaveData = string.Empty;

    public override void RequestOn()
    {
        //이모지 셋팅 값 불러오기
        if (requestStatus == 0)
        {
            baseUri = "https://node.meum.me/";
            classValue = "users/";
            getValue = "emoji/" + uid;
            requestType = RequestType.Get;
        }
        //이모지 셋팅
        else if (requestStatus == 1)
        {
            baseUri = "https://node.meum.me/";
            classValue = "users/";
            getValue = "emoji/";
            requestType = RequestType.POST;

            form.AddField("uid", uid);
            form.AddField("setting", userSaveData);

        }

        base.RequestOn();
    }

    public override void ResponsOn(string jsonData)
    {
        BaseRespons data = null;

        data = GetData<GestureData>(jsonData);

        if (successOn != null)
        {
            successOn(data);
        }
    }

}


[System.Serializable]
public class GestureData : BaseRespons
{
    public int status = 0;
    public int uid = 0;
    public string[] data;
}
