using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.Video;

namespace UI
{
    public class ContentData
    {
        public int id;
        public string name;
        public int like;
        public int hate;
        public string url;
        public string thumbnail_url;
    }
    public class Content : MonoBehaviour
    {
        public Text name;
        public Image image;

        public ContentData data
        {
            get { return _data; }
            set
            {
                _data = value;
                name.text = _data.name;
                StartCoroutine(LoadImage());
            }
        }
        
        private ContentData _data;

        private IEnumerator LoadImage()
        {
            var textureGetter = Global.MeumDB.Get().GetTexture(_data.thumbnail_url);
            yield return textureGetter.coroutine;
            var texture = textureGetter.result as Texture2D;
            if (texture == null) yield break;
            var spriteRect = new Rect(0, 0, texture.width, texture.height);
            image.sprite = Sprite.Create(texture, spriteRect, new Vector2(0.5f, 0.5f));
        }
    }
}
