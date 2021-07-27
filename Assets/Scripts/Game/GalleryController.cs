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

    Texture2D ScreenShotReSizing(Texture2D texture2D)
    {
        float width = texture2D.width;
        float height = texture2D.height;

        int newWidth = 1024;
        int newHeiht = 1024;

        if (width > height)
        {
            newHeiht = Mathf.RoundToInt((height * 1024) / width);
        }
        else
        {
            newWidth = Mathf.RoundToInt((width * 1024) / height);
        }

        Texture2D reSizeTexture2D = ScaleTexture(texture2D, newWidth, newHeiht);

        //카드 비율 420 : 240 의 이미지가 나올수 있도록 크롭 한다.
        int check_w_Rate = Mathf.RoundToInt(newWidth / 7);
        int check_h_Rate = Mathf.RoundToInt(newHeiht / 4);

        int cardNewWidth = 0;
        int cardNewHeiht = 0;

        if (check_w_Rate > check_h_Rate)
        {
            cardNewHeiht = newHeiht;
            cardNewWidth = Mathf.RoundToInt(newHeiht * 420 / 240);
        }
        else
        {
            cardNewWidth = newWidth;
            cardNewHeiht = Mathf.RoundToInt(newWidth * 240 / 420);
        }

#if dev
        Debug.LogWarning($"newHeiht = {newHeiht} , newWidth = {newWidth} , cardNewHeiht = {cardNewHeiht} , cardNewWidth = {cardNewWidth}");
#endif

        Texture2D cropTexture =  CropTexture(reSizeTexture2D, cardNewWidth, cardNewHeiht);

        image.texture = cropTexture;

        return cropTexture;
    }

    private Texture2D ScaleTexture(Texture2D source, int targetWidth, int targetHeight)
    {
        Texture2D result = new Texture2D(targetWidth, targetHeight, source.format, true);
        Color[] rpixels = result.GetPixels(0);
        float incX = (1.0f / (float)targetWidth);
        float incY = (1.0f / (float)targetHeight);
        for (int px = 0; px < rpixels.Length; px++)
        {
            rpixels[px] = source.GetPixelBilinear(incX * ((float)px % targetWidth), incY * ((float)Mathf.Floor(px / targetWidth)));
        }
        result.SetPixels(rpixels, 0);
        result.Apply();
        return result;
    }

    private Texture2D CropTexture(Texture2D source, int targetWidth, int targetHeight)
    {
        Texture2D result = new Texture2D(targetWidth, targetHeight, source.format, true);

        int startX = 0;
        int startY = 0;

        if (source.width == targetWidth)
        {
            startY = Mathf.RoundToInt((source.height - targetHeight) / 2);
        }
        else
        {
            startX = Mathf.RoundToInt((source.width - targetWidth) / 2);
        }

        Color[] c = source.GetPixels(startX, startY, targetWidth, targetHeight);
        result.SetPixels(c);

        result.Apply();
        return result;
    }

}
