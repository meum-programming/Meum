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
        LoginRequest();
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

}


