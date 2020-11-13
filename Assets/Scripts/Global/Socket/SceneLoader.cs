using System.Collections;
using System.Collections.Generic;
using Gallery.Builder;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Assertions;
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
            var artworksData = JsonUtility.FromJson<RoomData>(roomInfo.data_json);
            var artworksDataLength = artworksData.artworks.Length;

            var set2d = new HashSet<string>();
            var set3d = new HashSet<string>();
            for (var i = 0; i < artworksDataLength; ++i)
            {
                var artworkData = artworksData.artworks[i];
                if (artworkData.artwork_type == 0)
                    set2d.Add(artworkData.url);
                else if (artworkData.artwork_type == 1)
                    set3d.Add(artworkData.url);
            }

            var loadCompleteCnt = 1;
            var totalCnt = set2d.Count + set3d.Count;
            foreach (var url in set2d)
            {
                cd = MeumDB.Get().GetTextureCoroutine(url);
                yield return cd.coroutine;
                progressBar.SetProgress((float)loadCompleteCnt/totalCnt);
                ++loadCompleteCnt;
            }

            foreach (var url in set3d)
            {
                cd = MeumDB.Get().GetObject3DCoroutine(url);
                yield return cd.coroutine;
                progressBar.SetProgress((float)loadCompleteCnt/totalCnt);
                ++loadCompleteCnt;
            }
            
            // load userInfo data to check if player own gallery
            cd = new CoroutineWithData(_coroutineCaller, MeumDB.Get().GetUserInfo());
            yield return cd.coroutine;
            var userInfo = cd.result as MeumDB.UserInfo;
            cd = new CoroutineWithData(_coroutineCaller, MeumDB.Get().GetRoomInfoWithUser(userInfo.primaryKey));
            yield return cd.coroutine;
            var ownRoomInfo = cd.result as MeumDB.RoomInfo;
            var ownRoomId = ownRoomInfo.primaryKey;
            
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
            
            // setting userList
            var userList = UI.UserList.UserList.Get();
            var socket = MeumSocket.Get();
            userList.SetOwnerName(roomInfo.owner.nickname);
            userList.AddUser(socket.GetPlayerID(), socket.PlayerInfo.nickname);
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
            var cd = new CoroutineWithData(_coroutineCaller, MeumDB.Get().GetArtworks());
            yield return cd.coroutine;
            var artworkInfos = cd.result as MeumDB.ArtworkInfo[];
            Assert.IsNotNull(artworkInfos);
            
            cd = new CoroutineWithData(_coroutineCaller, MeumDB.Get().GetPurchasedArtworks());
            yield return cd.coroutine;
            var purchasedArtworkInfos = cd.result as MeumDB.ArtworkInfo[];
            Assert.IsNotNull(purchasedArtworkInfos);

            var artworksCount = artworkInfos.Length + purchasedArtworkInfos.Length;
            for (var i = 0; i < artworkInfos.Length; ++i)
            {
                var artworkInfo = artworkInfos[i];
                if (!ReferenceEquals(null, artworkInfo.thumbnail))
                {
                    var textureGetter = Global.MeumDB.Get().GetTextureCoroutine(artworkInfo.thumbnail);
                    yield return textureGetter.coroutine;
                }

                progressBar.SetProgress((float) (i + 1) / artworksCount);
                yield return null;
            }

            for (var i = 0; i < purchasedArtworkInfos.Length; ++i)
            {
                var artworkInfo = purchasedArtworkInfos[i];
                if (!ReferenceEquals(null, artworkInfo.thumbnail))
                {
                    var textureGetter = Global.MeumDB.Get().GetTextureCoroutine(artworkInfo.thumbnail);
                    yield return textureGetter.coroutine;
                }

                progressBar.SetProgress((float) (artworkInfos.Length + i + 1) / artworksCount);
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
