using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Gallery.MultiPlay;
using Global.Socket;
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

        private void Awake()
        {
            transform.position = Vector3.zero;
            transform.eulerAngles = Vector3.zero;
            transform.localScale = new Vector3(1, 1, 1);
        }

        public string GetJson()
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
        }

        public void OnSaveButton()
        {
            StartCoroutine(PatchRoomJson());
        }

        private IEnumerator PatchRoomJson()
        {
            var json = GetJson();
            yield return Global.MeumDB.Get().PatchRoomJson(json);
            MeumSocket.Get().BroadCastUpdateArtworks();
        }
    }
}
