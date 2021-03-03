using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Networking;

namespace Core
{
    /*
     * @brief DbBuffer의 부모 클래스
     * @details Db에서 받아온 정보를 런타임에서 캐싱하는 클래스
     */
    public abstract class DbBufferBase<T>
    {
        private Dictionary<string, T> _buffer = new Dictionary<string, T>();

        /*
         * @brief Get 함수, 만약 가지고 있지 않다면 로딩한 후 Dictionary에 저장함
         * @details caller: 로딩 코루틴을 실행할 MonoBehaviour, url: 로드할 정보를 담고있는 url
         */
        public CoroutineWithData Get(MonoBehaviour caller, string url)
        {
            var cd = new CoroutineWithData(caller, Load(url));
            return cd;
        }
        
        /*
         * @brief Get 함수, 가지고 있지 않다면 default(T) 를 반환
         * @details url: 로드할 정보를 담고있는 url <br>
         * 요청한 url에 해당하는 정보를 가지고 있지 않는경우 로드하지 않음
         */
        public T Get(string url)
        {
            if (_buffer.ContainsKey(url))
                return _buffer[url];
            return default(T);
        }
        
        public void Clear()
        {
            _buffer.Clear();
        }
        
        /*
         * @brief 버퍼가 url에 해당하는 정보를 가지고있다면 true
         */
        protected bool Contains(string url)
        {
            return _buffer.ContainsKey(url);
        }
        
        /*
         * @brief Dictionary에 아이템 추가 (자식 class에서 Load 함수 구현용)
         */
        protected void Add(string url, T value)
        {
            if (_buffer.ContainsKey(url) != false)
            {
                _buffer.Add(url, value);
            }
        }

        
        /*
         * @brief 자식클래스에서 구현해야할 함수 <br>
         * 지정한 url에서 필요한 정보를 로드한 후 저장
         */
        protected abstract IEnumerator Load(string url);
    }
    
    /*
     * @brief DbBuffer의 텍스쳐 구현체
     */
    public class TextureBuffer : DbBufferBase<Texture2D>
    {
        protected override IEnumerator Load(string url)
        {
            if (Contains(url))
                yield return Get(url);
            else
            {
                UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
                yield return www.SendWebRequest();

                if (www.isNetworkError || www.isHttpError)
                {
                    Debug.Log(www.error);
                    yield break;
                }

                var texture = ((DownloadHandlerTexture) www.downloadHandler).texture;
                Add(url, texture);
                yield return texture;
            }
        }
    }
    
    /*
     * @brief DbBuffer의 3D object 구현체
     */
    public class Object3DBuffer : DbBufferBase<GameObject>
    {
        protected override IEnumerator Load(string url)
        {
            if (Contains(url))
                yield return Get(url);
            else
            {
                UnityWebRequest www = UnityWebRequestAssetBundle.GetAssetBundle(url);
                yield return www.SendWebRequest();

                if (www.isNetworkError || www.isHttpError)
                {
                    Debug.Log(www.error);
                    yield break;
                }

                var bundle = ((DownloadHandlerAssetBundle) www.downloadHandler).assetBundle;

                if (bundle != null)
                {
                    var allAssets = bundle.GetAllAssetNames();
                    var request = bundle.LoadAssetAsync(allAssets[0], typeof(GameObject));
                    yield return request;

                    var result = request.asset as GameObject;
                    Add(url, result);
                    yield return result;
                }

            }
        }
    }
}