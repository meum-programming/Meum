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
        tempData.hairIndex = Random.Range(0, 4);
        tempData.maskIndex = Random.Range(0, 3);
        tempData.dressIndex = Random.Range(0, 4);

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
                dressStr = "화이트 세미정장";
                break;
            case 1:
                dressStr = "블랙 세미정장";
                break;
            case 2:
                dressStr = "데님 작업복";
                break;
            case 3:
                dressStr = "화가 복장";
                break;
            case 4:
                dressStr = "베이지 맨투맨";
                break;
            case 5:
                dressStr = "블랙 믐투믐";
                break;
            case 6:
                dressStr = "블루밍 자켓";
                break;
            case 7:
                dressStr = "브라운 멜빵";
                break;
        }
        dressName.text = dressStr;
    }


    public void HairIndexChangeBtnClick(bool next)
    {
        playerChaChange.HairIndexChangeBtnClick(next);

        tempData.hairIndex = playerChaChange.hairIndex;

        HairTextSet();
    }

    void HairTextSet()
    {
        string hairStr = "";

        switch (tempData.hairIndex)
        {
            case 0:
                hairStr = "몽실 헤어";
                break;
            case 1:
                hairStr = "댄디 헤어";
                break;
            case 2:
                hairStr = "안나 헤어";
                break;
            case 3:
                hairStr = "포니 헤어";
                break;
            case 4:
                hairStr = "쉼표 헤어";
                break;
            case 5:
                hairStr = "러플 헤어";
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
                    maskStr = "없음";
                    break;
                case 1:
                    maskStr = "동그란 안경";
                    break;
                case 2:
                    maskStr = "뿔테 안경";
                    break;
                case 3:
                    maskStr = "멋쟁이 선글라스";
                    break;
                case 4:
                    maskStr = "한쪽 안대";
                    break;
                case 5:
                    maskStr = "머리 위 선글라스";
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