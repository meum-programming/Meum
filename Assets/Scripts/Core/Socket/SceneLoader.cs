using System.Collections;
using System.Collections.Generic;
using Game.Artwork;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;
using Core;

namespace Core.Socket {
    /*
     * @brief 각각의 Scene들을 로딩하는 클래스
     * @details Scene 뿐만 아니라 필요한 DB자원들을 로딩 (WebGL 빌드에서 멀티스레딩이 안되므로 필수적),
     * SceneState를 업데이트
     */
    public class SceneLoader
    {
        private SceneState _state;
        private MonoBehaviour _coroutineCaller;
        private SocketEventHandler.EnteringSuccessEventData _galleryData;
        private SocketEventHandler.SquareEnteringSuccessEventData _squareData;
        private int _playerPk;
        
        public SceneLoader(MonoBehaviour caller, SceneState state)
        {
            _state = state;
            _coroutineCaller = caller;
        }

        public int GetRoomId()
        {
            return _galleryData.roomId;
        }

        public int GetPlayerPk()
        {
            return _playerPk;
        }

        #region Artworks Serialization
        
        /*
         * @brief Gallery에 포함된 Artwork 정보를 받아서 PaintSerializer 에 넘겨줌
         * Scene에 PaintSerializer(name: 'Artworks') 가 반드시 있어야함
         */
        public void SerializeArtworks()
        {
            Assert.IsTrue(_state.IsInGallery() || _state.IsSubOfGalleryOwn());
            _coroutineCaller.StartCoroutine(SerializeArtworksCoroutine(_galleryData.roomId));
        }
        
        private IEnumerator SerializeArtworksCoroutine(int roomId)
        {
            var paintsSerializer = GameObject.Find("Artworks").GetComponent<DataJsonSerializer>();
            Assert.IsNotNull(paintsSerializer);
            
            var cd = new CoroutineWithData(_coroutineCaller, MeumDB.Get().GetRoomInfo(roomId));
            yield return cd.coroutine;
            Assert.IsNotNull(cd.result);
            var roomInfo = cd.result as MeumDB.RoomInfo;
            Assert.IsNotNull(roomInfo);
            
            paintsSerializer.SetJson(roomInfo.data_json);
        }
        
        #endregion
        
        #region LoadGallery
        
        public void LoadGallery(SocketEventHandler.EnteringSuccessEventData data)
        {
            Assert.IsTrue(_state.IsNotInGalleryOrSquare() || _state.IsSubOfGalleryOwn());
            _coroutineCaller.StartCoroutine(LoadGalleryCoroutine(data));
        }
        
        private IEnumerator LoadGalleryCoroutine(SocketEventHandler.EnteringSuccessEventData data)
        {
            var sceneOpen = SceneManager.LoadSceneAsync("Loading");
            while (!sceneOpen.isDone) yield return null;
            var progressBar = GameObject.Find("ProgressBar").GetComponent<UI.ProgressBar>();
            Assert.IsNotNull(progressBar);
            
            // load demanded artworks
            var cd = new CoroutineWithData(_coroutineCaller, MeumDB.Get().GetRoomInfo(data.roomId));
            yield return cd.coroutine;
            Assert.IsNotNull(cd.result);
            var roomInfo = cd.result as MeumDB.RoomInfo;
            Assert.IsNotNull(roomInfo);
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
                Assert.IsNotNull(cd.result);
                progressBar.SetProgress((float)loadCompleteCnt/totalCnt);
                ++loadCompleteCnt;
            }

            foreach (var url in set3d)
            {
                cd = MeumDB.Get().GetObject3DCoroutine(url);
                yield return cd.coroutine;
                Assert.IsNotNull(cd.result);
                progressBar.SetProgress((float)loadCompleteCnt/totalCnt);
                ++loadCompleteCnt;
            }
            
            // load userInfo data to check if player own gallery
            cd = new CoroutineWithData(_coroutineCaller, MeumDB.Get().GetUserInfo());
            yield return cd.coroutine;
            Assert.IsNotNull(cd.result);
            var userInfo = cd.result as MeumDB.UserInfo;
            Assert.IsNotNull(userInfo);
            _playerPk = userInfo.primaryKey;

            cd = new CoroutineWithData(_coroutineCaller, MeumDB.Get().GetRoomInfoWithUser(userInfo.primaryKey));
            yield return cd.coroutine;
            Assert.IsNotNull(cd.result);
            var ownRoomInfo = cd.result as MeumDB.RoomInfo;

            MeumDB.Get().myRoomInfo = ownRoomInfo;

            Assert.IsNotNull(ownRoomInfo);
            var ownRoomId = ownRoomInfo.primaryKey;
            
            // load scene
            progressBar.SetProgress(0);
            sceneOpen = SceneManager.LoadSceneAsync("ProceduralGallery");
            while (!sceneOpen.isDone)
            {
                progressBar.SetProgress(sceneOpen.progress);
                yield return null;
            }

            if (_state.IsSubOfGallery())
                DataSynchronizer.Get().ShowPlayers();
            else
                DataSynchronizer.Get().Setup(data.maxN);
            
            
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
            userList.AddUser(socket.GetPlayerId(), socket.LocalPlayerInfo.nickname);
        }
        #endregion
        
        #region Load Square
        
        public void LoadSquare(SocketEventHandler.SquareEnteringSuccessEventData data)
        {
            Assert.IsTrue(_state.IsNotInGalleryOrSquare());
            _coroutineCaller.StartCoroutine(LoadSquareCoroutine(data));
        }
        
        private IEnumerator LoadSquareCoroutine(SocketEventHandler.SquareEnteringSuccessEventData data)
        {
            var sceneOpen = SceneManager.LoadSceneAsync("Loading");
            while (!sceneOpen.isDone) yield return null;
            var progressBar = GameObject.Find("ProgressBar").GetComponent<UI.ProgressBar>();
            
            // load scene
            sceneOpen = SceneManager.LoadSceneAsync("Square");
            while (!sceneOpen.isDone)
            {
                progressBar.SetProgress(sceneOpen.progress);
                yield return null;
            }
            
            if(_state.IsSubOfSquare())
                DataSynchronizer.Get().ShowPlayers();
            else
                DataSynchronizer.Get().Setup(data.maxN);
            
            _squareData = data;
            
            _state.EnterSquare();
        }
        
        #endregion

        #region LoadEditScene
        
        public void LoadEditScene()
        {
            //Assert.IsTrue(_state.IsSubOfGalleryOwn());
            _coroutineCaller.StartCoroutine(LoadEditSceneCoroutine());
        }
        
        private IEnumerator LoadEditSceneCoroutine()
        {
            DataSynchronizer.Get().HidePlayers();
            
            var sceneOpen = SceneManager.LoadSceneAsync("Loading");
            while (!sceneOpen.isDone) yield return null;
            var progressBar = GameObject.Find("ProgressBar").GetComponent<UI.ProgressBar>();
            
            // load demanded textures
            var cd = new CoroutineWithData(_coroutineCaller, MeumDB.Get().GetArtworks());
            yield return cd.coroutine;
            Assert.IsNotNull(cd.result);
            var artworkInfos = cd.result as MeumDB.ArtworkInfo[];
            Assert.IsNotNull(artworkInfos);
            
            cd = new CoroutineWithData(_coroutineCaller, MeumDB.Get().GetPurchasedArtworks());
            yield return cd.coroutine;
            Assert.IsNotNull(cd.result);
            var purchasedArtworkInfos = cd.result as MeumDB.ArtworkInfo[];
            Assert.IsNotNull(purchasedArtworkInfos);
        
            var artworksCount = artworkInfos.Length + purchasedArtworkInfos.Length;
            for (var i = 0; i < artworkInfos.Length; ++i)
            {
                var artworkInfo = artworkInfos[i];
                if (!ReferenceEquals(artworkInfo.thumbnail, null))
                {
                    var textureGetter = MeumDB.Get().GetTextureCoroutine(artworkInfo.thumbnail);
                    yield return textureGetter.coroutine;
                    Assert.IsNotNull(textureGetter.result);
                }

                progressBar.SetProgress((float) (i + 1) / artworksCount);
                yield return null;
            }

            for (var i = 0; i < purchasedArtworkInfos.Length; ++i)
            {
                var artworkInfo = purchasedArtworkInfos[i];
                if (!ReferenceEquals(null, artworkInfo.thumbnail))
                {
                    var textureGetter = MeumDB.Get().GetTextureCoroutine(artworkInfo.thumbnail);
                    yield return textureGetter.coroutine;
                    Assert.IsNotNull(textureGetter.result);
                }

                progressBar.SetProgress((float) (artworkInfos.Length + i + 1) / artworksCount);
                yield return null;
            }
            
            // load scene
            progressBar.SetProgress(0);
            sceneOpen = SceneManager.LoadSceneAsync("ProceduralGalleryBuilder");
            while (!sceneOpen.isDone)
            {
                progressBar.SetProgress(sceneOpen.progress);
                yield return null;
            }
            
            var artworksLoader = GameObject.Find("Artworks").GetComponent<ArtworksLoader>();
            Assert.IsNotNull(artworksLoader);
            artworksLoader.Load();
            
            _state.EnterEdit();

            // setting paints
            SerializeArtworks();
        }
        
        #endregion

        public void Return()
        {
            Assert.IsTrue(_state.IsSubOfGallery() || _state.IsSubOfGallery());
            if (_state.IsSubOfGallery())
                LoadGallery(_galleryData);
        }
    }
}
