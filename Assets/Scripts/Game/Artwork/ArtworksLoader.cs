using System.Collections;
using System.Collections.Generic;
using Core;
using UI;
using UnityEngine;
using UnityEngine.Assertions;

namespace Game.Artwork
{
    public class ArtworksLoader : MonoBehaviour
    {
        [SerializeField] private ContentsContainer container2d;
        [SerializeField] private ContentsContainer container3d;

        private Core.MeumDB.ArtworkInfo[] _artworkInfos;
        private Core.MeumDB.ArtworkInfo[] _purchasedArtworkInfos;

        public void Load()
        {
            StartCoroutine(LoadArtworks());
        }

        private IEnumerator LoadArtworks()
        {
            var cd = new CoroutineWithData(this, MeumDB.Get().GetArtworks());
            yield return cd.coroutine;
            _artworkInfos = cd.result as MeumDB.ArtworkInfo[];
            Assert.IsNotNull(_artworkInfos);
            
            cd = new CoroutineWithData(this, MeumDB.Get().GetPurchasedArtworks());
            yield return cd.coroutine;
            _purchasedArtworkInfos = cd.result as MeumDB.ArtworkInfo[];
            Assert.IsNotNull(_purchasedArtworkInfos);
            
            if(container2d != null && container3d != null)
                AddArtworksToContainer();
        }
        
        private void AddArtworksToContainer()
        {
            var artworkInfos2d = new List<MeumDB.ArtworkInfo>();
            var artworkInfos3d = new List<MeumDB.ArtworkInfo>();
            
            for (var i = 0; i < _artworkInfos.Length; ++i)
            {
                if(_artworkInfos[i].type_artwork == 0)
                    artworkInfos2d.Add(_artworkInfos[i]);
                else
                    artworkInfos3d.Add(_artworkInfos[i]);
            }

            for (var i = 0; i < _purchasedArtworkInfos.Length; ++i)
            {
                if(_purchasedArtworkInfos[i].type_artwork == 0)
                    artworkInfos2d.Add(_purchasedArtworkInfos[i]);
                else
                    artworkInfos3d.Add(_purchasedArtworkInfos[i]);
            }

            container2d.AddContents(artworkInfos2d);
            container3d.AddContents(artworkInfos3d);
        }
    }
}