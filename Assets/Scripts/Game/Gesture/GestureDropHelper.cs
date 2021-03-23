using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GestureDropHelper : GestureHelperBase, IDropHandler, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public int slotNum;
    [SerializeField] Text numText;

    public void DataSet(int slotNum, GestureModel gestureModel)
    {
        this.slotNum = slotNum;
        this.gestureModel = gestureModel;

        numText.text = ((slotNum + 1) % 10).ToString();

        if (this.gestureModel == null)
        {
            nullImage.gameObject.SetActive(true);
            iconImage.gameObject.SetActive(false);
        }
        else
        {
            nullImage.gameObject.SetActive(false);
            iconImage.gameObject.SetActive(true);
            iconImage.sprite = gestureModel.sprite;
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
            return;

        if (eventData.pointerDrag != null)
        {
            GestureModel newGestureModel = null;

            if (eventData.pointerDrag.GetComponent<GestureDragHelper>() != null)
            {
                newGestureModel = eventData.pointerDrag.GetComponent<GestureDragHelper>().gestureModel;
            }
            else if (eventData.pointerDrag.GetComponent<GestureDropHelper>() != null)
            {
                newGestureModel = eventData.pointerDrag.GetComponent<GestureDropHelper>().gestureModel;
                gestureSettingPopup.MoveDropObjDataChange(this.gestureModel);
                gestureSettingPopup.dropOn = true;
            }

            if (newGestureModel != null)
            {
                GestureManager.Instance.SetSlotData(slotNum, newGestureModel.id);
                DataSet(slotNum, newGestureModel);

            }
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            DataSet(slotNum, null);
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

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (gestureModel == null || eventData.button == PointerEventData.InputButton.Right)
            return;

        gestureSettingPopup.OnBeginDrag(eventData, this);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (gestureModel == null || eventData.button == PointerEventData.InputButton.Right)
            return;

        gestureSettingPopup.OnDrag(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
            return;

        gestureSettingPopup.OnEndDrag(eventData);
    }

}