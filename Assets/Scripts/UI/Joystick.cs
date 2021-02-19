using Game.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Joystick : MonoBehaviour
{
    [SerializeField] Image bg;
    [SerializeField] Image pad;

    float maxDistance = 0;

    public bool moveOn = false;
    Vector2 moveValue = Vector2.zero;

    public UnityAction<Vector2> moveEventOn = null;

    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    void Init()
    {
        maxDistance = bg.rectTransform.sizeDelta.x / 2.5f;
    }

    // Update is called once per frame
    void Update()
    {
        MoveCheck();   
    }

    void MoveCheck()
    {
        if (moveOn == false)  
            return;

        MoveEventOn();
    }

    void MoveEventOn()
    {
        if (moveEventOn != null)
        {
            moveEventOn(moveValue);
        }
    }


    public void OnPointDown(BaseEventData eventData)
    {
        MoveOnSet(true);
    }

    public void OnDrag(BaseEventData eventData)
    {
        PointerEventData peData = (PointerEventData)eventData;

        Vector2 joySticPos = GetPadResultPos(peData.delta);

        MoveValueSet(joySticPos);

        pad.rectTransform.anchoredPosition3D = joySticPos;

    }

    void MoveValueSet(Vector2 joySticPos)
    {
        float absX = Mathf.Abs(joySticPos.x);
        float absY = Mathf.Abs(joySticPos.y);

        if (joySticPos == Vector2.zero)
        {
            moveValue = Vector2.zero;
        }
        //절대값y 보다 절대값x이 더 크다면
        else if (absX > absY)
        {
            float moveValueX = joySticPos.x / Mathf.Abs(joySticPos.x);
            moveValue = new Vector2(moveValueX, 0);
        }
        else
        {
            float moveValueY = joySticPos.y / Mathf.Abs(joySticPos.y);
            moveValue = new Vector2(0, moveValueY);
        }
    }


    Vector2 GetPadResultPos(Vector2 deltaPos)
    {
        Vector2 pos = pad.rectTransform.anchoredPosition3D;
        
        float posX = pos.x + deltaPos.x;
        float posY = pos.y + deltaPos.y;

        Vector2 resultPos = new Vector2(posX, posY);

        float distance = Vector2.Distance(Vector2.zero, resultPos);

        if (distance > maxDistance)
        {
            resultPos *= (maxDistance / distance);
        }

        return resultPos;
    }

    public void OnPointUp(BaseEventData eventData)
    {
        MoveOnSet(false);

        MoveEventOn();
    }

    void MoveOnSet(bool moveOn)
    {
        this.moveOn = moveOn;
        MoveValueSet(Vector2.zero);
        pad.rectTransform.anchoredPosition3D = moveValue;
    }

}
