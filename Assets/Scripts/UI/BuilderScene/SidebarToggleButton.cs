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

        public bool toggled => transform.localScale.x < 0;

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
            togglePosition.Toggle();

            var scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;
        }
    }
}
