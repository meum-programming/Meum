using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UI;
using UnityEngine;
using UnityEngine.Networking;

namespace Gallery.Builder
{
    public class ArtworksLoader : MonoBehaviour
    {
        [SerializeField] private ContentsContainer container;

        private MeumDB.ArtworkInfo[] _artworkInfos;

        private void Start()
        {
            StartCoroutine(LoadArtworks());
        }

        private IEnumerator LoadArtworks()
        {
            var cd = new CoroutineWithData(this, MeumDB.Get().GetUserInfo());
            yield return cd.coroutine;
            var userInfo = cd.result as MeumDB.UserInfo;
            
            Debug.Assert(userInfo != null);
            cd = new CoroutineWithData(this, MeumDB.Get().GetArtworks(userInfo.primaryKey));
            yield return cd.coroutine;
            _artworkInfos = cd.result as MeumDB.ArtworkInfo[];
            
            if(container != null)
                AddArtworksToContainer();
        }
        
        private void AddArtworksToContainer()
        {
            var contentData = new UI.ContentData[_artworkInfos.Length];
            for (var i = 0; i < contentData.Length; ++i)
                contentData[i] = CreateContentData(_artworkInfos[i]);
            container.AddContents(contentData);
        }

        private string GetNameFromUrl(string url)
        {
            var uri = new Uri(url);
            return uri.Segments.Last();
        }

        private UI.ContentData CreateContentData(MeumDB.ArtworkInfo info)
        {
            var output = new UI.ContentData();
            output.id = info.primaryKey;
            output.name = GetNameFromUrl(info.url);
            output.like = info.like;
            output.hate = info.hate;
            output.url = info.url;
            output.thumbnail_url = info.url;

            return output;
        }

    }
}