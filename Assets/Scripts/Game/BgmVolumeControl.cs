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
        #region PrivateFields
        
        private AudioSource _audio;
        
        #endregion

        private void Awake()
        {
            _audio = GetComponent<AudioSource>();
            Assert.IsNotNull(_audio);
        }
        
        private void Start()
        {
            Update();
        }

        private void Update()
        {
            var volumeSliders = UI.SettingMenu.VolumeSliders.Get();
            if(volumeSliders)
                _audio.volume = volumeSliders.BgmVolume;
        }
    }
}
