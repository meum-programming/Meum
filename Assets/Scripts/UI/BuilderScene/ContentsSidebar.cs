using Core.Socket;
using Game.Player;
using System.Collections.Generic;
using UnityEngine;

namespace UI.BuilderScene
{
    /*
     * @brief Build scene에 있는 사이드바를 담당하는 컴포넌트 (2d, 3d, banner state 관리)
     */
    public class ContentsSidebar : MonoBehaviour
    {
        [SerializeField] private RectTransform container2d;
        [SerializeField] private RectTransform container3d;

        private float _defaultContainerTopDist = 0.0f;

        int mainTabStatus = 0;

        [SerializeField] List<RectTransform> mainTabPanelList = new List<RectTransform>();

        int subTabStatus = 0;
        [SerializeField] List<RectTransform> subTabPanelList = new List<RectTransform>();

        private void Start()
        {
            //_defaultContainerTopDist = container2d.offsetMax.y;
            //Object2DState();
            MainTabActiveSet();
        }

        public void Object2DState()
        {
            container2d.gameObject.SetActive(true);
            container3d.gameObject.SetActive(false);

            container2d.offsetMax = new Vector2(container2d.offsetMax.x, _defaultContainerTopDist);
        }
        
        public void Object3DState()
        {
            container2d.gameObject.SetActive(false);
            container3d.gameObject.SetActive(true);

            container3d.offsetMax = new Vector2(container3d.offsetMax.x, _defaultContainerTopDist);

        }

        public void TabChange(int status)
        {
            mainTabStatus = status;
            MainTabActiveSet();
            
        }

        void MainTabActiveSet()
        {
            for (int i = 0; i < mainTabPanelList.Count; i++)
            {
                mainTabPanelList[i].gameObject.SetActive(i == mainTabStatus);
            }

            if (mainTabStatus == 0)
            {
                SubTabActiveSet();
            }

        }

        public void SubTabChange(int status)
        {
            subTabStatus = status;
            SubTabActiveSet();
        }

        void SubTabActiveSet()
        {
            for (int i = 0; i < subTabPanelList.Count; i++)
            {
                subTabPanelList[i].gameObject.SetActive(i == subTabStatus);
            }

        }

        public void ChangeStartPoint()
        {
            Transform playerTransfrom = DataSynchronizer.Get().GetLocalPlayer();

            Debug.LogWarning("ChangeStartPoint"+ playerTransfrom.localPosition);
        }

        public void ChangeStartPointClear()
        {
            Debug.LogWarning("ChangeStartPointClear");
        }

        public void ShowTopCamera()
        {
            Debug.LogWarning("ShowTopCamera");
        }

        public void ShowTopCameraToStartPoint()
        {
            Debug.LogWarning("ShowTopCameraToStartPoint");
        }


    }
}
