using UnityEngine;
using UnityEngine.UI;

namespace UI.GalleryScene
{
    public class SoundToggleButton : MonoBehaviour
    {
        [SerializeField] private Sprite enabledImage;
        [SerializeField] private Sprite disabledImage;
        [SerializeField] private Image image;

        public bool muted { get; private set; } = false;

        private void Awake()
        {
            var btn = GetComponent<Button>();
            btn.onClick.AddListener(ButtonAction);

            if (AudioListener.volume < 1e-3)
                Mute();
            else
                Unmute();
        }

        private void ButtonAction()
        {
            if (muted)
            {
                Unmute();
            }
            else
            {
                Mute();
            }
        }

        private void Unmute()
        {
            AudioListener.pause = false;
            image.sprite = enabledImage;
            muted = false;
        }

        private void Mute()
        {
            AudioListener.pause = true;
            image.sprite = disabledImage;
            muted = true;
        }
    }
}
