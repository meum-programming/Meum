using System;
using System.Collections;
using System.Collections.Generic;
using Gallery.MultiPlay;
using UnityEngine;
using UnityEngine.UI;

public class QuitButton : MonoBehaviour
{
    private void Awake()
    {
        var button = GetComponent<Button>();
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(Global.Socket.MeumSocket.Get().Quit);
    }
}

