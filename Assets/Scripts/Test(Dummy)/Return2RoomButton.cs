using UnityEngine;
using UnityEngine.UI;

public class Return2RoomButton : MonoBehaviour
{
    private void Awake()
    {
        var button = GetComponent<Button>();
        button.onClick.AddListener(ButtonAction);
    }

    private void ButtonAction()
    {
        Core.Socket.MeumSocket.Get().ReturnToGalleryScene();
    }
}
