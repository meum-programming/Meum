using Core.Socket;
using UnityEngine;
using UnityEngine.UI;

public class GoBuilderButton : MonoBehaviour
{
    private void Awake()
    {
        var button = GetComponent<Button>();
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(ButtonAction);
    }

    private void ButtonAction()
    {
        MeumSocket.Get().GoToEditScene();
    }
}
