using System;
using System.Collections;
using System.Collections.Generic;
using UI;
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
        #region SerializeFields
        
        [SerializeField] private UITogglePosition togglePosition;

        #endregion
        
        #region PublicFields
        
        public bool toggled => transform.localScale.x < 0;
        
        #endregion
        
        #region PrivateFields
        
        private Button _button;
        
        #endregion

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
