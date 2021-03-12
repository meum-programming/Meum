using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GestureSettingPopup : MonoBehaviour
{
    [SerializeField] RectTransform gestureReadyListPanel;
    [SerializeField] GestureDragHelper gestureReadyObj;

    [SerializeField] RectTransform gestureSlotListPanel;

    private List<GestureDropHelper> gestureSlotObjList = new List<GestureDropHelper>();
    [SerializeField] GestureDropHelper gestureSlotObj;

    [SerializeField] RectTransform moveOBJ;
    private GestureDragHelper moveDragOBJData;
    public GestureDropHelper moveDropOBJData;

    public bool dropOn = false;

    Canvas canvas = null;

    // Start is called before the first frame update
    void Start()
    {
        Init();
        GestureReadyListSet();
        GestureSlotListSet();
    }
    void Init()
    {
        canvas = transform.GetComponentInParent<Canvas>();
    }


    void GestureReadyListSet()
    {
        for (int i = 0; i < 30; i++)
        {
            GestureDragHelper obj = Instantiate(gestureReadyObj, gestureReadyListPanel);

            GestureModel gestureModel = GestureManager.Instance.GetModel(i);
            obj.DataSet(gestureModel);
            obj.gameObject.SetActive(true);
        }
    }

    void GestureSlotListSet()
    {
        int slotCnt = 10;

        if (gestureSlotObjList.Count < slotCnt)
        {
            for (int i = gestureSlotObjList.Count; i < slotCnt; i++)
            {
                GestureDropHelper obj = Instantiate(gestureSlotObj, gestureSlotListPanel);
                gestureSlotObjList.Add(obj);
            }
        }

        for (int i = 0; i < slotCnt; i++)
        {
            GestureDropHelper obj = gestureSlotObjList[i];

            GestureModel gestureModel = GestureManager.Instance.GetSlotData(i);
            obj.DataSet(i, gestureModel);
            obj.gameObject.SetActive(true);
        }

    }


    public void OnBeginDrag(PointerEventData eventData , GestureDragHelper gestureDragHelper)
    {
        moveDragOBJData = gestureDragHelper;
        moveDropOBJData = null;

        moveOBJ.gameObject.SetActive(true);
        moveOBJ.position = gestureDragHelper.transform.position;

        moveOBJ.GetComponent<Image>().sprite = moveDragOBJData.gestureModel.sprite;

    }
    public void OnBeginDrag(PointerEventData eventData, GestureDropHelper gestureDragHelper)
    {
        moveDragOBJData = null;
        moveDropOBJData = gestureDragHelper;

        moveOBJ.gameObject.SetActive(true);
        moveOBJ.position = gestureDragHelper.transform.position;

        moveOBJ.GetComponent<Image>().sprite = moveDropOBJData.gestureModel.sprite;

    }

    public void OnDrag(PointerEventData eventData)
    {
        float scaleValue = canvas.transform.localScale.y;

        moveOBJ.anchoredPosition += (eventData.delta/ scaleValue);
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        moveOBJ.gameObject.SetActive(false);

        if (dropOn)
        {
            dropOn = false;
        }
        else
        {
            MoveDropObjDataChange(null);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
    }

    public void AllClearBtnClick()
    {
        GestureManager.Instance.SlotAllClear();
        GestureSlotListSet();
    }

    public void MoveDropObjDataChange(GestureModel gestureModel)
    {
        if (moveDropOBJData == null)
            return;
        
        int modelId = gestureModel != null ? gestureModel.id : -1;

        GestureManager.Instance.SetSlotData(moveDropOBJData.slotNum, modelId);
        moveDropOBJData.DataSet(moveDropOBJData.slotNum, gestureModel);
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
