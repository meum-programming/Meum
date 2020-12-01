using UnityEngine;
using UnityEngine.UI;

namespace UI.GalleryScene
{
    [RequireComponent(typeof(Slider))]
    public class SoundSlider : MonoBehaviour
    {
        private void Awake()
        {
            var slider = GetComponent<Slider>();
            slider.onValueChanged.AddListener(OnSliderValueChanged);

            AudioListener.volume = slider.value;
        }

        private void OnSliderValueChanged(float val)
        {
            AudioListener.volume = val;
        }
    }
}
