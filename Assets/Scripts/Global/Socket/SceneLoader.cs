using System.Collections;
using Gallery.Builder;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Global.Socket {
    public class SceneLoader
    {
        private SceneState _state;
        private MonoBehaviour _coroutineCaller;
        private SocketEventHandler.EnteringSuccessEventData _galleryData;
        private SocketEventHandler.SquareEnteringSuccessEventData _squareData;

        public SceneLoader(MonoBehaviour caller, SceneState state)
        {
            _state = state;
            _coroutineCaller = caller;
        }

        #region SerializeArtworks
        public void SerializeArtworks()
        {
            if (!_state.IsInGallery())
            {
                Debug.LogError("Global.Socket.SceneLoader - SerializeArtworks : invalid state for calling");
                Application.Quit(-1);
            }
            _coroutineCaller.StartCoroutine(SerializeArtworksCoroutine(_galleryData.roomId));
        }
        
        private IEnumerator SerializeArtworksCoroutine(int roomId)
        {
            var paintsSerializer = GameObject.Find("Artworks").GetComponent<ArtworkSerializer>();
            var cd = new CoroutineWithData(_coroutineCaller, MeumDB.Get().GetRoomInfo(roomId));
            yield return cd.coroutine;
            var roomInfo = cd.result as MeumDB.RoomInfo;
            Debug.Assert(roomInfo != null);
            paintsSerializer.SetJson(roomInfo.data_json);
        }
        #endregion
        
        #region LoadGallery
        private IEnumerator LoadGalleryCoroutine(SocketEventHandler.EnteringSuccessEventData data)
        {
            // wait load complete
            // var loading = SceneManager.LoadSceneAsync(MeumSocket.Get().GetGallerySceneName(data.roomType));
            // while (!loading.isDone) yield return null;

            var sceneOpen = SceneManager.LoadSceneAsync("Loading");
            while (!sceneOpen.isDone) yield return null;
            var progressBar = GameObject.Find("ProgressBar").GetComponent<ProgressBar>();
            
            // load demanded artworks
            var cd = new CoroutineWithData(_coroutineCaller, MeumDB.Get().GetRoomInfo(data.roomId));
            yield return cd.coroutine;
            var roomInfo = cd.result as MeumDB.RoomInfo;
            var artworksData = JsonUtility.FromJson<ArtworksData>(roomInfo.data_json);
            var artworksDataLength = artworksData.paints.Length;
            for (var i = 0; i < artworksDataLength; ++i)
            {
                var artworkData = artworksData.paints[i];
                var textureGetter = Global.MeumDB.Get().GetTextureCoroutine(artworkData.url);
                yield return textureGetter.coroutine;
                progressBar.SetProgress((float)(i+1)/artworksDataLength);
                yield return null;
            }
            
            // load userInfo data to check if player own gallery
            cd = new CoroutineWithData(_coroutineCaller, MeumDB.Get().GetUserInfo());
            yield return cd.coroutine;
            var userInfo = cd.result as MeumDB.UserInfo;
            var ownRoomId = roomInfo.primaryKey;
            
            // load scene
            progressBar.SetProgress(0);
            // sceneOpen = SceneManager.LoadSceneAsync(MeumSocket.Get().GetGallerySceneName(data.roomType));
            sceneOpen = SceneManager.LoadSceneAsync("ProceduralGallery");
            while (!sceneOpen.isDone)
            {
                progressBar.SetProgress(sceneOpen.progress);
                yield return null;
            }

            if (_state.IsSubOfGallery())
                DataSyncer.Get().Activate();
            else
                DataSyncer.Get().Setup(data.maxN);
            
            
            if(ownRoomId == data.roomId)
                _state.EnterGalleryOwn();
            else
                _state.EnterGallery();
            
            // save galleryData
            _galleryData = data;
            
            // setting paints
            SerializeArtworks();
        }
        public void LoadGallery(SocketEventHandler.EnteringSuccessEventData data)
        {
            _coroutineCaller.StartCoroutine(LoadGalleryCoroutine(data));
        }
        #endregion
        
        #region Load Square
        private IEnumerator LoadSquareCoroutine(SocketEventHandler.SquareEnteringSuccessEventData data)
        {
            var sceneOpen = SceneManager.LoadSceneAsync("Loading");
            while (!sceneOpen.isDone) yield return null;
            var progressBar = GameObject.Find("ProgressBar").GetComponent<ProgressBar>();
            
            // load scene
            sceneOpen = SceneManager.LoadSceneAsync("Square");
            while (!sceneOpen.isDone)
            {
                progressBar.SetProgress(sceneOpen.progress);
                yield return null;
            }
            
            if(_state.IsSubOfSquare())
                DataSyncer.Get().Activate();
            else
                DataSyncer.Get().Setup(data.maxN);
            
            _squareData = data;
            
            _state.EnterSquare();
        }
        public void LoadSquare(SocketEventHandler.SquareEnteringSuccessEventData data)
        {
            _coroutineCaller.StartCoroutine(LoadSquareCoroutine(data));
        }
        #endregion
        
        #region LeaveToShop
        private IEnumerator LeaveToShopCoroutine()
        {
            DataSyncer.Get().Deactivate();
            
            var sceneOpen = SceneManager.LoadSceneAsync("Loading");
            while (!sceneOpen.isDone) yield return null;
            var progressBar = GameObject.Find("ProgressBar").GetComponent<ProgressBar>();
            
            // load scene
            sceneOpen = SceneManager.LoadSceneAsync("Shop");
            while (!sceneOpen.isDone)
            {
                progressBar.SetProgress(sceneOpen.progress);
                yield return null;
            }
            
            _state.EnterShop();
        }
        
        public void LeaveToShop()
        {
            if (_state.IsNotInGalleryOrSquare())
            {
                Debug.Log("Global.Socket.SceneLoader - LeaveToShop : Invalid transition");
                Application.Quit(-1);
            }
            _coroutineCaller.StartCoroutine(LeaveToShopCoroutine());
        }
        #endregion
        
        #region LeaveToEdit
        private IEnumerator LeaveToEditCoroutine()
        {
            DataSyncer.Get().Deactivate();
            
            var sceneOpen = SceneManager.LoadSceneAsync("Loading");
            while (!sceneOpen.isDone) yield return null;
            var progressBar = GameObject.Find("ProgressBar").GetComponent<ProgressBar>();
            
            // load demanded textures
            var cd = new CoroutineWithData(_coroutineCaller, MeumDB.Get().GetUserInfo());
            yield return cd.coroutine;
            var userInfo = cd.result as MeumDB.UserInfo;
            Debug.Assert(userInfo != null);
            cd = new CoroutineWithData(_coroutineCaller, MeumDB.Get().GetArtworks(userInfo.primaryKey));
            yield return cd.coroutine;
            var artworkInfos = cd.result as Global.MeumDB.ArtworkInfo[];
            var artworksCount = artworkInfos.Length;
            for (var i = 0; i < artworksCount; ++i)
            {
                var artworkInfo = artworkInfos[i];
                var textureGetter = Global.MeumDB.Get().GetTextureCoroutine(artworkInfo.url);  // TODO: Change to thumbnail url
                yield return textureGetter.coroutine;
                progressBar.SetProgress((float) (i + 1) / artworksCount);
                yield return null;
            }
            
            // load scene
            progressBar.SetProgress(0);
            // sceneOpen = SceneManager.LoadSceneAsync(MeumSocket.Get().GetBuilderSceneName(_galleryData.roomType));
            sceneOpen = SceneManager.LoadSceneAsync("ProceduralGalleryBuilder");
            while (!sceneOpen.isDone)
            {
                progressBar.SetProgress(sceneOpen.progress);
                yield return null;
            }
            
            var artworksLoader = GameObject.Find("Artworks").GetComponent<ArtworksLoader>();
            artworksLoader.Load();
            
            _state.EnterEdit();
        }
        
        public void LeaveToEdit()
        {
            if (!_state.IsSubOfGalleryOwn())
            {
                Debug.Log("Global.Socket.SceneLoader - LeaveToEdit : Invalid transition");
                Application.Quit(-1);
            }
            _coroutineCaller.StartCoroutine(LeaveToEditCoroutine());
        }
        #endregion

        public void Return()
        {
            if (_state.IsSubOfGallery())
            {
                LoadGallery(_galleryData);
                _state.ReturnToGallery();
            }
            else if (_state.IsSubOfSquare())
            {
                LoadSquare(_squareData);
                _state.ReturnToSquare();
            }
            else
            {
                Debug.LogError("Global.Socket.SceneLoader - Return : invalid transition");
                Application.Quit(-1);
            }
        }
    }
}
