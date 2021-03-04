using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GestureDragDrop : MonoBehaviour , IPointerDownHandler,IBeginDragHandler,IEndDragHandler,IDragHandler
{
    private RectTransform rectTransform;

    void Awake() 
    {
        rectTransform = GetComponent<RectTransform>();
    }


    public void OnBeginDrag(PointerEventData eventData)
    {
        transform.GetComponent<Image>().raycastTarget = false;
        Debug.LogWarning("OnBeginDrag");
    }
    public void OnDrag(PointerEventData eventData)
    {
        Debug.LogWarning("OnDrag");
        rectTransform.anchoredPosition += eventData.delta;
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        transform.GetComponent<Image>().raycastTarget = true;
        Debug.LogWarning("OnEndDrag");
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.LogWarning("OnPointerDown");
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
