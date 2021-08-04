using System;
using System.Collections;
using System.Reflection;
using UnityEngine;
using UnityEngine.Assertions;
using Core;
using UnityEngine.Events;

namespace Game.Artwork
{
    /*
     * @brief DB의 roomData 속의 data_json["artworks"] 에 저장되는 Artwork 정보 구조체
     */
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
    
    /*
     * @brief Artwork GameObject 에서 ArtworkData 를 관리하는 컴포넌트
     * @details ArtworkData, ArtworkInfo(DB)(UI.Content.data) 를 통해 업데이트 가능
     * GetData를 통해 ArtworkData 구조체로 받을 수 있음
     */
    public class ArtworkInfo : MonoBehaviour
    {
        [SerializeField] public Renderer paintRenderer;
        [SerializeField] public Material mat;
        [SerializeField] public YoutubePlayer player;
        [HideInInspector] public string bannerUrl;

        public int ArtworkType => _artworkData.artwork_type;

        private ArtworkData _artworkData;

        public ArtworkData GetArtworkData()
        {
            var selfTransform = transform;
            _artworkData.position = selfTransform.localPosition;
            _artworkData.eulerAngle = selfTransform.localEulerAngles;
            _artworkData.scale = selfTransform.localScale;
            _artworkData.bannerUrl = bannerUrl;
            return _artworkData;
        }
        
        /*
         * @brief 해당하는 Artwork의 ArtworkInfo(DB) 를 불러옴
         * @details 컴포넌트에서 ArtworkData 만 가지고 있으므로 ArtworkInfo는 DB에서 불러와야함
         */

        public IEnumerator GetArtworkInfo2()
        {
            bool nextOn = false;

            ArtWorkData data = null;

            ArtWorkRequest artWorkRequest = new ArtWorkRequest()
            {
                requestStatus = 0,
                id = _artworkData.artwork_pk,
                successOn = ResultData =>
                {
                    data = ((ArtWorkResponsOnlyOne)ResultData).result;
                    nextOn = true;
                }
            };
            artWorkRequest.RequestOn();

            yield return new WaitUntil(() => nextOn);

            yield return data;
        }

        /*
         * @brief ArtworkData로 정보를 업데이트, 방을 불러오며 data_json을 통해 Artwork들을 설치할때 사용됨
         */
        public void UpdateWithArtworkData(ArtworkData data)
        {
            var selfTransform = transform;
            selfTransform.position = data.position;
            selfTransform.eulerAngles = data.eulerAngle;
            selfTransform.localScale = data.scale;

            _artworkData = data;
            bannerUrl = data.bannerUrl;
            
            if (data.artwork_type == 0)
                StartCoroutine(LoadTextureCoroutine());
            else if(data.artwork_type == 1)
                LoadModelCoroutine();
        }
        
        /*
         * @brief UI.Content.data(ArtworkInfo(DB))를 통해 업데이트, BuilderScene에서 새로 설치할때 사용됨
         */
        public void UpdateWithArtworkContent(UI.ContentViewer.Content content , UnityAction loadOn = null)
        {
            if (ReferenceEquals(content, null) || 
                ReferenceEquals(content.Image.texture, null)) 
                return;

            _artworkData.artwork_pk = content.Data.id;
            _artworkData.artwork_type = content.Data.type_artwork;

            var scale = transform.localScale;
            if (_artworkData.artwork_type == 0)
            {
                _artworkData.url = content.Data.image_file;
                StartCoroutine(LoadTextureCoroutine(loadOn));
                scale.x = content.Data.size_w ;
                scale.z = content.Data.size_h ;
            }
            else if (_artworkData.artwork_type == 1)
            {
                _artworkData.url = content.Data.object_file;
                LoadModelCoroutine(loadOn);
                scale.x = scale.y = scale.z = content.Data.size_w;
            }
            transform.localScale = scale;

            transform.rotation = Quaternion.identity;

        }

        /*
         * @brief DB에서 텍스쳐를 불러오는 함수 Artwork의 타입이 2d(=0) 일 경우 사용됨
         */
        private IEnumerator LoadTextureCoroutine(UnityAction loadOn = null)
        {
            string url = _artworkData.url;

            string baseURL = "https://api.meum.me/datas/";
            int index = url.IndexOf(baseURL);

            if (index == -1)
            {
                url = baseURL + url;
            }

            var textureGetter = MeumDB.Get().GetTextureCoroutine(url);
            yield return textureGetter.coroutine;

            var texture = textureGetter.result as Texture2D;
            
            paintRenderer.material = new Material(mat);
            paintRenderer.material.mainTexture = texture;

            if (loadOn != null)
            {
                loadOn();
            }
        }

        /*
         * @brief DB에서 3D Object를 불러오는 함수 Artwork의 타입이 3d(=1) 일 경우 사용됨
         */
        private void LoadModelCoroutine(UnityAction loadOn = null)
        {
            string path = _artworkData.url.Replace("artwork_1master.meum/", "");
            path = path.Replace("https://api.meum.me/datas/", "");

            AddressableManager.Insatnce.GetObj(path, (GameObject resultPbj) =>
            {
                var obj = Instantiate(resultPbj, transform);

                if (obj != null)
                {
                    obj.transform.localRotation = Quaternion.identity;
                    obj.transform.localPosition = Vector3.zero;
                    obj.gameObject.SetActive(true);

                    if (loadOn != null)
                    {
                        loadOn();
                    }
                }
            });
        }

        public void LoadVideoCoroutine(string url)
        {
            player.Play(url);
            player.videoPlayer.GetTargetAudioSource(0).mute = true;
            player.videoPlayer.SetDirectAudioMute(0, true);
        }

        /*
         * @brief 컴포넌트 복사함수, Collider 옮기는데 사용됨
         * @details https://answers.unity.com/questions/458207/copy-a-component-at-runtime.html?_ga=2.240204931.1217398672.1604318035-771045389.1596531803
         * 주소 참조
         */
        private T CopyComponent<T>(T original, GameObject destination) where T : Component
        {
            var copy = destination.AddComponent(original.GetType());
            Assert.IsNotNull(copy);
            
            var type = copy.GetType();
            Assert.IsNotNull(type);
            var flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Default | BindingFlags.DeclaredOnly;
            var pinfos = type.GetProperties(flags);
            Assert.IsNotNull(pinfos);
            
            foreach (var pinfo in pinfos) {
                if (pinfo.CanWrite) {
                    try
                    {
                        pinfo.SetValue(copy, pinfo.GetValue(original, null), null);
                    }
                    catch
                    { } 
                    // In case of NotImplementedException being thrown. For some reason specifying that exception didn't seem to catch it, so I didn't catch anything specific.
                }
            }
            
            var finfos = type.GetFields(flags);
            Assert.IsNotNull(finfos);
            
            foreach (var finfo in finfos) {
                finfo.SetValue(copy, finfo.GetValue(original));
            }

            return copy as T;
        }

    }
}