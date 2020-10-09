using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
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
        Global.Socket.MeumSocket.Get().Return();
    }
}
