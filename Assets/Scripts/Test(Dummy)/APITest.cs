using Game.Artwork;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class APITest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //LoginRequest();
        Test();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoginRequest()
    {
        UserInfoRequest userInfoRequest = new UserInfoRequest()
        {
            requestStatus = 1,
            //uid = 2,
            nickName = "test",
            successOn = ResultData =>
            {
                UserInfoRespons data = (UserInfoRespons)ResultData;

            }
        };
        userInfoRequest.RequestOn();
    }

    void Test()
    {
        //string content = "\"position\":{ \"x\":-7.0,\"y\":2.0,\"z\":3.30},\"eulerAngle\":{ \"x\":90.0,\"y\":270.0,\"z\":0.0},\"scale\":{ \"x\":3.7,\"y\":0.5,\"z\":1.79}";
        //string content = "\"position\":{ \"x\":-7.0,\"y\":2.0,\"z\":3.30},\"eulerAngle\":{\"x\":90.0,\"y\":270.0,\"z\":0.0}";
        //string testStr = "{ \"artworks\" : {" + content + "}}";
        string testStr = "{ \"position\" : {\"x\":-7.0,\"y\":2.0,\"z\":3.30}}";




        Debug.LogWarning(testStr);
        

        var artworksData = JsonUtility.FromJson<StartPointData>(testStr);

        

        //Vector3 loadData = JsonUtility.FromJson<Vector3>(testStr);
        Debug.LogWarning("loadData = " + artworksData.position);


        StartPointData startPointData = new StartPointData();

        startPointData.position = Vector3.one;
        startPointData.eulerAngle = Vector3.one;

        Debug.LogWarning(JsonUtility.ToJson(startPointData));


    }
}


