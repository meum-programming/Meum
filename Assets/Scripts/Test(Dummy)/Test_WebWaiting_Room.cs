using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

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
        //webHandler.SetToken("61");
        webHandler.SetToken("59");
    }

    public void RoomSetBtnClick()
    {
        //webHandler.EnterRoom("Guest");
        webHandler.EnterRoom("믐대표서비스");
    }
}
