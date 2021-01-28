using Core;
using UnityEngine;
using UnityEngine.Assertions;

namespace Game
{
    /*
     * @brief UI.SettingMenu.VolumeSliders 값에 따라 bgm 소리 크기를 조절해주는 컴포넌트 
     */
    [RequireComponent(typeof(AudioSource))]
    public class BgmVolumeControl : MonoBehaviour
    {
        private void Awake()
        {
            //int bgmIndex = MeumDB.Get().currentRoomInfo.bgm_type_int;
            //SoundManager.Instance.PlayBGM((BGMEnum)bgmIndex);
        }
        
    }
}
