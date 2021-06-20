using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class MouseToggleButton : MonoBehaviour
{
    [SerializeField] private RectTransform sliderPanel;
    [SerializeField] private Slider slider;
    [SerializeField] private SoundToggleButton soundToggleButton;
    [SerializeField] private GestureController gestureController;
    
    public bool showOn = false;

    private void Awake()
    {
        slider.value = DataManager.Instance.mouseSensitivityValue;
    }

    public void ButtonAction()
    {
        showOn = !showOn;

        float yValue = showOn ? 1 : -90;
        sliderPanel.DOAnchorPosY(yValue, 0.3f);
    }

    public void SliderChange(float value)
    {
        DataManager.Instance.SetMouseSensitivityValue(value);
    }

}