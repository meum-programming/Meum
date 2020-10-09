using System;
using System.Collections;
using System.Collections.Generic;
using Global.Socket;
using UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Gallery
{
    public class CircleSelectorActions : MonoBehaviour
    {
        private CircleSelector _selector;

        private void Awake()
        {
            _selector = GetComponent<CircleSelector>();
        }

        public void GoShop()
        {
            _selector.Shrink();
            MeumSocket.Get().LeaveToShop();
        }

        public void GoRoomBuilder()
        {
            _selector.Shrink();
            MeumSocket.Get().LeaveToEdit();
        }
    }
}