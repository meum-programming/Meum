using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemSlot : MonoBehaviour , IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        Debug.LogWarning("OnDrop");

        if (eventData.pointerDrag != null)
        {
            RectTransform rectTransform = eventData.pointerDrag.GetComponent<RectTransform>();

            rectTransform.parent = transform;

            rectTransform.anchoredPosition3D = Vector3.zero;
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
