using System;
using System.Collections;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnitySocketIO;

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
            MeumSocket.Get().EnterRoom(nickname.text);
        }
    }
}
