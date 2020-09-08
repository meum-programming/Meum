using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace UI.Shop
{
    public class ContentData
    {
        public int id;
        public string name;
        public int like;
        public int hate;
        public Texture2D thumbnail;
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
                var spriteRect = new Rect(0, 0, _data.thumbnail.width, _data.thumbnail.height);
                image.sprite = Sprite.Create(_data.thumbnail, spriteRect, new Vector2(0.5f, 0.5f));
            }
        }
        
        private ContentData _data;
    }
}
