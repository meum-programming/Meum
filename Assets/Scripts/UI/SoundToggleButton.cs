using UnityEngine;
using UnityEngine.UI;

public class SoundToggleButton : MonoBehaviour
{
    [SerializeField] private Sprite enabledImage;
    [SerializeField] private Sprite disabledImage;
    
    private bool _mute = false;
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
        if (_mute)
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
        AudioListener.volume = 1.0f;
        _image.sprite = enabledImage;
        _mute = false;
    }

    private void Mute()
    {
        AudioListener.volume = 0.0f;
        _image.sprite = disabledImage;
        _mute = true;
    }
    
}
