using UnityEngine;
using UnityEngine.UI;

public class SoundToggleButton : MonoBehaviour
{
    [SerializeField] private Sprite enabledImage;
    [SerializeField] private Sprite disabledImage;
    
    public bool muted { get; private set; } = false;
    private Image _image;

    private void Awake()
    {
        var btn = GetComponent<Button>();
        btn.onClick.AddListener(ButtonAction);

        _image = GetComponent<Image>();

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
        // AudioListener.volume = 1.0f;
        AudioListener.pause = false;
        _image.sprite = enabledImage;
        muted = false;
    }

    private void Mute()
    {
        // AudioListener.volume = 0.0f;
        AudioListener.pause = true;
        _image.sprite = disabledImage;
        muted = true;
    }
    
}
