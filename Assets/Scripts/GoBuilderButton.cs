using System.Collections;
using System.Collections.Generic;
using Global.Socket;
using UnityEngine;
using UnityEngine.SceneManagement;
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
        MeumSocket.Get().LeaveToEdit();
    }
}
