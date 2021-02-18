using Core;
using Core.Socket;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class WebHandler : MonoBehaviour
{
    private IEnumerator _waitingCoroutine = null;
    
    public void SetToken(string token)
    {
        Debug.Log(token);
        Debug.Log("token has set");
        MeumDB.Get().SetToken(token);
    }

    public void EnterRoom(string nickname)
    {
        nickname = UnityWebRequest.UnEscapeURL(nickname);
        Debug.Log(nickname);
        if (!MeumDB.Get().TokenExist()) Debug.LogError("SetToken first");
        MeumSocket.Get().EnterGallery(nickname);
    }

    public void EnterSquare()
    {
        if (!MeumDB.Get().TokenExist()) Debug.LogError("SetToken first");
        MeumSocket.Get().EnterSquare();
    }
}
