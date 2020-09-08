using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class LoginButton : MonoBehaviour
{
    public InputField email;

    public InputField pwd;

    public void Login()
    {
        MeumDB.Get().Login(email.text, pwd.text);
        SceneManager.LoadScene("Lobby");
    }
}
