using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BGMWave : MonoBehaviour
{
    bool colorOn = false;

    public RectTransform rect;
    [SerializeField] private Image imageObj;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetSize(float value)
    {
        Vector2 sizeDelta = rect.sizeDelta;
        sizeDelta.y = value;
        rect.sizeDelta = sizeDelta;
    }


    public void ColorChange(bool colorOn)
    {
        if (this.colorOn == colorOn)
            return;

        this.colorOn = colorOn;

        SetColor();
    }

    void SetColor()
    {
        Color color = colorOn ? new Color32(57, 168, 231, 255) : new Color32(196, 196, 196, 255);
        imageObj.color = color;
    }
}
