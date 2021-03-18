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
        PointerEventData peData = (PointerEventData)eventData;

        if (peData.button == PointerEventData.InputButton.Right)
            return;

        //아트워크 설명창이 안나오도록 세팅
        EventSystem.current.SetSelectedGameObject(this.gameObject, eventData);

        MoveOnSet(true);
    }

    public void OnDrag(BaseEventData eventData)
    {
        PointerEventData peData = (PointerEventData)eventData;

        if (peData.button == PointerEventData.InputButton.Right)
            return;

        Vector2 joySticPos = GetPadResultPos(peData.delta);

        MoveValueSet(joySticPos);

        pad.rectTransform.anchoredPosition3D = joySticPos;

    }

    void MoveValueSet(Vector2 joySticPos)
    {
        float absX = Mathf.Abs(joySticPos.x);
        float absY = Mathf.Abs(joySticPos.y);

        float moveValueX = joySticPos.x > 0 ? 1 : -1;
        float moveValueY = joySticPos.y > 0 ? 1 : -1;

        if (joySticPos == Vector2.zero)
        {
            moveValue = Vector2.zero;
        }
        //조이스틱 방향이 오른쪽 이나 왼쪽이라면
        else if (absX > absY && absX > 65)
        {
            moveValue = new Vector2(moveValueX, 0);
        }
        //조이스틱 방향이 위나 아래 라면
        else if (absY > absX && absY > 65)
        {
            moveValue = new Vector2(0, moveValueY);
        }
        //조이스틱 방향이 대각선이라면
        else
        {
            moveValue = new Vector2(moveValueX * 0.75f, moveValueY * 0.75f);
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
        PointerEventData peData = (PointerEventData)eventData;

        if (peData.button == PointerEventData.InputButton.Right)
            return;

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
