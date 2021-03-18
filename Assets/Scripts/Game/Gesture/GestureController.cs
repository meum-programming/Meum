using Core.Socket;
using Game.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
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
        NumberKeyTouchCheck();
        
    }

    /// <summary>
    /// 키보드 숫자키가 눌렸는지 체크
    /// </summary>
    void NumberKeyTouchCheck()
    {
        if (Keyboard.current.digit1Key.wasPressedThisFrame || Keyboard.current.numpad1Key.wasPressedThisFrame)
        {
            HotKeyTouchOn(0);
        }
        if (Keyboard.current.digit2Key.wasPressedThisFrame || Keyboard.current.numpad2Key.wasPressedThisFrame)
        {
            HotKeyTouchOn(1);
        }
        if (Keyboard.current.digit3Key.wasPressedThisFrame || Keyboard.current.numpad3Key.wasPressedThisFrame)
        {
            HotKeyTouchOn(2);
        }
        if (Keyboard.current.digit4Key.wasPressedThisFrame || Keyboard.current.numpad4Key.wasPressedThisFrame)
        {
            HotKeyTouchOn(3);
        }
        if (Keyboard.current.digit5Key.wasPressedThisFrame || Keyboard.current.numpad5Key.wasPressedThisFrame)
        {
            HotKeyTouchOn(4);
        }
        if (Keyboard.current.digit6Key.wasPressedThisFrame || Keyboard.current.numpad6Key.wasPressedThisFrame)
        {
            HotKeyTouchOn(5);
        }
        if (Keyboard.current.digit7Key.wasPressedThisFrame || Keyboard.current.numpad7Key.wasPressedThisFrame)
        {
            HotKeyTouchOn(6);
        }
        if (Keyboard.current.digit8Key.wasPressedThisFrame || Keyboard.current.numpad8Key.wasPressedThisFrame)
        {
            HotKeyTouchOn(7);
        }
        if (Keyboard.current.digit9Key.wasPressedThisFrame || Keyboard.current.numpad9Key.wasPressedThisFrame)
        {
            HotKeyTouchOn(8);
        }
        if (Keyboard.current.digit0Key.wasPressedThisFrame || Keyboard.current.numpad0Key.wasPressedThisFrame)
        {
            HotKeyTouchOn(9);
        }
    }

    /// <summary>
    /// 넘버키가 눌렸을때 실행
    /// </summary>
    /// <param name="index"></param>
    void HotKeyTouchOn(int index)
    {
        //터치된 넘버키에 맞는 슬롯정보로 제스쳐 모델정보 취득
        GestureModel gestureModel = GestureManager.Instance.GetSlotData(index);

        if (gestureModel != null)
        {
            PlayGestureAnim(gestureModel.id);
        }
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

    public void OnPointerClick()
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
