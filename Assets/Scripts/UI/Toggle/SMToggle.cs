using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SMToggle : MonoBehaviour
{
    [SerializeField] MaskableGraphic target;

    public bool value;
    
    public Color trueColor;
    public Color falseColor;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ToggleOn()
    {
        value = !value;
        SetColor();

        SMToggle[] smToggleList = GetComponentsInChildren<SMToggle>(true);

        for (int i = 0; i < smToggleList.Length; i++)
        {
            if (smToggleList[i] == this)
                continue;
            smToggleList[i].ToggleOn();
        }
    }

    void SetColor()
    {
        target.color = value ? trueColor : falseColor;
    }


}
