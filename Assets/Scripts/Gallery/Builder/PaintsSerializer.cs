using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Gallery.Builder
{
    [Serializable]
    struct PaintsData
    {
        public PaintData[] paints;

        public PaintsData(int n)
        {
            paints = new PaintData[n];
        }
    }

    public class PaintsSerializer : MonoBehaviour
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
            var data = new PaintsData(selfTransform.childCount);
            for (int i = 0; i < selfTransform.childCount; ++i)
                data.paints[i] = selfTransform.GetChild(i).GetComponent<PaintInfo>().GetData();

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

        // private void OpenJsonFile(string filename)
        // {
        //     var path = Path.Combine(Application.dataPath, filename);
        //     Debug.Log("Opening " + path);
        //     if (!File.Exists(path))
        //         Debug.LogError(path + " is not exist");
        //     else
        //     {
        //         var json = File.ReadAllText(path);
        //         Serialize(json);
        //     }
        // }

        private void Serialize(string json)
        {
            var data = JsonUtility.FromJson<PaintsData>(json);

            ClearChild();

            for (var i = 0; i < data.paints.Length; ++i)
            {
                var paintInfo = Instantiate(paintPrefab, transform).GetComponent<PaintInfo>();
                paintInfo.SetUpWithData(data.paints[i]);
            }
        }

        public void OnSaveButton()
        {
            var json = GetJson();
            StartCoroutine(MeumDB.Get().PatchRoomJson(json));
            // if (inputfield == null) return;
            // var json = GetJson();
            // var path = Path.Combine(Application.dataPath, inputfield.text);
            //
            // File.WriteAllText(path, json);
        }
    }
}
