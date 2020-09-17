using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Gallery
{
    public class CircleSelectorActions : MonoBehaviour
    {
        public void GoShop()
        {
            MeumSocket.Get().TemporaryLeaveGallery();
            SceneManager.LoadScene("Shop");
        }

        public void GoRoomBuilder()
        {
            MeumSocket.Get().TemporaryLeaveGallery();
            SceneManager.LoadScene("ProceduralGalleryBuilder");
        }
    }
}