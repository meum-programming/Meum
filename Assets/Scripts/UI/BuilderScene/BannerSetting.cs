using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace UI.BuilderScene
{
    /*
     * @brief Sidebar의 banner 설정창을 담당하는 컴포넌트
     */
    public class BannerSetting : MonoBehaviour
    {
        [SerializeField] private InputField urlInputField;
        [SerializeField] private Content selectedContent;
        [SerializeField] private Button applyBtn;
        
        private void Awake()
        {
            applyBtn.onClick.AddListener(Apply);
        }
    
        public void SetSelectedContent(Content content)
        {
            selectedContent.data = content.data;
        }

        private void Apply()
        {
            var placer = GameObject.Find("Artworks").GetComponent<Game.Artwork.ArtworkPlacer>();
            Assert.IsNotNull(placer);
            placer.CreateSelectedBanner(selectedContent, urlInputField.text);
        }
    }
}
