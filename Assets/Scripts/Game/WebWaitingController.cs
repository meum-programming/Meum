using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WebWaitingController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AskBtnClick() 
    {
        string uri = "https://meum.me/help";
#if dev
        uri = "https://dev.meum.me/help";
#endif
        Application.OpenURL(uri);
    }

}
