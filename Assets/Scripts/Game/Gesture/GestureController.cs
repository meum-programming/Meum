using Core.Socket;
using Game.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GestureController : MonoBehaviour
{
    bool hoverOn = false;
    bool panelOpenOn = false;

    [SerializeField] Image bgImage;
    [SerializeField] Image iconImage;
    [SerializeField] Image hoverImage;

    private List<GestureBtn> gestureSlotObjList = new List<GestureBtn>();
    [SerializeField] RectTransform gestureSlotListPanel;
    [SerializeField] GestureBtn gestureSlotObj;
    [SerializeField] GestureBtn gestureSetBtn;

    void Awake()
    {
        Init();
    }

    void Init()
    {
        DataSet();
        PanelOpenSet(this.panelOpenOn);

        GestureManager.Instance.slotChangeOn = DataSet;
    }

    public void DataSet()
    {
        int slotCnt = 10;

        if (gestureSlotObjList.Count < slotCnt)
        {
            for (int i = gestureSlotObjList.Count; i < slotCnt; i++)
            {
                GestureBtn obj = Instantiate(gestureSlotObj, gestureSlotListPanel);
                obj.playBtnClickOn = PlayGestureAnim;
                gestureSlotObjList.Add(obj);
            }

            gestureSetBtn.transform.SetSiblingIndex(gestureSlotListPanel.transform.childCount);

        }

        for (int i = 0; i < slotCnt; i++)
        {
            GestureBtn obj = gestureSlotObjList[i];

            GestureModel gestureModel = GestureManager.Instance.GetSlotData(i);
            obj.DataSet(i, gestureModel);
            obj.gameObject.SetActive(true);
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

    public void OnPointerEnter(BaseEventData eventData)
    {
        HoverOnSet(true);
    }


    public void OnPointerExit(BaseEventData eventData)
    {
        HoverOnSet(false);
    }

    void HoverOnSet(bool hoverOn)
    {
        this.hoverOn = hoverOn;

        iconImage.gameObject.SetActive(!hoverOn);
        hoverImage.gameObject.SetActive(hoverOn);
    }

    public void OnPointerClick(BaseEventData eventData)
    {
        PanelOpenSet(!this.panelOpenOn);
    }

    void PanelOpenSet(bool panelOpenOn)
    {
        this.panelOpenOn = panelOpenOn;

        Color iconImageColor = panelOpenOn ? new Color(0.5f, 0.5f, 0.5f, 0.5f) : new Color(1, 1, 1, 1);
        iconImage.color = iconImageColor;
        //bgImage.color = iconImageColor;

        int zVlaue = panelOpenOn ? 90 : 270;
        hoverImage.transform.rotation = Quaternion.Euler(new Vector3(0, 0, zVlaue));

        gestureSlotListPanel.gameObject.SetActive(panelOpenOn);
    }


    public void PlayGestureAnim(int status)
    {
        string animName = string.Empty;

        switch (status)
        {
            case 0:
                animName = "Gesture_greet_bigwave";
                break;
            case 1:
                animName = "Gesture_clap";
                break;
            case 2:
                animName = "Gesture_no";
                break;
            case 3:
                animName = "Gesture_nod";
                break;
            case 4:
                animName = "Gesture_pointing";
                break;
            case 5:
                animName = "Gesture_angry";
                break;
            case 6:
                animName = "Gesture_depressed";
                break;
            case 7:
                animName = "Gesture_yawn";
                break;
            case 8:
                animName = "Gesture_afraid";
                break;
            case 9:
                animName = "Gesture_excited";
                break;
        }

        if (animName != string.Empty)
        {
            PlayerAnimationController playerAnimationController = DataSynchronizer.Get().GetLocalPlayer().GetComponent<PlayerAnimationController>();
            playerAnimationController.PlayGestureAnim(animName);
        }
    }

    public void GestureSettingPopupOpen(bool open)
    {
        Debug.LogWarning(open);
    }

}
