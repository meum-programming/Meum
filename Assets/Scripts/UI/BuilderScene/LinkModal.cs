using TMPro;
using UnityEngine;
using UnityEngine.Assertions;

namespace UI.BuilderScene
{
    public class LinkModal : MonoBehaviour
    {
        [SerializeField] private TMP_InputField urlInputField;
        [SerializeField] private GameObject checkObject;
        
        private Game.Artwork.ArtworkInfo _artworkInfo;

        private void Awake()
        {
            Assert.IsNotNull(urlInputField);
            Assert.IsNotNull(checkObject);
        }

        public void Show(Game.Artwork.ArtworkInfo artworkInfo)
        {
            _artworkInfo = artworkInfo;
            urlInputField.text = _artworkInfo.BannerUrl;
            if(!_artworkInfo.BannerUrl.Equals(""))
                checkObject.SetActive(true);
            gameObject.SetActive(true);
        }

        public void Close()
        {
            if (checkObject.activeSelf)
            {
                _artworkInfo.BannerUrl = urlInputField.text;
            }
            gameObject.SetActive(false);
        }

        public void ToggleLink()
        {
            Assert.IsNotNull(_artworkInfo);
            if (checkObject.activeSelf)
            {
                _artworkInfo.BannerUrl = "";
                checkObject.SetActive(false);
            }
            else
            {
                _artworkInfo.BannerUrl = urlInputField.text;
                checkObject.SetActive(true);
            }
        }
    }
}