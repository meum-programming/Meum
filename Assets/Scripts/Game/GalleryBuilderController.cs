using Core;
using Core.Socket;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GalleryBuilderController : MonoBehaviour
{
    [SerializeField] Toggle toggle;
    [SerializeField] RectTransform allUIPanel;

    private void Awake()
    {
        DataSet();   
    }

    void DataSet() 
    {
        if (MeumDB.Get() == null || MeumDB.Get().myRoomInfo == null)
            return;
        
        toggle.isOn = MeumDB.Get().myRoomInfo.is_auto_thumbnail == 1;
    }

    /// <summary>
    /// 썸네일 자동 촬영 
    /// </summary>
    /// <param name="value"></param>
    public void AutoThumbnailPickValueChangeOn(bool value)
    {
        Color color = value ? new Color32(135,135,135,255) : new Color32(235, 235, 235, 255);
        toggle.targetGraphic.color = color;

        if (MeumSocket.Get() != null)
        {
            AutoThumbnailRequest gestureRequest = new AutoThumbnailRequest()
            {
                requestStatus = 1,
                uid = MeumDB.Get().myRoomInfo.owner.user_id,
                is_auto_thumbnail = value,
                successOn = ResultData =>
                {
                    AutoThumbnailData data = (AutoThumbnailData)ResultData;
                    MeumDB.Get().myRoomInfo.is_auto_thumbnail = data.result ? 1 : 0;
                }
            };
            gestureRequest.RequestOn();
        }
    }

    /// <summary>
    /// 저장 하기 버튼이 눌렸을때 호출
    /// </summary>
    public void SaveBtnClick()
    {
        if (MeumDB.Get().myRoomInfo.is_auto_thumbnail == 0)
            return;
        
        StartCoroutine(ScreenShowOn());
    }

    IEnumerator ScreenShowOn()
    {
        allUIPanel.gameObject.SetActive(false);

        yield return ScreenShotManager.Instance.ScreenShowOn(false);

        allUIPanel.gameObject.SetActive(true);
    }

}
