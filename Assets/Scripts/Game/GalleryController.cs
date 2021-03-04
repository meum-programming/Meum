using Core;
using Core.Socket;
using Game.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GalleryController : MonoBehaviour
{
    [SerializeField] private RectTransform editBtn;

    private void Awake()
    {
        Init();
    }

    void Init()
    {
        BGMSet();
        SkyBoxSet();

        if (editBtn != null)
        {
            bool isOwnerRoom = MeumDB.Get().myRoomInfo.owner.id == MeumDB.Get().currentRoomInfo.owner.id;
            editBtn.gameObject.SetActive(isOwnerRoom);
        }
    }

    public void BGMSet()
    {
        int bgmIndex = MeumDB.Get().currentRoomInfo.bgm_type_int;
        SoundManager.Instance.PlayBGM((BGMEnum)bgmIndex);
    }

    public void SkyBoxSet()
    {
        int index = MeumDB.Get().currentRoomInfo.sky_type_int;
        SkyBoxSaveData skydata = Resources.Load<MeumSaveData>("MeumSaveData").GetSKYData((SkyBoxEnum)index);
        RenderSettings.skybox = skydata.material;
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GoChaEditBtnClick()
    {
        MeumSocket.Get().GoToChaEditScene();
    }

    public void PlayGestureAnim(int status)
    {
        string animName = string.Empty;

        switch (status)
        {
            case 0:
                animName = "Gesture_BigWave";
                break;
            case 1:
                animName = "Gesture_Clap";
                break;
            case 2:
                animName = "Gesture_No";
                break;
            case 3:
                animName = "Gesture_Yes";
                break;
            case 4:
                animName = "Gesture_Pointing";
                break;
            case 5:
                animName = "Gesture_Thankful";
                break;
            case 6:
                animName = "Gesture_Sad";
                break;
        }

        if (animName != string.Empty)
        {
            PlayerAnimationController playerAnimationController = DataSynchronizer.Get().GetLocalPlayer().GetComponent<PlayerAnimationController>();
            playerAnimationController.PlayGestureAnim(animName);
        }
    }

    public void GestureSettingPopupOpen(bool open)
    {
        Debug.LogWarning(open);
    }

}
