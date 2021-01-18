using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.UserList
{
    public class UserListContent : MonoBehaviour
    {
        [SerializeField] private RawImage profileImage;
        [SerializeField] private TextMeshProUGUI userName;

        public void Setup(string name)
        {
            userName.text = name;
        }
    }
}