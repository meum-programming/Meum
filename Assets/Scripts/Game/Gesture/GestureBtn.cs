using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GestureBtn : MonoBehaviour , IPointerEnterHandler , IPointerExitHandler , IPointerClickHandler
{
    [SerializeField] Image iconImage;
    [SerializeField] Image nullImage;
    [SerializeField] Text iconNameText;
    [SerializeField] Text slotNumText;

    GestureModel model = null;
    public int slotNum = 0;
    bool hoverOn = false;

    public UnityAction<int> playBtnClickOn = null;

    void Awake()
    {
        Init();
        
    }

    void Init()
    {
        //DataSet();
        
    }

    public void DataSet(int slotNum, GestureModel gestureModel)
    {
        this.slotNum = slotNum;
        model = gestureModel;

        if (slotNumText != null)
        {
            string slotStr = ((slotNum + 1) % 10).ToString();
            slotNumText.text = slotStr;
        }

        if (model != null)
        {
            iconNameText.text = model.name;
            iconImage.sprite = model.sprite;

            

            nullImage.gameObject.SetActive(false);
            iconNameText.gameObject.SetActive(false);
            iconImage.gameObject.SetActive(true);
         //   slotNumText.gameObject.SetActive(false);

        }
        else
        {
            nullImage.gameObject.SetActive(true);
            iconNameText.gameObject.SetActive(false);
            iconImage.gameObject.SetActive(false);
           // slotNumText.gameObject.SetActive(false);
        }

        HoverOnSet(false);
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        HoverOnSet(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        HoverOnSet(false);
    }

    void HoverOnSet(bool hoverOn)
    {
        if (slotNum != -1 && model == null)
            return;
        
        this.hoverOn = hoverOn;

        iconImage.gameObject.SetActive(!this.hoverOn);
        iconNameText.gameObject.SetActive(this.hoverOn);

        if (slotNumText != null)
        {
            //slotNumText.gameObject.SetActive(this.hoverOn);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (model == null)
            return;

        if (playBtnClickOn != null)
        {
            playBtnClickOn(model.id);
        }

        //FindObjectOfType<GestureController>().PlayGestureAnim(model.id);
    }
}

