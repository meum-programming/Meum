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
        TokenSetBtnClick();
        RoomSetBtnClick();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TokenSetBtnClick()
    {
        //webHandler.SetToken("61");
        //webHandler.SetToken("59");
        //webHandler.SetToken("149");
        //webHandler.SetToken("1");
        //webHandler.SetToken("157");
        //webHandler.SetToken("242");
        //webHandler.SetToken("238");
        webHandler.SetToken("8");

    }

    public void RoomSetBtnClick()
    {
        //webHandler.EnterRoom("Guest");
        //webHandler.EnterRoom("믐대표서비스");
        //webHandler.EnterRoom("blackgallery");
        //webHandler.EnterRoom("string");
        //webHandler.EnterRoom("the_other_layers");
        //webHandler.EnterRoom("sangminTest");
        //webHandler.EnterRoom("unseenland");
        webHandler.EnterRoom("UGKIM");
    }
}
