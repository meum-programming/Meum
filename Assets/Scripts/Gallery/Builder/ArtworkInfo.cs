using System;
using System.Collections;
using System.Reflection;
using Global;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Networking;

namespace Gallery.Builder
{
    [Serializable]
    public struct ArtworkData
    {
        public string url;
        public string bannerUrl;
        public Vector3 position;
        public Vector3 eulerAngle;
        public Vector3 scale;
        public int artwork_pk;
        public int artwork_type;

        public ArtworkData(string url,
            Vector3 position, Vector3 eulerAngle, Vector3 scale, 
            string bannerUrl, int artworkPk, int artworkType)
        {
            this.url = url;
            this.position = position;
            this.eulerAngle = eulerAngle;
            this.scale = scale;
            this.bannerUrl = bannerUrl;
            this.artwork_pk = artworkPk;
            this.artwork_type = artworkType;
        }
    }

    public class ArtworkInfo : MonoBehaviour
    {
        [SerializeField] public Renderer paintRenderer;
        [SerializeField] public Material mat;
        
        public string bannerUrl { get; private set; }
        public MeumDB.ArtworkInfo artworkInfo { get; private set; } = new MeumDB.ArtworkInfo();

        public ArtworkData GetData()
        {
            var selfTransform = transform;
            var url = artworkInfo.type_artwork == 0 ? artworkInfo.image_file : artworkInfo.object_file;
            return new ArtworkData(url,
                selfTransform.position, selfTransform.eulerAngles, selfTransform.localScale,
                bannerUrl, artworkInfo.primaryKey, artworkInfo.type_artwork);
        }

        public void SetUpWithData(ArtworkData data)
        {
            var selfTransform = transform;
            selfTransform.position = data.position;
            selfTransform.eulerAngles = data.eulerAngle;
            selfTransform.localScale = data.scale;

            artworkInfo.type_artwork = data.artwork_type;
            if (data.artwork_type == 0)
                artworkInfo.image_file = data.url;
            else
                artworkInfo.object_file = data.url;
            artworkInfo.primaryKey = data.artwork_pk;
            bannerUrl = data.bannerUrl;
            if (artworkInfo.type_artwork == 0)
                LoadImage();
            else
                LoadObject3D();
        }
        
        private void LoadImage()
        {
            var texture = Global.MeumDB.Get().GetTexture(artworkInfo.image_file);
            if (ReferenceEquals(texture, null)) return;
            paintRenderer.material = new Material(mat);
            paintRenderer.material.mainTexture = texture;
        }

        private void LoadObject3D()
        {
            var object3D = Global.MeumDB.Get().GetObject3D(artworkInfo.object_file);
            if (ReferenceEquals(object3D, null)) return;
            var obj = Instantiate(object3D, transform);
            obj.transform.localRotation = Quaternion.identity;
            obj.transform.localPosition = Vector3.zero;
            var collider = obj.GetComponent<Collider>();
            CopyComponent(collider, gameObject).isTrigger = true;
            collider.enabled = false;
        }
        
        public void SetupWithContent(UI.Content content)
        {
            if (content == null || content.image.texture == null) return;

            artworkInfo = content.data;

            if (content.data.type_artwork == 0)
            {
                StartCoroutine(SetTextureCoroutine());

                var scale = transform.localScale;
                scale.x = content.data.size_w;
                scale.z = content.data.size_h;
                transform.localScale = scale;
                transform.rotation = Quaternion.identity;
            }
            else if (content.data.type_artwork == 1)
            {
                StartCoroutine(SetModelCoroutine());
                
                var scale = transform.localScale;
                scale.x = scale.y = scale.z = content.data.size_w;
                transform.localScale = scale;
                transform.rotation = Quaternion.identity;
            }
        }

        private IEnumerator SetTextureCoroutine()
        {
            var textureGetter = Global.MeumDB.Get().GetTextureCoroutine(artworkInfo.image_file);
            yield return textureGetter.coroutine;
            
            Assert.IsNotNull(textureGetter.result);
            paintRenderer.material = new Material(mat);
            paintRenderer.material.mainTexture = textureGetter.result as Texture2D;
        }

        private IEnumerator SetModelCoroutine()
        {
            var object3DGetter = Global.MeumDB.Get().GetObject3DCoroutine(artworkInfo.object_file);
            yield return object3DGetter.coroutine;
            var obj = Instantiate(object3DGetter.result as GameObject, transform);
            obj.transform.localRotation = Quaternion.identity;
            obj.transform.localPosition = Vector3.zero;
            var collider = obj.GetComponent<Collider>();
            CopyComponent(collider, gameObject).isTrigger = true;
            collider.enabled = false;
        }
        
        // https://answers.unity.com/questions/458207/copy-a-component-at-runtime.html?_ga=2.240204931.1217398672.1604318035-771045389.1596531803
        private T CopyComponent<T>(T original, GameObject destination) where T : Component
        {
            var copy = destination.AddComponent(original.GetType());

            Type type = copy.GetType();
            BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Default | BindingFlags.DeclaredOnly;
            PropertyInfo[] pinfos = type.GetProperties(flags);
            foreach (var pinfo in pinfos) {
                if (pinfo.CanWrite) {
                    try {
                        pinfo.SetValue(copy, pinfo.GetValue(original, null), null);
                    }
                    catch { } 
                    // In case of NotImplementedException being thrown. For some reason specifying that exception didn't seem to catch it, so I didn't catch anything specific.
                }
            }
            
            FieldInfo[] finfos = type.GetFields(flags);
            foreach (var finfo in finfos) {
                finfo.SetValue(copy, finfo.GetValue(original));
            }

            return copy as T;
        }

        public void SetupBannerWithContent(UI.Content content, string bannerUrl)
        {
            SetupWithContent(content);
            this.bannerUrl = bannerUrl;
        }
    }
}