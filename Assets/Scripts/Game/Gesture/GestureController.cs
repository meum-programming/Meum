using Core.Socket;
using Game.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
//using UnityEngine.InputSystem;
using UnityEngine.UI;

public class GestureController : MonoBehaviour
{
    bool hoverOn = false;
    public bool panelOpenOn = false;

    [SerializeField] Image bgImage;
    [SerializeField] Image iconImage;
    [SerializeField] Image hoverImage;

    private List<GestureBtn> gestureSlotObjList = new List<GestureBtn>();
    [SerializeField] RectTransform gestureSlotListPanel;
    [SerializeField] GestureBtn gestureSlotObj;
    [SerializeField] GestureBtn gestureSetBtn;

    bool showOn = false;

    [SerializeField] MouseToggleButton mouseToggleButton;
    [SerializeField] SoundToggleButton soundToggleButton;


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
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            HotKeyTouchOn(0);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            HotKeyTouchOn(1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            HotKeyTouchOn(2);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            HotKeyTouchOn(3);
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            HotKeyTouchOn(4);
        }
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            HotKeyTouchOn(5);
        }
        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            HotKeyTouchOn(6);
        }
        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            HotKeyTouchOn(7);
        }
        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            HotKeyTouchOn(8);
        }
        if (Input.GetKeyDown(KeyCode.Alpha0))
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

    public void PanelOpenSet(bool panelOpenOn)
    {
        this.panelOpenOn = panelOpenOn;

        Color iconImageColor = panelOpenOn ? new Color(0.5f, 0.5f, 0.5f, 0.5f) : new Color(1, 1, 1, 1);
        iconImage.color = iconImageColor;
        //bgImage.color = iconImageColor;

        int zVlaue = panelOpenOn ? 90 : 270;
        hoverImage.transform.rotation = Quaternion.Euler(new Vector3(0, 0, zVlaue));

        gestureSlotListPanel.transform.parent.gameObject.SetActive(panelOpenOn);
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
            PlayerAnimationController playerAnimationController = DataSynchronizer.Get().GetLocalPlayer().GetComponentInChildren<PlayerAnimationController>();
            playerAnimationController.PlayGestureAnim(animName);
        }
    }

    public void GestureSettingPopupOpen(bool open)
    {
        Debug.LogWarning(open);
    }

}
