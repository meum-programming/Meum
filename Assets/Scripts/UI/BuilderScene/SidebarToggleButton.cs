using System;
using System.Collections;
using System.Collections.Generic;
using UI;
using UnityEngine;
using UnityEngine.UI;

namespace UI.BuilderScene
{
    public class SidebarToggleButton : MonoBehaviour
    {
        [SerializeField] private UITogglePosition togglePosition;

        private Button _button;
        
        public bool toggled => transform.localScale.x > 0;

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
