using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SquareEnterButton : MonoBehaviour
{
    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(Enter);
    }

    private void Enter()
    {
        Global.Socket.MeumSocket.Get().EnterSquare();
    }
}
