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

        private void Start()
        {
            _defaultContainerTopDist = container2d.offsetMax.y;
            Object2DState();
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

            container2d.offsetMax = new Vector2(container2d.offsetMax.x, _defaultContainerTopDist);
        }
    }
}
