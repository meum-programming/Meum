using Core;
using Core.Socket;
using Game;
using Game.Player;
using System;
using System.Collections;
using System.Collections.Generic;
using UI.ChattingUI;
using UnityEngine;
using UnityEngine.UI;

public class GalleryController : MonoBehaviour
{

    [SerializeField] private List<RectTransform>  editBtnList = new List<RectTransform>();

    [SerializeField] SoundToggleButton soundToggleButton;
    [SerializeField] MouseToggleButton mouseToggleButton;
    [SerializeField] GestureController gestureController;
    [SerializeField] ChattingUI chattingUI;

    [SerializeField] List<RectTransform> hideUIList = new List<RectTransform>();

    [SerializeField] Image hideBtnImage;
    [SerializeField] List<Sprite> hideSpriteList = new List<Sprite>();

    bool hideOn = false;

    [SerializeField] List<RectTransform> screenShotHidUIList = new List<RectTransform>();
    [SerializeField] RawImage image;

    private void Awake()
    {
        Init();
    }

    void Init()
    {
        BGMSet();
        //SkyBoxSet();

        if (editBtnList.Count > 0)
        {
            bool isOwnerRoom = MeumDB.Get().myRoomInfo.owner.user_id == MeumDB.Get().currentRoomInfo.owner.user_id;

            //유니티 애디터 상태가 아니라면
#if !UNITY_EDITOR
            //게스트로 접속했다면
            if (MeumDB.Get().currentRoomInfo.owner.user_id == 61)
            {
                isOwnerRoom = false;
            }
#endif


            for (int i = 0; i < editBtnList.Count; i++)
            {
                editBtnList[i].gameObject.SetActive(isOwnerRoom);
            }
        }
    }

    public void BGMSet()
    {
        int bgmIndex = MeumDB.Get().currentRoomInfo.bgm_type_int;
        SoundManager.Instance.PlayBGM(bgmIndex);
    }

    public void SkyBoxSet()
    {
        int index = MeumDB.Get().currentRoomInfo.sky_type_int;
        SkyBoxSaveData skydata = Resources.Load<MeumSaveData>("MeumSaveData").GetSKYData((SkyBoxEnum)index);

        if (skydata != null)
        {
            RenderSettings.skybox = skydata.material;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        if (DataManager.Instance.roomSaveOn)
        {
            DataManager.Instance.roomSaveOn = false;
            ScreenShowBtnClick(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GoChaEditBtnClick()
    {
        MeumSocket.Get().GoToChaEditScene();
    }

    public void SoundShowToggle()
    {
        soundToggleButton.ButtonAction();

        if (soundToggleButton.showOn)
        {
            if (mouseToggleButton.showOn)
            {
                mouseToggleButton.ButtonAction();
            }

            gestureController.PanelOpenSet(false);
        }
    }

    public void MouseShowToggle()
    {
        mouseToggleButton.ButtonAction();

        if (mouseToggleButton.showOn)
        {
            if (soundToggleButton.showOn)
            {
                soundToggleButton.ButtonAction();
            }

            gestureController.PanelOpenSet(false);
        }

        //showOn = !showOn;

        //float yValue = showOn ? 1 : -90;
        //sliderPanel.DOAnchorPosY(yValue, 0.3f);

        //if (showOn)
        //{
        //    if (mouseToggleButton.showOn)
        //    {
        //        mouseToggleButton.ButtonAction();
        //    }

        //    gestureController.PanelOpenSet(false);
        //}
    }

    public void GestureShowToggle()
    {
        gestureController.OnPointerClick();

        if (gestureController.panelOpenOn)
        {
            if (mouseToggleButton.showOn)
            {
                mouseToggleButton.ButtonAction();
            }

            if (soundToggleButton.showOn)
            {
                soundToggleButton.ButtonAction();
            }
        }
    }

    public void AllHideBtnClick()
    {
        AllHideOn(!hideOn);
    }

    void AllHideOn(bool hideOn)
    {
        this.hideOn = hideOn;

        if (hideOn)
        {
            if (mouseToggleButton.showOn)
            {
                mouseToggleButton.ButtonAction();
            }

            if (soundToggleButton.showOn)
            {
                soundToggleButton.ButtonAction();
            }

            chattingUI.ShowOn(false);

        }

        hideBtnImage.sprite = hideOn ? hideSpriteList[1] : hideSpriteList[0];

        foreach (var obj in hideUIList)
        {
            obj.gameObject.SetActive(!hideOn);
        }
    }


    public void ScreenShowBtnClick(bool isScreenShot)
    {
        StartCoroutine(ScreenShowOn(isScreenShot));
    }

    IEnumerator ScreenShowOn(bool isScreenShot)
    {
        bool firstHideOn = hideOn;

        if (!hideOn)
        {
            AllHideOn(true);
        }

        foreach (var obj in screenShotHidUIList)
        {
            obj.gameObject.SetActive(false);
        }

        yield return ScreenShotManager.Instance.ScreenShowOn(isScreenShot);

        if (isScreenShot)
        {
            yield return new WaitForSeconds(1);
        }
        
        foreach (var obj in screenShotHidUIList)
        {
            obj.gameObject.SetActive(true);
        }

        AllHideOn(firstHideOn);

    }

}
