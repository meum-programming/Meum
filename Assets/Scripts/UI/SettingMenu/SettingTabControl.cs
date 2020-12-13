using System;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UI.SettingMenu
{
    /*
     * @brief 환경설정에서 탭을 관리(버튼클릭시 전환 담당)
     */
    public class SettingTabControl : MonoBehaviour
    {
        #region SerializeFields

        [SerializeField] private Button[] buttons;
        [SerializeField] private Text[] buttonTexts;
        [SerializeField] private GameObject[] tabs;

        [Header("Button Color")] 
        [SerializeField] private Color activatedColor;
        [SerializeField] private Color activatedTextColor;
        [SerializeField] private Color deactivatedColor;
        [SerializeField] private Color deactivatedTextColor;
        
        #endregion

        private void Awake()
        {
            Assert.AreEqual(buttons.Length, buttonTexts.Length);
            Assert.AreEqual(buttons.Length, tabs.Length);
            
            for (var i = 0; i < buttons.Length; ++i)
            {
                var temp = i;
                buttons[i].onClick.AddListener(() => ChangeTab(temp));
            }
            
            ChangeTab(0);
        }

        private void ChangeTab(int idx)
        {
            for (var i = 0; i < tabs.Length; ++i)
            {
                Assert.IsNotNull(tabs[i]);
                Assert.IsNotNull(buttons[i]);
                tabs[i].SetActive(false);
                buttons[i].image.color = deactivatedColor;
                if(!ReferenceEquals(buttonTexts[i], null))
                    buttonTexts[i].color = deactivatedTextColor;
            }
            
            tabs[idx].SetActive(true);
            buttons[idx].image.color = activatedColor;
            if(!ReferenceEquals(buttonTexts[idx], null))
                buttonTexts[idx].color = activatedTextColor;
        }
    
        public void Show() {
            gameObject.SetActive(true);
        }

        public void Close()
        {
            gameObject.SetActive(false);
        }
    }
}