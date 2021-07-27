using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoThumbnailRequest : BaseRequest
{
    public int uid = 0;
    public bool is_auto_thumbnail = true;

    public override void RequestOn()
    {
        baseUri = "https://node.meum.me/";
        classValue = "room/";
        getValue = string.Format("{0}/auto-thumbnail", uid);

        //이모지 셋팅 값 불러오기
        if (requestStatus == 0)
        {
            requestType = RequestType.Get;
        }
        //이모지 셋팅
        else if (requestStatus == 1)
        {
            requestType = RequestType.POST;
            string settingValue = is_auto_thumbnail ? "true" : "false";
            form.AddField("setting", settingValue);
        }

        base.RequestOn();
    }

    public override void ResponsOn(string jsonData)
    {
        BaseRespons data = null;

        data = GetData<AutoThumbnailData>(jsonData);

        if (successOn != null)
        {
            successOn(data);
        }
    }
}
public class AutoThumbnailData : BaseRespons
{
    public int status = 0;
    public bool result = false;
}