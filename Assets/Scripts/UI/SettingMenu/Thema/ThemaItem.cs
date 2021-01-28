using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ThemaItem : MonoBehaviour
{
    string name;
    [SerializeField] TextMeshProUGUI nameText;
    
    [SerializeField] Image iconImage;


    int index = 0;
    UnityAction<int> choisOnEvent = null;

    bool selectOn = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DataSet(string name, int index, UnityAction<int> choisOnEvent)
    {
        this.name = name;
        nameText.text = this.name;
        this.index = index;
        this.choisOnEvent = choisOnEvent;
    }

    public void ChoisSkyOn()
    {
        if (choisOnEvent != null)
        {
            choisOnEvent(index);
        }

    }

    public void SelectOnReset(bool selectOn)
    {
        this.selectOn = selectOn;

        Color32 cor = this.selectOn ? new Color32(55, 115, 55, 255) : new Color32(255, 255, 255, 255);

        nameText.color = cor;
        iconImage.color = cor;

        iconImage.gameObject.SetActive(selectOn);

    }



}
