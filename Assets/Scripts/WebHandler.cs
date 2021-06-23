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
#if dev
        Debug.Log("token = " + token);
#endif
        MeumDB.Get().SetToken(token);
    }

    public void EnterRoom(string nickname)
    {
        nickname = UnityWebRequest.UnEscapeURL(nickname);

#if dev
        Debug.Log("nickname join = " + nickname);
#endif

        if (!MeumDB.Get().TokenExist()) Debug.LogError("SetToken first");
        MeumSocket.Get().EnterGallery(nickname);
    }

    public void EnterRoomFromID(int roomId)
    {
#if dev
        Debug.Log("roomID join = "+roomId);
#endif

        if (!MeumDB.Get().TokenExist()) Debug.LogError("SetToken first");
        MeumSocket.Get().EnterGalleryFromRoomID(roomId);
    }

    public void EnterSquare()
    {
        if (!MeumDB.Get().TokenExist()) Debug.LogError("SetToken first");
        MeumSocket.Get().EnterSquare();
    }
}
