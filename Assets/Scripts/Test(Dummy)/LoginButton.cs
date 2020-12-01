using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoginButton : MonoBehaviour
{
    public InputField email;

    public InputField pwd;

    public void Login()
    {
        StartCoroutine(LoginCoroutine());
    }
    private IEnumerator LoginCoroutine()
    {
        var cd = new CoroutineWithData(this, Core.MeumDB.Get().Login(email.text, pwd.text));
        yield return cd.coroutine;
        var result = Convert.ToBoolean(cd.result);
        if (result)
            SceneManager.LoadScene("Lobby");
        else
            Debug.LogError("login failed");
    }
}
