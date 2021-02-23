using Core.Socket;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

public class AnimTestContoller : MonoBehaviour
{
    [SerializeField] Animator anim;
    [SerializeField] Transform modelParant;
    [SerializeField] PlayerChaChange playerChaChange;
    [SerializeField] Text saveOnText;
    Tween saveOnTextEventTween = null;
    Coroutine saveOnTextEvent = null;

    //private SkinnedMeshRenderer skinMesh;
    private List<SkinnedMeshRenderer>  skinMesh = new List<SkinnedMeshRenderer>();
    private List<SkinnedMeshRenderer> hairMesh = new List<SkinnedMeshRenderer>();

    int currentSkinStatus = 1;

    ChaCustomizingSaveData chaCustomizingSaveData = null;

    [SerializeField] RectTransform goHomePopup;

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
        SkinMetarialSet();

        GetChaCustomizingSaveData();

        ChangeAllData(chaCustomizingSaveData);
    }

    void GetChaCustomizingSaveData()
    {
        chaCustomizingSaveData = new ChaCustomizingSaveData(0, 0, 0, 0);
        
    }

    void ChangeAllData(ChaCustomizingSaveData data)
    {
        currentSkinStatus = data.skinStatus;
        SkinColorSet();
        playerChaChange.AllChangeData(data);
    }


    /// <summary>
    /// 피부색을 담당하는 메테리얼 세팅
    /// </summary>
    void SkinMetarialSet()
    {
        skinMesh = new List<SkinnedMeshRenderer>();

        //skinMesh.Add(anim.transform.Find("person_model_default").GetComponent<SkinnedMeshRenderer>());
        skinMesh.Add(anim.transform.Find("person_model_short_clothes").GetComponent<SkinnedMeshRenderer>());

        //skinMesh.material.SetFloat("_Smoothness", 0.1f);

        hairMesh = new List<SkinnedMeshRenderer>();
        hairMesh.Add(anim.transform.Find("hair_1").GetComponent<SkinnedMeshRenderer>());
        hairMesh.Add(anim.transform.Find("hair_2").GetComponent<SkinnedMeshRenderer>());
        hairMesh.Add(anim.transform.Find("hair_3").GetComponent<SkinnedMeshRenderer>());
        hairMesh.Add(anim.transform.Find("hair_4").GetComponent<SkinnedMeshRenderer>());
        hairMesh.Add(anim.transform.Find("hair_5").GetComponent<SkinnedMeshRenderer>());
        hairMesh.Add(anim.transform.Find("hair_6").GetComponent<SkinnedMeshRenderer>());
        hairMesh.Add(anim.transform.Find("hair_7").GetComponent<SkinnedMeshRenderer>());

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// 애니메이션 변경 버튼 클릭시 호출
    /// </summary>
    /// <param name="status"></param>
    public void AnimPlayBtnClick(int status)
    {
        AnimPlay((AnimStatus)status);
    }

    /// <summary>
    /// 애니메이션 재생
    /// </summary>
    /// <param name="animStatus"></param>
    private void AnimPlay(AnimStatus animStatus)
    {
        anim.Play(animStatus.ToString());
    }

    /// <summary>
    /// 피부색 변경 버튼 클릭시 호출
    /// </summary>
    public void SkinChangeBtnClick(int skinStatus)
    {
        currentSkinStatus = skinStatus;
        SkinColorSet();
    }

    /// <summary>
    /// 피부색 변경
    /// </summary>
    void SkinColorSet()
    {
        //4C3530

        for (int i = 0; i < skinMesh.Count; i++)
        {
            skinMesh[i].material.color = GetSkinColor();
        }


        for (int i = 0; i < hairMesh.Count; i++)
        {
            hairMesh[i].material.color = GetHairColor();
        }
    }

    private Color GetHairColor()
    {
        string hexCode = "#625F5E";

        switch (currentSkinStatus)
        {
            case 0:
                hexCode = "#625F5E";
                break;
            case 1:
                hexCode = "#35434F";
                break;
            case 2:
                hexCode = "#101010";
                break;
            case 3:
                hexCode = "#FFFEF4";
                break;
            case 4:
                hexCode = "#9ABBAD";
                break;
            case 5:
                hexCode = "#EFE6E1";
                break;
            case 6:
                hexCode = "#BED0D9";
                break;
            case 7:
                hexCode = "#AE9ABB";
                break;
        }

        Color color;
        ColorUtility.TryParseHtmlString(hexCode, out color);

        return color;

    }

    private Color GetSkinColor()
    {
        string hexCode = "#F6EBE5";

        switch (currentSkinStatus)
        {
            case 0:
                hexCode = "#F6EBE5";
                break;
            case 1:
                hexCode = "#E4C6B5";
                break;
            case 2:
                hexCode = "#B38B7D";
                break;
            case 3:
                hexCode = "#4C3530";
                break;
            case 4:
                hexCode = "#E7D3E4";
                break;
            case 5:
                hexCode = "#7C292B";
                break;
            case 6:
                hexCode = "#969EB0";
                break;
            case 7:
                hexCode = "#6652CA";
                break;
        }

        Color color;
        ColorUtility.TryParseHtmlString(hexCode, out color);

        return color;

    }

    public void SkinSetChangeBtnClick(int index)
    {
        currentSkinStatus = index;
        SkinColorSet();
    }


    /// <summary>
    /// 드래그 시 호출 - 캐릭터 회전용
    /// </summary>
    /// <param name="eventData"></param>
    public void OnDrag(BaseEventData eventData)
    {
        PointerEventData peData = (PointerEventData)eventData;

        Vector3 rotValue = modelParant.localRotation.eulerAngles;
        rotValue.y -= (peData.delta.x/2);
        modelParant.rotation = Quaternion.Euler(rotValue);
    }


    public void SaveBtnClick() 
    {
        chaCustomizingSaveData.skinStatus = currentSkinStatus;
        chaCustomizingSaveData.hairIndex = playerChaChange.hairIndex;
        chaCustomizingSaveData.maskIndex = playerChaChange.maskIndex;
        chaCustomizingSaveData.dressIndex = playerChaChange.dressIndex;

        //이전에 실행중인 이벤트가 있으면 멈춘다
        if (saveOnTextEvent != null)
        {
            StopCoroutine(saveOnTextEvent);
        }

        //변경 완료 텍스트 이벤트 실행
        saveOnTextEvent =  StartCoroutine(SaveOnTextSet());
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
        ChangeAllData(chaCustomizingSaveData);
    }

    public void RandomSetBtnClick()
    {
        ChaCustomizingSaveData tempData = new ChaCustomizingSaveData();

        tempData.skinStatus = Random.Range(0, 8);
        tempData.hairIndex = Random.Range(0, 7);
        tempData.maskIndex = Random.Range(0, 2);
        tempData.dressIndex = Random.Range(0, 5);

        ChangeAllData(tempData);
    }

}

public class ChaCustomizingSaveData
{
    public int hairIndex = 0;
    public int maskIndex = 0;
    public int dressIndex = 0;
    public int skinStatus = 1;

    public ChaCustomizingSaveData() { }
    public ChaCustomizingSaveData(int hairIndex, int maskIndex, int dressIndex, int currentSkinStatus ) 
    {
        this.hairIndex = hairIndex;
        this.maskIndex = maskIndex;
        this.dressIndex = dressIndex;
        this.skinStatus = currentSkinStatus;
    }
}