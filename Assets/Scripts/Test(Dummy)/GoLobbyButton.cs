using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GoLobbyButton : MonoBehaviour
{
    private void Awake()
    {
        var button = GetComponent<Button>();
        var serverSelector = GameObject.FindWithTag("SocketIO").GetComponent<Gallery.MultiPlay.ServerSelector>();
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(serverSelector.Quit);
    }
}

