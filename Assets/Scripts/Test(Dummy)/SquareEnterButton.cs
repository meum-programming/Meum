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
        Core.Socket.MeumSocket.Get().EnterSquare();
    }
}
