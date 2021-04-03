using Core;
using Core.Socket;
using Game.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GalleryController : MonoBehaviour
{

    [SerializeField] private List<RectTransform>  editBtnList = new List<RectTransform>();

    private void Awake()
    {
        Init();
    }

    void Init()
    {
        BGMSet();
        SkyBoxSet();

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

    public void CameraWwitchingBtnClick()
    {
        if (FindObjectOfType<LocalPlayerCamera>())
        {
            FindObjectOfType<LocalPlayerCamera>().OnSwitchView();
        }
    }

}
