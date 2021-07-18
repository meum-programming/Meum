using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenShotRequest : BaseRequest
{
    public int id;
    public string file_name;

    public enum saveTypeEnum 
    {
        screenshot,
        thumbnail
    }
    public saveTypeEnum saveType = saveTypeEnum.screenshot;
    public int screenShotType;
    public byte[] bytes;
    public override void RequestOn()
    {
        requestType = RequestType.POST;

        baseUri = "https://node.meum.me/";

        if (requestStatus == 0)
        {
            classValue = "s3";

            form.AddField("type", saveType.ToString());
            form.AddField("target", id);
            form.AddField("file_name", file_name);
            form.AddBinaryData("file", bytes);
        }

        base.RequestOn();
    }

    public override void ResponsOn(string jsonData)
    {
        BaseRespons data = null;

        data = GetData<ScreenShotInfoData>(jsonData);

        if (successOn != null)
        {
            successOn(data);
        }
    }
}

public class ScreenShotInfoRespons : BaseRespons
{
    public ScreenShotInfoData result = new ScreenShotInfoData();
}

public class ScreenShotInfoData : BaseRespons
{
    public int status;
    public string message;
    public string url;
}
