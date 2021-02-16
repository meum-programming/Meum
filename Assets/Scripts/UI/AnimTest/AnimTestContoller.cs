using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class AnimTestContoller : MonoBehaviour
{
    [SerializeField] Animator anim;
    [SerializeField] Transform modelParant;
    private SkinnedMeshRenderer skinMesh;

    int currentSkinStatus = 1;

    enum AnimStatus
    {
        idle = 0,
        walk,
        argue
    }
         

    // Start is called before the first frame update
    void Start()
    {
        SkinMetarialSet();
        SkinColorSet();
    }

    /// <summary>
    /// 피부색을 담당하는 메테리얼 세팅
    /// </summary>
    void SkinMetarialSet()
    {
        skinMesh = anim.transform.Find("person_model").GetComponent<SkinnedMeshRenderer>();
        skinMesh.material.SetFloat("_Smoothness", 0.1f);
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
                hexCode = "#969EB0";
                break;
            case 5:
                hexCode = "#E7D3E4";
                break;
            case 6:
                hexCode = "#7C292B";
                break;
            case 7:
                hexCode = "#6652CA";
                break;
        }

        Color color;
        ColorUtility.TryParseHtmlString(hexCode, out color);

        skinMesh.material.color = color;
    }


    /// <summary>
    /// 드래그 시 호출 - 캐릭터 회전용
    /// </summary>
    /// <param name="eventData"></param>
    public void OnDrag(BaseEventData eventData)
    {
        PointerEventData peData = (PointerEventData)eventData;

        Vector3 rotValue = modelParant.localRotation.eulerAngles;
        rotValue.y -= peData.delta.x;
        modelParant.rotation = Quaternion.Euler(rotValue);
    }

}
