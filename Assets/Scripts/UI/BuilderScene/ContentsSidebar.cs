using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI.BuilderScene
{
    /*
     * @brief Build scene에 있는 사이드바를 담당하는 컴포넌트 (2d, 3d, banner state 관리)
     */
    public class ContentsSidebar : MonoBehaviour
    {
        #region SerializeFields
        
        [SerializeField] private RectTransform container2d;
        [SerializeField] private RectTransform container3d;
        [SerializeField] private BannerSetting bannerSetting;

        [SerializeField] private float containerTopDistOnBannerState = 200.0f;

        [Header("Background Objects")]
        [SerializeField] private GameObject object2dBG;

        [SerializeField] private GameObject bannerBG;
        [SerializeField] private GameObject object3dBG;
        
        #endregion

        #region PrivateFields
        
        private enum State
        {
            Object2D,
            Banner,
            Object3D
        }
        private float _defaultContainerTopDist = 0.0f;
        private State _state = State.Object2D;
        private Image _background;

        #endregion

        private void Start()
        {
            _background = GetComponent<Image>();
            _defaultContainerTopDist = container2d.offsetMax.y;
            Object2DState();
        }

        public void Object2DState()
        {
            container2d.gameObject.SetActive(true);
            container3d.gameObject.SetActive(false);
            bannerSetting.gameObject.SetActive(false);

            object2dBG.SetActive(true);
            object3dBG.SetActive(false);
            bannerBG.SetActive(false);
            container2d.offsetMax = new Vector2(container2d.offsetMax.x, _defaultContainerTopDist);

            _state = State.Object2D;
        }

        public void BannerState(UI.Content content)
        {
            container2d.gameObject.SetActive(true);
            container3d.gameObject.SetActive(false);
            bannerSetting.gameObject.SetActive(true);

            object2dBG.SetActive(false);
            object3dBG.SetActive(false);
            bannerBG.SetActive(true);
            container2d.offsetMax = new Vector2(container2d.offsetMax.x, -containerTopDistOnBannerState);

            bannerSetting.SetSelectedContent(content);

            _state = State.Banner;
        }

        public void Object3DState()
        {
            container2d.gameObject.SetActive(false);
            container3d.gameObject.SetActive(true);
            bannerSetting.gameObject.SetActive(false);

            object2dBG.SetActive(false);
            object3dBG.SetActive(true);
            bannerBG.SetActive(false);
            container2d.offsetMax = new Vector2(container2d.offsetMax.x, _defaultContainerTopDist);

            _state = State.Object3D;
        }
    }
}
