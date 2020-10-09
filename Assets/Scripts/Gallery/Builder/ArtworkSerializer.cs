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
using UnityEngine.UI;

namespace Gallery.Builder
{
    [Serializable]
    struct ArtworksData
    {
        public ArtworkData[] paints;

        public ArtworksData(int n)
        {
            paints = new ArtworkData[n];
        }
    }

    public class ArtworkSerializer : MonoBehaviour
    {
        [SerializeField] public GameObject paintPrefab;

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
            var data = new ArtworksData(selfTransform.childCount);
            for (int i = 0; i < selfTransform.childCount; ++i)
                data.paints[i] = selfTransform.GetChild(i).GetComponent<ArtworkInfo>().GetData();

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
            var data = JsonUtility.FromJson<ArtworksData>(json);

            ClearChild();

            for (var i = 0; i < data.paints.Length; ++i)
            {
                var paintInfo = Instantiate(paintPrefab, transform).GetComponent<ArtworkInfo>();
                paintInfo.SetUpWithData(data.paints[i]);
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
