using Core;
using Core.Socket;
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

}
