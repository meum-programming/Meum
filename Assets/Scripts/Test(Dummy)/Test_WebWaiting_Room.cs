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
        webHandler.SetToken("1");
    }

    public void RoomSetBtnClick()
    {
        webHandler.EnterRoom("violetlemon2020");
    }
}
