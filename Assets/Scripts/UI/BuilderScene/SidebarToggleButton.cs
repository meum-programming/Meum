using UnityEngine;
using UnityEngine.UI;

namespace UI.BuilderScene
{
    /*
     * @brief Build scene에 있는 Sidebar를 토글하는 버튼 담당 컴포넌트
     */
    [RequireComponent(typeof(Button))]
    public class SidebarToggleButton : MonoBehaviour
    {
        [SerializeField] private TogglePosition togglePosition;

        [SerializeField] private Image iconImage;

        public bool toggled = false;

        private Button _button;

        public void InvokeButton()
        {
            _button.onClick.Invoke();
        }

        public void SetButtonEnable(bool v)
        {
            _button.enabled = v;
        }
        
        private void Awake()
        {
            _button = GetComponent<Button>();
            _button.onClick.AddListener(ButtonAction);
        }

        private void ButtonAction()
        {
            ToggleChange(!toggled);
        }

        public void ToggleChange(bool toggled)
        {
            this.toggled = toggled;

            togglePosition.Toggle(this.toggled);

            float rotZValue = this.toggled ? 0 : 180;
            iconImage.transform.rotation = Quaternion.Euler(new Vector3(0, 0, rotZValue));
        }
    }
}
