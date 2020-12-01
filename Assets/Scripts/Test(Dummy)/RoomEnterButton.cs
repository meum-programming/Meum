using UnityEngine;
using UnityEngine.UI;

namespace Gallery.MultiPlay
{
    public class RoomEnterButton : MonoBehaviour
    {
        private InputField nickname;

        private void Start()
        {
            nickname = GameObject.Find("InputField_Nickname").GetComponent<InputField>();
            GetComponent<Button>().onClick.AddListener(Enter);
        }

        private void Enter()
        {
            Core.Socket.MeumSocket.Get().EnterGallery(nickname.text);
        }
    }
}
