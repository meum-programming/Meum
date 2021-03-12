using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GestureDragHelper : GestureHelperBase, IPointerDownHandler,IBeginDragHandler,IEndDragHandler,IDragHandler
{
    [SerializeField] Text nameText;

    void Awake() 
    {
    }


    public void OnBeginDrag(PointerEventData eventData)
    {
        if (gestureModel == null)
            return;
        
        gestureSettingPopup.OnBeginDrag(eventData , this);
    }
    public void OnDrag(PointerEventData eventData)
    {
        if (gestureModel == null)
            return;

        gestureSettingPopup.OnDrag(eventData);
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        if (gestureModel == null)
            return;

        gestureSettingPopup.OnEndDrag(eventData);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (gestureModel == null)
            return;

        gestureSettingPopup.OnPointerDown(eventData);
    }

    public void DataSet(GestureModel gestureModel)
    {
        this.gestureModel = gestureModel;

        if (this.gestureModel == null)
        {
            nullImage.gameObject.SetActive(true);
            iconImage.gameObject.SetActive(false);

            nameText.text = "비어있음";
        }
        else
        {
            nullImage.gameObject.SetActive(false);
            iconImage.gameObject.SetActive(true);
            iconImage.sprite = gestureModel.sprite;

            nameText.text = gestureModel.name;
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    
}
