using System;
using System.Collections;
using System.Collections.Generic;
using Global.Socket;
using UnityEngine;

public class SwitchCharacter : MonoBehaviour
{
    private int _current = 0;

    // Start is called before the first frame update
    private void Awake()
    {
        _current = 0;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
            Switch();
    }

    public void Switch()
    {
        var val = _current == 0 ? 1 : 0;
        MeumSocket.Get().BroadCastChangeCharacter(val);
        _current = val;
    }
}

