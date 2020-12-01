using System.Collections;
using Game.Artwork;
using UnityEngine;
using UnityEngine.Assertions;

namespace UI.BuilderScene
{
    /*
     * @brief BuilderScene의 사이드바에 있는 Artwork Content 들을 클릭했을 때 불러지는 트리거이벤트
     * @details PointerUp 이벤트에 Artwork를 생성함
     * 누르고 있는 시간이 pressingTimeForMakeBanner 이상일 경우 배너로 만듬
     */
    public class ArtworkContentTriggerEvents : MonoBehaviour
    {
        #region SerializedFields
        
        [SerializeField] private float pressingTimeForMakeBanner = 0.0f;
        [SerializeField] private bool is3D = false;
        
        #endregion

        #region PrivateFields
        
        private bool _pointerIn = false;
        private float _pressingTime = 0.0f;
        private IEnumerator _timeRecording = null;
        
        #endregion
        
        /*
         * @brief 누르는 시간을 기록하기 위한 코루틴
         */
        private IEnumerator TimeRecording()
        {
            _pressingTime = 0.0f;
            while (true)
            {
                _pressingTime += Time.deltaTime;
                yield return null;
            }
        }

        #region Event Triggers 
        
        public void PointerEnter()
        {
            _pointerIn = true;
        }

        public void PointerExit()
        {
            _pointerIn = false;
        }

        public void PointerDown()
        {
            if (!ReferenceEquals(_timeRecording, null))
            {
                StopCoroutine(_timeRecording);
                _timeRecording = null;
            }

            if (!is3D)
                StartCoroutine(_timeRecording = TimeRecording());
        }

        public void PointerUp()
        {
            if (!ReferenceEquals(_timeRecording, null))
            {
                StopCoroutine(_timeRecording);
                _timeRecording = null;
            }

            if (!_pointerIn) return;

            if (is3D)
            {
                var sidebar = GameObject.Find("ContentsSidebar").GetComponent<ContentsSidebar>();
                Assert.IsNotNull(sidebar);
                sidebar.Object3DState();
                
                var placer = GameObject.Find("Artworks").GetComponent<ArtworkPlacer>();
                Assert.IsNotNull(placer);
                placer.CreateSelected(GetComponent<Content>());
            }
            else 
            { 
                if (_pressingTime < pressingTimeForMakeBanner)
                {
                    var sidebar = GameObject.Find("ContentsSidebar").GetComponent<ContentsSidebar>();
                    Assert.IsNotNull(sidebar);
                    sidebar.Object2DState();

                    var placer = GameObject.Find("Artworks").GetComponent<ArtworkPlacer>();
                    Assert.IsNotNull(placer);
                    placer.CreateSelected(GetComponent<UI.Content>());
                }
                else
                {
                    var sidebar = GameObject.Find("ContentsSidebar").GetComponent<ContentsSidebar>();
                    Assert.IsNotNull(sidebar);
                    sidebar.BannerState(GetComponent<Content>());
                }
            }
        }
        
        #endregion
    }
}
