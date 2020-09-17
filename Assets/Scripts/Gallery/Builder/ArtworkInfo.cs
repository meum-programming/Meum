using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace Gallery.Builder
{
    [Serializable]
    public struct ArtworkData
    {
        public string url;
        public Vector3 position;
        public Vector3 eulerAngle;
        public Vector3 scale;

        public ArtworkData(string url, Vector3 position, Vector3 eulerAngle, Vector3 scale)
        {
            this.url = url;
            this.position = position;
            this.eulerAngle = eulerAngle;
            this.scale = scale;
        }
    }

    public class ArtworkInfo : MonoBehaviour
    {
        [SerializeField] public Renderer paintRenderer;
        [SerializeField] public Material mat;

        public string url;

        public ArtworkData GetData()
        {
            var selfTransform = transform;
            return new ArtworkData(url,
                selfTransform.position, selfTransform.eulerAngles, selfTransform.localScale);
        }

        public void SetUpWithData(ArtworkData data)
        {
            var selfTransform = transform;
            selfTransform.position = data.position;
            selfTransform.eulerAngles = data.eulerAngle;
            selfTransform.localScale = data.scale;

            url = data.url;
            StartCoroutine(LoadImage());
        }
        
        private IEnumerator LoadImage()
        {
            var textureGetter = MeumDB.Get().GetTexture(url);
            yield return textureGetter.coroutine;
            var texture = textureGetter.result as Texture2D;
            SetTexture(texture);
            
            // UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
            // yield return www.SendWebRequest();
            //
            // if (www.isNetworkError || www.isHttpError)
            // {
            //     Debug.Log(www.error);
            //     yield break;
            // }
            //
            // var texture = ((DownloadHandlerTexture) www.downloadHandler).texture;
            // SetTexture(texture);
        }
        
        public void SetTexture(Texture2D texture)
        {
            if (texture == null) return;
            paintRenderer.material = new Material(mat);
            paintRenderer.material.mainTexture = texture;

            var scale = transform.localScale;
            scale.x = scale.y * ((float) texture.width / texture.height);
            transform.localScale = scale;
        }
    }
}