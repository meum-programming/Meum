using System;
using System.Collections;
using System.Collections.Generic;
using Global.Socket;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Gallery
{
    public class CircleSelectorActions : MonoBehaviour
    {
        public void GoShop()
        {
            MeumSocket.Get().LeaveToShop();
        }

        public void GoRoomBuilder()
        {
            MeumSocket.Get().LeaveToEdit();
        }
    }
}