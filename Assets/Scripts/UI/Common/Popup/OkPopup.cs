using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class OkPopup : MonoBehaviour
{
    [SerializeField] Text contensText;
    [SerializeField] Text okBtnText;
    [SerializeField] Text cancelBtnText;

    UnityAction okBtnClickEvent;
    UnityAction cancelBtnClickEvent;
    UnityAction destoryBtnClickEvent;

    bool clickOn = false;
    float clickDelay = 1;

    bool maskClickDestoryOn = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (clickOn == false)
        {
            clickDelay -= Time.deltaTime;
            if (clickDelay < 0)
            {
                clickOn = true;
                clickDelay = 0.5f;
            }
        }
    }

    public void DataSet(string contensStr, UnityAction okBtnClickEvent , UnityAction cancelBtnClickEvent , bool maskClickDestoryOn, string okBtnStr, string cancelBtnStr)
    {
        this.okBtnClickEvent = okBtnClickEvent;
        this.cancelBtnClickEvent = cancelBtnClickEvent;

        this.maskClickDestoryOn = maskClickDestoryOn;

        contensText.text = contensStr;
        okBtnText.text = okBtnStr;
        cancelBtnText.text = cancelBtnStr;

        clickOn = false;
        clickDelay = 0.5f;

        
    }

    public void OkBtnClick()
    {
        if (clickOn == false)
            return;
        
        if (okBtnClickEvent != null)
        {
            okBtnClickEvent();
        }
        DestroyPopup();
    }
    public void CancelBtnClick()
    {
        if (clickOn == false)
            return;

        if (cancelBtnClickEvent != null)
        {
            cancelBtnClickEvent();
        }
        DestroyPopup();
    }

    public void MaskClick()
    {
        if (clickOn == false)
            return;

        if (maskClickDestoryOn == false)
        {
            if (cancelBtnClickEvent != null)
            {
                cancelBtnClickEvent();
            }
        }
        
        DestroyPopup();
    }

    void DestroyPopup()
    {
        Destroy(gameObject);
    }

}
