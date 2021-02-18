using Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.ContentViewer
{
    public class Content : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI title;
        [SerializeField] private RawImage image;
        [SerializeField] private int titleMaxLength;

        public MeumDB.ArtworkInfo Data
        {
            get { return _data; }
            set
            {
                _data = value;
                var titleText = _data.title;
                if (titleText.Length >= titleMaxLength)
                {
                    titleText = titleText.Substring(0, titleMaxLength);
                    titleText += "...";
                }
                title.text = titleText;
                LoadImage();
            }
        }
        public RawImage Image => image;
        
        private MeumDB.ArtworkInfo _data;

        private void LoadImage()
        {
            if (null == _data.thumbnail) return;

            string baseURL = "https://api.meum.me/datas/";

            var texture = MeumDB.Get().GetTexture(baseURL+_data.thumbnail);
            image.texture = texture;

            var imageTransform = image.rectTransform;
            var localScale= imageTransform.localScale;

            if (texture.width > texture.height)
                localScale.y *= (float) texture.height / texture.width;
            else
                localScale.x *= (float) texture.width / texture.height;
            imageTransform.localScale = localScale;

        }
    }
}
