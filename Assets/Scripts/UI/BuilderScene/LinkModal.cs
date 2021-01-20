using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;

namespace UI.BuilderScene
{
    public class LinkModal : MonoBehaviour
    {
        [SerializeField] private TMP_InputField urlInputField;
        [SerializeField] private GameObject checkObject;
        [SerializeField] private InputActionAsset playerInput;
        
        private Game.Artwork.ArtworkInfo _artworkInfo;

        private void Awake()
        {
            Assert.IsNotNull(urlInputField);
            Assert.IsNotNull(checkObject);
            Assert.IsNotNull(playerInput);
        }

        public void Show(Game.Artwork.ArtworkInfo artworkInfo)
        {
            _artworkInfo = artworkInfo;
            urlInputField.text = _artworkInfo.bannerUrl;
            
            checkObject.SetActive(false);
            if(!_artworkInfo.bannerUrl.Equals(""))
                checkObject.SetActive(true);
            
            playerInput.Disable();
            gameObject.SetActive(true);
        }

        public void Close()
        {
            Assert.IsNotNull(_artworkInfo);
            if (checkObject.activeSelf)
                _artworkInfo.bannerUrl = CheckProtocolAndAddHttpIfNoProtocol(urlInputField.text);
            else
                _artworkInfo.bannerUrl = "";
            
            playerInput.Enable();
            gameObject.SetActive(false);
        }

        public void ToggleLink()
        {
            if (checkObject.activeSelf)
                checkObject.SetActive(false);
            else
                checkObject.SetActive(true);
        }

        private string CheckProtocolAndAddHttpIfNoProtocol(string url)
        {
            if (!url.Contains("://"))
                url = "http://" + url;
            return url;
        }
    }
}