using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WebHandler : MonoBehaviour
{
    private IEnumerator _waitingCoroutine = null;
    
    public void SetToken(string token)
    {
        Global.MeumDB.Get().SetToken(token);
    }

    public void EnterRoom(string nickname)
    {
        if (!Global.MeumDB.Get().TokenExist()) Debug.LogError("SetToken first");
        Global.Socket.MeumSocket.Get().EnterGallery(nickname);
    }

    public void EnterSquare()
    {
        if (!Global.MeumDB.Get().TokenExist()) Debug.LogError("SetToken first");
        Debug.Log("called entersquare");
        Global.Socket.MeumSocket.Get().EnterSquare();
    }
}
