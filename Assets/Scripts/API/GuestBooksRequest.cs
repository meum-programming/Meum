using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 게스트 북 정보
/// </summary>
public class GuestBooksRequest : BaseRequest
{
    public int id = 0;
    public int roomId = 0;
    public int stamp_type = 0;
    public string content = string.Empty;
    public int writer_id = 0;

    public override void RequestOn()
    {
        requestType = RequestType.POST;

        //roomId로 게스트 북 정보 검색
        if (requestStatus == 0)
        {
            classValue = "getGuestBookContens";
            
            form.AddField("roomId", roomId);
        }
        //roomId로 스템프 카운트 정보 검색
        else if (requestStatus == 1)
        {
            classValue = "getGuestBookStempCount";

            form.AddField("roomId", roomId);
        }
        //게스트 북 내용 작성
        else if (requestStatus == 2)
        {
            classValue = "insertGuestBookContens";

            form.AddField("roomId", roomId);
            form.AddField("stamp_type", stamp_type);
            form.AddField("content", content);
            form.AddField("writer_id", writer_id);
        }
        //게스트 북 내용 삭제
        else if (requestStatus == 3)
        {
            classValue = "deleteGuestBookContens";

            form.AddField("guestBookId", id);
        }

        base.RequestOn();
    }

    public override void ResponsOn(string jsonData)
    {
        BaseRespons data = null;

        if (requestStatus == 0)
        {
            data = GetData<GuestBooksRespons>(jsonData);
        }
        else if (requestStatus == 1)
        {
            data = GetData<GuestBooksStempRespons>(jsonData);
        }
        else if (requestStatus == 2 || requestStatus == 3)
        {
            data = GetData<BaseRespons>(jsonData);
        }

        if (successOn != null)
        {
            successOn(data);
        }
    }

}

public class GuestBooksRespons : BaseRespons
{
    public List<GuestBooksData> result = new List<GuestBooksData>();
}

public class GuestBooksStempRespons : BaseRespons
{
    public GuestBooksStempData result = null;
}


[System.Serializable]
public class GuestBooksData : BaseRespons
{
    public int stamp_type = 0;
    public string content = string.Empty;
    public int room_id = 0;
    public int writer_id = 0;
    public UserData owner = null;
}


[System.Serializable]
public class GuestBooksStempData : BaseRespons
{
    public int one_queryset = 0;
    public int two_queryset = 0;
    public int three_queryset = 0;
    public int four_queryset = 0;
}
