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
        public RawImage image;

        public ContentData data
        {
            get { return _data; }
            set
            {
                _data = value;
                name.text = _data.name;
                LoadImage();
            }
        }
        
        private ContentData _data;

        private void LoadImage()
        {
            var texture = Global.MeumDB.Get().GetTexture(_data.thumbnail_url);
            image.texture = texture;
        }
    }
}
