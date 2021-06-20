using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;
using UnityEngine.ResourceManagement.AsyncOperations;

public class Test_WebWaiting_Room : MonoBehaviour
{
    [SerializeField] WebHandler webHandler;

    public string loginUid;
    public int roomid;
    public string roomName;
    public bool roomJoinToID = true;
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
        //webHandler.SetToken("180");
        //webHandler.SetToken("62");
        //webHandler.SetToken("61");
        //webHandler.SetToken("60");
        //webHandler.SetToken("59");
        //webHandler.SetToken("149");
        //webHandler.SetToken("1");
        //webHandler.SetToken("157");
        //webHandler.SetToken("242");
        //webHandler.SetToken("238");
        //webHandler.SetToken("8");
        //webHandler.SetToken("268");

        webHandler.SetToken(loginUid);
    }

    public void RoomSetBtnClick()
    {
        //webHandler.EnterRoom("죽산중학교");
        //webHandler.EnterRoom("SNUART2020");
        //webHandler.EnterRoom("Guest");
        //webHandler.EnterRoom("믐MASTER");
        //webHandler.EnterRoom("Meum");
        //webHandler.EnterRoom("blackgallery");
        //webHandler.EnterRoom("string");
        //webHandler.EnterRoom("the_other_layers");
        //webHandler.EnterRoom("sangminTest");
        //webHandler.EnterRoom("unseenland");
        //webHandler.EnterRoom("UGKIM");
        //webHandler.EnterRoom("gogh");

        if (roomJoinToID)
        {
            webHandler.EnterRoomFromID(roomid);
        }
        else 
        {
            webHandler.EnterRoom(roomName);
        }
        
    }
}
