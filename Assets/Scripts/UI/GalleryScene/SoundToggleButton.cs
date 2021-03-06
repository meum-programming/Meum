using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class SoundToggleButton : MonoBehaviour
{
    [SerializeField] private RectTransform sliderPanel;
    [SerializeField] private Slider slider;
    [SerializeField] private MouseToggleButton mouseToggleButton;
    [SerializeField] private GestureController gestureController;
    public bool showOn = false;

    private void Awake()
    {
        slider.value = AudioListener.volume;
    }

    public void ButtonAction()
    {
        showOn = !showOn;

        float yValue = showOn ? 1 : -90;
        sliderPanel.DOAnchorPosY(yValue, 0.3f);

        if (showOn)
        {
            if (mouseToggleButton.showOn)
            {
                mouseToggleButton.ButtonAction();
            }

            gestureController.PanelOpenSet(false);
        }
    }

    public void SliderChange(float value)
    {
        AudioListener.volume = value;
    }

}