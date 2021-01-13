using Core;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace UI.SettingMenu
{
    /*
     * @beief 환경설정에 있는 볼륨관리 부분 컴포넌트
     */
    public class VolumeSliders : Singleton<VolumeSliders>
    {
        #region SerializeFields

        [SerializeField] private Slider masterSlider;
        [SerializeField] private Slider effectSlider;
        [SerializeField] private Slider bgmSlider;
        
        #endregion
        
        #region PublicFields
        
        public float MasterVolume { get; private set; }
        public float EffectVolume { get; private set; }
        public float BgmVolume { get; private set; }
        
        #endregion
        
        private void Awake()
        {
            base.Awake();
            
            Assert.IsNotNull(masterSlider);
            Assert.IsNotNull(effectSlider);
            Assert.IsNotNull(bgmSlider);
            
            masterSlider.onValueChanged.AddListener(OnMasterSliderValueChanged);
            effectSlider.onValueChanged.AddListener(OnEffectSliderValueChanged);
            bgmSlider.onValueChanged.AddListener(OnBGMSliderValueChanged);

            AudioListener.volume = MasterVolume = masterSlider.value;
            EffectVolume = effectSlider.value;
            BgmVolume = bgmSlider.value;
        }

        private void OnMasterSliderValueChanged(float val)
        {
            AudioListener.volume = val;
        }

        private void OnEffectSliderValueChanged(float val)
        {
            Debug.Log(val);
            EffectVolume = val;
        }

        private void OnBGMSliderValueChanged(float val)
        {
            BgmVolume = val;
        }
    }
}
