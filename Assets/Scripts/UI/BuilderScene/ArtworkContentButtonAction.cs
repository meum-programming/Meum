using Game.Artwork;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace UI.BuilderScene
{
    /*
     * @brief BuilderScene의 사이드바에 있는 Artwork Content 들을 클릭했을 때 불러지는 트리거이벤트
     * @details Click 할 때 Artwork를 생성함
     */
    [RequireComponent(typeof(Button))]
    public class ArtworkContentButtonAction : MonoBehaviour
    {
        [SerializeField] private bool is3D = false;

        private void Awake()
        {
            GetComponent<Button>().onClick.AddListener(CreateArtwork);
        }

        private void CreateArtwork()
        {
            var sidebar = GameObject.Find("ContentsSidebar").GetComponent<ContentsSidebar>();
            Assert.IsNotNull(sidebar);
            
            var placer = GameObject.Find("Artworks").GetComponent<ArtworkPlacer>();
            Assert.IsNotNull(placer);
            
            if (is3D)
            {
                sidebar.Object3DState();
                placer.CreateSelected(GetComponent<ContentViewer.Content>());
            }
            else 
            { 
                sidebar.Object2DState();
                placer.CreateSelected(GetComponent<ContentViewer.Content>());
            }
        }
    }
}
