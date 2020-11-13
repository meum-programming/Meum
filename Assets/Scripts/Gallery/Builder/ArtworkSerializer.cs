using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Gallery.MultiPlay;
using Global.Socket;
using UI.BuilderScene;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace Gallery.Builder
{
    [Serializable]
    struct RoomData
    {
        public ArtworkData[] artworks;
        public LandInfo[] lands;

        public RoomData(int n)
        {
            artworks = new ArtworkData[n];
            lands = null;
        }
    }

    public class ArtworkSerializer : MonoBehaviour
    {
        [SerializeField] public GameObject paintPrefab;
        [SerializeField] public GameObject object3DPrefab;

        private string _resetCheckpoint = "";

        private void Awake()
        {
            transform.position = Vector3.zero;
            transform.eulerAngles = Vector3.zero;
            transform.localScale = new Vector3(1, 1, 1);
        }

        private string GetJson()
        {
            var selfTransform = transform;
            var data = new RoomData(selfTransform.childCount);
            for (int i = 0; i < selfTransform.childCount; ++i)
                data.artworks[i] = selfTransform.GetChild(i).GetComponent<ArtworkInfo>().GetData();

            var proceduralSpace = GameObject.Find("proceduralSpace");
            Assert.IsNotNull(proceduralSpace);
            data.lands = proceduralSpace.GetComponent<ProceduralGalleryBuilder>().GetLandInfos();
            
            return JsonUtility.ToJson(data);
        }

        public void SetJson(string json)
        {
            Serialize(json);
        }

        private void ClearChild()
        {
            var selfTransform = transform;
            for (var i = 0; i < selfTransform.childCount; ++i)
                Destroy(selfTransform.GetChild(i).gameObject);
        }

        private void Serialize(string json)
        {
            var data = JsonUtility.FromJson<RoomData>(json);
            
            var proceduralSpace = GameObject.Find("proceduralSpace");
            Assert.IsNotNull(proceduralSpace);
            proceduralSpace.GetComponent<ProceduralGalleryBuilder>().Build(data.lands);

            ClearChild();
            
            for (var i = 0; i < data.artworks.Length; ++i)
            {
                ArtworkInfo artworkInfo = null;
                if(data.artworks[i].artwork_type == 0)
                    artworkInfo = Instantiate(paintPrefab, transform).GetComponent<ArtworkInfo>();
                else if (data.artworks[i].artwork_type == 1)
                    artworkInfo = Instantiate(object3DPrefab, transform).GetComponent<ArtworkInfo>();
                artworkInfo.SetUpWithData(data.artworks[i]);
            }

            _resetCheckpoint = json;
        }

        public void Save()
        {
            StartCoroutine(PatchRoomJson(GetJson()));
        }

        public void Clean()
        {
            GetComponent<ArtworkPlacer>().Deselect();
            
            for(var i=0; i<transform.childCount; ++i)
                Destroy(transform.GetChild(i).gameObject);
        }

        public void Reset()
        {
            GetComponent<ArtworkPlacer>().Deselect();
            
            SetJson(_resetCheckpoint);
        }

        private IEnumerator PatchRoomJson(string json)
        {
            yield return Global.MeumDB.Get().PatchRoomJson(json);
            MeumSocket.Get().BroadCastUpdateArtworks();

            _resetCheckpoint = json;
        }
    }
}
