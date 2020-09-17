using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WebHandler : MonoBehaviour
{
    // private void Start()
    // {
    //     Application.Ext
    // }

    public void SetToken(string token)
    {
        MeumDB.Get().SetToken(token);
    }

    public void EnterRoom(string nickname)
    {
        if (!MeumDB.Get().TokenExist()) Debug.LogError("SetToken first ");
        MeumSocket.Get().EnterRoom(nickname);
    }

    public void EnterSquare()
    {
        if (!MeumDB.Get().TokenExist()) Debug.LogError("SetToken first ");
    }
}
