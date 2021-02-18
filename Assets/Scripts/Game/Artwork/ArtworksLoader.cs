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
        [SerializeField] private UI.ContentViewer.ContentsContainer container2d;
        [SerializeField] private UI.ContentViewer.ContentsContainer container3d;

        private ArtWorkData[] _artworkInfos;
        private ArtWorkData[] _purchasedArtworkInfos;

        public void Load()
        {
            StartCoroutine(LoadArtworks());
        }

        private IEnumerator LoadArtworks()
        {
            bool nextOn = false;

            List<ArtWorkData> artWorkDataList = new List<ArtWorkData>();

            ArtWorkRequest artWorkRequest = new ArtWorkRequest()
            {
                requestStatus = 1,
                uid = MeumDB.Get().GetToken(),
                successOn = ResultData =>
                {
                    artWorkDataList = ((ArtWorkRespons)ResultData).result;
                    nextOn = true;
                }
            };
            artWorkRequest.RequestOn();

            yield return new WaitUntil(() => nextOn);


            var output = new ArtWorkData[artWorkDataList.Count];
            for (var i = 0; i < artWorkDataList.Count; ++i)
            {
                output[i] = artWorkDataList[i];
            }

            _artworkInfos = output;

            nextOn = false;

            List<ShopByArtWorkData> shopByArtWorkDataList = new List<ShopByArtWorkData>();

            ArtWorkRequest artWorkRequest2 = new ArtWorkRequest()
            {
                requestStatus = 2,
                uid = MeumDB.Get().GetToken(),
                successOn = ResultData =>
                {
                    shopByArtWorkDataList = ((ShopByArtWorkRespons)ResultData).result;
                    nextOn = true;
                }
            };
            artWorkRequest2.RequestOn();

            yield return new WaitUntil(() => nextOn);

            var output2 = new ArtWorkData[shopByArtWorkDataList.Count];
            for (var i = 0; i < shopByArtWorkDataList.Count; ++i)
            {
                output2[i] = shopByArtWorkDataList[i].artwork;
            }

            _purchasedArtworkInfos = output2;
            
            if(container2d != null && container3d != null)
                AddArtworksToContainer();
        }
        
        private void AddArtworksToContainer()
        {
            var artworkInfos2d = new List<ArtWorkData>();
            var artworkInfos3d = new List<ArtWorkData>();
            
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