using Core.Socket;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;
using Core;

public class AnimTestContoller : MonoBehaviour
{
    [SerializeField] Transform modelParant;
    [SerializeField] PlayerChaChange playerChaChange;
    [SerializeField] Text saveOnText;
    Tween saveOnTextEventTween = null;
    Coroutine saveOnTextEvent = null;

    [SerializeField] RectTransform goHomePopup;

    ChaCustomizingSaveData tempData = new ChaCustomizingSaveData();

    [SerializeField] Text hairName;
    [SerializeField] Text maskName;
    [SerializeField] Text dressName;

    enum AnimStatus
    {
        idle = 0,
        walk,
        argue
    }
         

    // Start is called before the first frame update
    void Start()
    {
        Init();
        
    }

    void Init()
    {
        ChaCustomizingSaveData saveData = DataManager.Instance.chaCustomizingSaveData;
        tempData = new ChaCustomizingSaveData();
        tempData.hairIndex = saveData.hairIndex;
        tempData.maskIndex = saveData.maskIndex;
        tempData.dressIndex = saveData.dressIndex;
        tempData.skinIndex = saveData.skinIndex;

        HairTextSet();
        MaskTextSet();
        DressTextSet();
    }

    /// <summary>
    /// 드래그 시 호출 - 캐릭터 회전용
    /// </summary>
    /// <param name="eventData"></param>
    public void OnDrag(BaseEventData eventData)
    {
        PointerEventData peData = (PointerEventData)eventData;

        Vector3 rotValue = modelParant.rotation.eulerAngles;
        rotValue.y -= (peData.delta.x);

        modelParant.rotation = Quaternion.Euler(rotValue);
    }


    public void SaveBtnClick() 
    {
        ChaCustomizingSaveData newSaveData = new ChaCustomizingSaveData();
        newSaveData.skinIndex = tempData.skinIndex;
        newSaveData.hairIndex = tempData.hairIndex;
        newSaveData.maskIndex = tempData.maskIndex;
        newSaveData.dressIndex = tempData.dressIndex;

        UserInfoRequest userInfoRequest = new UserInfoRequest()
        {
            requestStatus = 2,
            uid = MeumDB.Get().GetToken(),
            hairIndex = newSaveData.hairIndex,
            maskIndex = newSaveData.maskIndex,
            dressIndex = newSaveData.dressIndex,
            skinIndex = newSaveData.skinIndex,

            successOn = ResultData =>
            {
                DataManager.Instance.chaCustomizingSaveData = newSaveData;
                playerChaChange.chaCustomizingSaveData = newSaveData;

                //이전에 실행중인 이벤트가 있으면 멈춘다
                if (saveOnTextEvent != null)
                {
                    StopCoroutine(saveOnTextEvent);
                }

                //변경 완료 텍스트 이벤트 실행
                saveOnTextEvent = StartCoroutine(SaveOnTextSet());
            }
        };
        userInfoRequest.RequestOn();
    }

    /// <summary>
    /// 저장 완료 문구 출력 이벤트
    /// </summary>
    /// <returns></returns>
    IEnumerator SaveOnTextSet()
    {
        saveOnText.gameObject.SetActive(true);

        if (saveOnTextEventTween != null && saveOnTextEventTween.IsPlaying())
        {
            saveOnTextEventTween.Kill();
        }

        saveOnTextEventTween =  saveOnText.DOFade(1, 0.1f);

        yield return new WaitForSeconds(1);

        saveOnTextEventTween = saveOnText.DOFade(0, 1f);

        yield return new WaitForSeconds(1);
    }

    public void GoHomePopupOpen(bool active)
    {
        goHomePopup.gameObject.SetActive(active);
    }

    public void GoHomeBtnClick()
    {
        Core.Socket.MeumSocket.Get().ReturnToGalleryScene();
    }

    public void ReturnBtnClick()
    {
        tempData = new ChaCustomizingSaveData();
        tempData.skinIndex = playerChaChange.chaCustomizingSaveData.skinIndex;
        tempData.hairIndex = playerChaChange.chaCustomizingSaveData.hairIndex;
        tempData.maskIndex = playerChaChange.chaCustomizingSaveData.maskIndex;
        tempData.dressIndex = playerChaChange.chaCustomizingSaveData.dressIndex;

        AllDataSet();
    }

    public void RandomSetBtnClick()
    {
        tempData.skinIndex = Random.Range(0, 8);
        tempData.hairIndex = Random.Range(0, 7);
        tempData.maskIndex = Random.Range(0, 2);
        tempData.dressIndex = Random.Range(0, 5);

        AllDataSet();
    }

    public void AllDataSet()
    {
        playerChaChange.AllChangeData(tempData);

        DressTextSet();
        HairTextSet();
        MaskTextSet();
    }

    public void SkinColorSet(int currentSkinStatus)
    {
        tempData.skinIndex = currentSkinStatus;
        playerChaChange.SkinColorSet(currentSkinStatus);
    }

    public void DrassIndexChangeBtnClick(bool next)
    {
        playerChaChange.DrassIndexChangeBtnClick(next);

        tempData.dressIndex = playerChaChange.dressIndex;

        DressTextSet();
    }
    void DressTextSet()
    {
        string dressStr = "";

        switch (tempData.dressIndex)
        {
            case 0:
                dressStr = "단정한 복장";
                break;
            case 1:
                dressStr = "작업자 복장";
                break;
            case 2:
                dressStr = "따뜻한 복장";
                break;
            case 3:
                dressStr = "정중한 복장";
                break;
            case 4:
                dressStr = "시원한 복장";
                break;
        }
        dressName.text = dressStr;
    }

    public void HairIndexChangeBtnClick(bool next)
    {
        playerChaChange.HairIndexChangeBtnClick(next);

        tempData.dressIndex = playerChaChange.hairIndex;

        HairTextSet();
    }

    void HairTextSet()
    {
        string hairStr = "";

        switch (tempData.dressIndex)
        {
            case 0:
                hairStr = "곱슬머리 헤어";
                break;
            case 1:
                hairStr = "꽁지머리 헤어";
                break;
            case 2:
                hairStr = "깔끔한 헤어";
                break;
            case 3:
                hairStr = "단정한 헤어";
                break;
            case 4:
                hairStr = "포니 헤어";
                break;
            case 5:
                hairStr = "러프 헤어";
                break;
            case 6:
                hairStr = "정열적인 헤어";
                break;
        }
        hairName.text = hairStr;
    }



    public void MaskIndexChangeBtnClick(bool next)
    {
        playerChaChange.MaskIndexChangeBtnClick(next);

        tempData.maskIndex = playerChaChange.maskIndex;

        MaskTextSet();
    }

    void MaskTextSet()
    {
        if (maskName != null)
        {
            string maskStr = "";

            switch (tempData.maskIndex)
            {
                case 0:
                    maskStr = "늑대 가면";
                    break;
                case 1:
                    maskStr = "토끼 가면";
                    break;
            }
            maskName.text = maskStr;
        }
    }


}

public class ChaCustomizingSaveData
{
    public int hairIndex = 0;
    public int maskIndex = 0;
    public int dressIndex = 0;
    public int skinIndex = 1;

    public ChaCustomizingSaveData() { }
    public ChaCustomizingSaveData(int hairIndex, int maskIndex, int dressIndex, int currentSkinStatus ) 
    {
        this.hairIndex = hairIndex;
        this.maskIndex = maskIndex;
        this.dressIndex = dressIndex;
        this.skinIndex = currentSkinStatus;
    }
}