using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchCharacter : MonoBehaviour
{
    private DataSyncer syncer;
    // Start is called before the first frame update
    private void Awake()
    {
        syncer = GetComponent<DataSyncer>();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Q)) 
            Switch();
    }

    public void Switch()
    {
        var current = syncer.currentCharId;
        syncer.ChangeCharacter(current == 0 ? 1 : 0);
    }
}
