using Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class Content : MonoBehaviour
    {
        public TextMeshProUGUI name;
        public RawImage image;

        public MeumDB.ArtworkInfo data
        {
            get { return _data; }
            set
            {
                _data = value;
                name.text = _data.title;
                LoadImage();
            }
        }
        
        private MeumDB.ArtworkInfo _data;

        private void LoadImage()
        {
            if (null == _data.thumbnail) return;
            var texture = MeumDB.Get().GetTexture(_data.thumbnail);
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
