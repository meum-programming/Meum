using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchCharacter : MonoBehaviour
{
    private Gallery.MultiPlay.DataSyncer _syncer;

    // Start is called before the first frame update
    private void Awake()
    {
        _syncer = GetComponent<Gallery.MultiPlay.DataSyncer>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
            Switch();
    }

    public void Switch()
    {
        var current = _syncer.currentCharId;
        _syncer.BroadCastChangeCharacter(current == 0 ? 1 : 0);
    }
}

