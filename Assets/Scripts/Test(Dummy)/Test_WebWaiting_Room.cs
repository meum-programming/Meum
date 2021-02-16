using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_WebWaiting_Room : MonoBehaviour
{
    [SerializeField] WebHandler webHandler;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TokenSetBtnClick()
    {
        //webHandler.SetToken("eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJ1c2VyX2lkIjoxNDcsInVzZXJuYW1lIjoiaGFueWFuZzA3IiwiZXhwIjoxNjEzNDYzNjAxLCJlbWFpbCI6ImhhbnlhbmcwN0BuYXZlci5jb20iLCJvcmlnX2lhdCI6MTYxMzQ1NjQwMX0.66bNLFY-gXy_4EwG0qj3tQY8jPqa3McM0ouF86xrbuI");
        webHandler.SetToken("1");
    }

    public void RoomSetBtnClick()
    {
        webHandler.EnterRoom("violetlemon2020");
    }
}
