using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 아트워크 정보
/// </summary>
public class ArtWorkRequest : BaseRequest
{
    public int id = 0;
    public int uid = 0;
    public string content = string.Empty;

    public override void RequestOn()
    {
        requestType = RequestType.POST;
        
        //id로 아트워크 정보 검색
        if (requestStatus == 0)
        {
            classValue = "artWorkById";
            
            form.AddField("id", id);
        }
        //ownerid로 아트워크 정보 검색
        else if (requestStatus == 1)
        {
            classValue = "artWorkByOwnerId";

            form.AddField("uid", uid);
        }
        //ownerid로 상점에서 구매한 아트워크 정보 검색
        else if (requestStatus == 2)
        {
            classValue = "shopBuyByOwnerId";

            form.AddField("uid", uid);
        }
        //id로 아트워크 댓글 검색
        else if (requestStatus == 3)
        {
            classValue = "getArtworkComment";

            form.AddField("artwork_id", id);
        }
        //아트워크 댓글 작성
        else if (requestStatus == 4)
        {
            classValue = "insertArtworkComment";

            form.AddField("artwork_id", id);
            form.AddField("content", content);
            form.AddField("writer_id", uid);
        }
        //아트워크 댓글 삭제
        else if (requestStatus == 5)
        {
            classValue = "deleteArtworkComment";

            form.AddField("comment_id", id);
        }

        base.RequestOn();
    }

    public override void ResponsOn(string jsonData)
    {
        BaseRespons data = null;

        if (requestStatus == 0)
        {
            data = GetData<ArtWorkResponsOnlyOne>(jsonData);
        }
        else if (requestStatus == 1)
        {
            data = GetData<ArtWorkRespons>(jsonData);
        }
        else if (requestStatus == 2)
        {
            data = GetData<ShopByArtWorkRespons>(jsonData);
        }
        else if (requestStatus == 3)
        {
            data = GetData<ArtWorkCommentRespons>(jsonData);
        }
        else if (requestStatus == 4)
        {
            data = GetData<ArtWorkCommentOnlyOneRespons>(jsonData);
        }
        else if (requestStatus == 5)
        {
            data = GetData<BaseRespons>(jsonData);
        }

        if (successOn != null)
        {
            successOn(data);
        }
    }

}

public class ArtWorkRespons : BaseRespons
{
    public List<ArtWorkData> result = new List<ArtWorkData>();
}
public class ArtWorkResponsOnlyOne : BaseRespons
{
    public ArtWorkData result = new ArtWorkData();
}
public class ShopByArtWorkRespons : BaseRespons
{
    public List<ShopByArtWorkData> result = new List<ShopByArtWorkData>();
}

public class ArtWorkCommentRespons : BaseRespons
{
    public List<ArtWorkCommentData> result = new List<ArtWorkCommentData>();
}

public class ArtWorkCommentOnlyOneRespons : BaseRespons
{
    public ArtWorkCommentData result = null;
}


[System.Serializable]
public class ShopByArtWorkData : BaseRespons
{
    public int price = 0;
    public int artwork_id = 0;
    public int buyer_id = 0;
    public ArtWorkData artwork = null;
}

[System.Serializable]
public class ArtWorkCommentData : BaseRespons
{
    public string content = string.Empty;
    public int artwork_id = 0;
    public int writer_id = 0;
    public UserData owner = null;
}


[System.Serializable]
public class ArtWorkData : BaseRespons
{
    public int type_artwork = 0;
    public int price = 0;
    public int owner_id = 0;
    public string instruction = string.Empty;
    public int size_h = 0;
    public int size_w = 0;
    public string title = string.Empty;
    public int year = 0;

    public string albedo_url = string.Empty;
    public int allow = 0;
    public string image_file = string.Empty;
    public string object_file = string.Empty;
    public string thumbnail = string.Empty;
    public bool is_owner = false;
    public bool is_sexual = false;
    public bool is_violent = false;
    public string tags = string.Empty;
    public int ccl = 0;
    public string author = string.Empty;
}