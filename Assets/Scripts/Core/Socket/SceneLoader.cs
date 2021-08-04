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
            _coroutineCaller.StartCoroutine(SerializeArtworksCoroutine2(_galleryData.roomId));
        }
        
        /*
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
        */

        private IEnumerator SerializeArtworksCoroutine2(int roomId)
        {
            var paintsSerializer = GameObject.Find("Artworks").GetComponent<DataJsonSerializer>();
            Assert.IsNotNull(paintsSerializer);

            var cd = new CoroutineWithData(_coroutineCaller, MeumDB.Get().GetRoomInfo2(roomId));
            yield return cd.coroutine;

            RoomInfoData roomInfoData = cd.result as RoomInfoData;
            paintsSerializer.SetJson(roomInfoData);
        }

        #endregion

        #region LoadGallery

        public void LoadGallery(SocketEventHandler.EnteringSuccessEventData data)
        {
            //Assert.IsTrue(_state.IsNotInGalleryOrSquare() || _state.IsSubOfGalleryOwn());
            _coroutineCaller.StartCoroutine(LoadGalleryCoroutine2(data));
        }
        
        private IEnumerator LoadGalleryCoroutine2(SocketEventHandler.EnteringSuccessEventData data)
        {
            var sceneOpen = SceneManager.LoadSceneAsync("Loading");
            while (!sceneOpen.isDone) yield return null;
            var progressBar = GameObject.Find("ProgressBar").GetComponent<UI.ProgressBar>();
            Assert.IsNotNull(progressBar);

            bool nextOn = false;

            RoomInfoData roomInfo = null;

            RoomRequest roomRequest = new RoomRequest()
            {
                requestStatus = 0,
                id = data.roomId,
                successOn = ResultData =>
                {
                    RoomInfoRespons respons = (RoomInfoRespons)ResultData;
                    roomInfo = respons.result;
                    nextOn = true;
                }
            };
            roomRequest.RequestOn();

            yield return new WaitUntil(() => nextOn);

            Assert.IsNotNull(roomInfo);

            if (roomInfo.data_json != string.Empty)
            {
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
                    var cd = MeumDB.Get().GetTextureCoroutine(url);
                    yield return cd.coroutine;
                    Assert.IsNotNull(cd.result);
                    //progressBar.SetProgress((float)loadCompleteCnt / totalCnt);
                    progressBar.SetProgress((float)loadCompleteCnt);
                    ++loadCompleteCnt;
                }

                
                foreach (var url in set3d)
                {
                    yield return AddressableManager.Insatnce.DownLoadObj(url);

                    progressBar.SetProgress((float)loadCompleteCnt);
                    ++loadCompleteCnt;
                }
                
                //어드레서블에서 겔러리 미리 로딩
                for (var i = 0; i < artworksData.lands.Length; i++)
                {
                    nextOn = false;

                    int type = artworksData.lands[i].type;

                    string objName = string.Format("gallery_type_{0}", type);

                    yield return AddressableManager.Insatnce.DownLoadObj(objName);
                }
            }


            nextOn = false;

            UserData userInfo = null;

            UserInfoRequest userInfoRequest = new UserInfoRequest()
            {
                requestStatus = 0,
                uid = MeumDB.Get().GetToken(),
                successOn = ResultData =>
                {
                    UserInfoRespons respons = (UserInfoRespons)ResultData;
                    userInfo = respons.result;
                    nextOn = true;
                }
            };
            userInfoRequest.RequestOn();

            yield return new WaitUntil(() => nextOn);

            _playerPk = userInfo.user_id;

            RoomInfoData ownRoomInfo = null;

            nextOn = false;
            RoomRequest roomRequest2 = new RoomRequest()
            {
                requestStatus = 1,
                uid = userInfo.user_id,
                successOn = ResultData =>
                {
                    RoomInfoRespons respons = (RoomInfoRespons)ResultData;
                    ownRoomInfo = respons.result;

                    nextOn = true;
                }
            };
            roomRequest2.RequestOn();

            yield return new WaitUntil(() => nextOn);

            MeumDB.Get().myRoomInfo = ownRoomInfo;

            var ownRoomId = ownRoomInfo.owner.id;

            // load scene
            progressBar.SetProgress(0);
            sceneOpen = SceneManager.LoadSceneAsync("ProceduralGallery");
            while (!sceneOpen.isDone)
            {
                progressBar.SetProgress(sceneOpen.progress);
                yield return null;
            }

            if (ownRoomId == data.roomId)
                _state.EnterGalleryOwn();
            else
                _state.EnterGallery();

            // save galleryData
            _galleryData = data;

            // setting paints
            //SerializeArtworks();
            var paintsSerializer = GameObject.Find("Artworks").GetComponent<DataJsonSerializer>();
            paintsSerializer.SetJson(roomInfo);

            // setting userList
            var userList = UI.UserList.UserList.Get();
            var socket = MeumSocket.Get();
            userList.SetOwnerName(roomInfo.owner.nickname);

            userList.AddUser(socket.GetPlayerId(), socket.LocalPlayerInfo.nickname);
            //userList.AddUser(socket.GetPlayerId(), "nickName");

            if (DataSynchronizer.Get().maxN != data.maxN) 
            {
                DataSynchronizer.Get().Setup(data.maxN);
            }
            else
            {
                DataSynchronizer.Get().ShowPlayers();
            }   
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

        public void LoadChaEditScene()
        {
            _coroutineCaller.StartCoroutine(LoadEditChaSceneCoroutine());
        }

        private IEnumerator LoadEditChaSceneCoroutine()
        {
            DataSynchronizer.Get().HidePlayers();

            var sceneOpen = SceneManager.LoadSceneAsync("Loading");
            while (!sceneOpen.isDone) yield return null;
            var progressBar = GameObject.Find("ProgressBar").GetComponent<UI.ProgressBar>();

            // load scene
            progressBar.SetProgress(0);
            sceneOpen = SceneManager.LoadSceneAsync("EditCha");
            while (!sceneOpen.isDone)
            {
                progressBar.SetProgress(sceneOpen.progress);
                yield return null;
            }
        }


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
            
            bool nextOn = false;

            List<ArtWorkData> artWorkDataList = new List<ArtWorkData>();

            ArtWorkRequest artWorkRequest = new ArtWorkRequest()
            {
                requestStatus = 1,
                uid = MeumDB.Get().GetToken(),
                successOn = ResultData =>
                {
                    artWorkDataList = ((ArtWorkRespons)ResultData).result;
                    nextOn = true;
                }
            };
            artWorkRequest.RequestOn();

            yield return new WaitUntil(() => nextOn);


            var output = new ArtWorkData[artWorkDataList.Count];
            for (var i = 0; i < artWorkDataList.Count; ++i)
            {
                output[i] = artWorkDataList[i];
            }

            var artworkInfos = output;

            nextOn = false;

            List<ShopByArtWorkData> shopByArtWorkDataList = new List<ShopByArtWorkData>();

            ArtWorkRequest artWorkRequest2 = new ArtWorkRequest()
            {
                requestStatus = 2,
                uid = MeumDB.Get().GetToken(),
                successOn = ResultData =>
                {
                    shopByArtWorkDataList = ((ShopByArtWorkRespons)ResultData).result;
                    nextOn = true;
                }
            };
            artWorkRequest2.RequestOn();

            yield return new WaitUntil(() => nextOn);

            var output2 = new ArtWorkData[shopByArtWorkDataList.Count];
            for (var i = 0; i < shopByArtWorkDataList.Count; ++i)
            {
                output2[i] = shopByArtWorkDataList[i].artwork;
            }

            var purchasedArtworkInfos = output2;

            string baseURL = "https://api.meum.me/datas/";

            var artworksCount = artworkInfos.Length + purchasedArtworkInfos.Length;
            for (var i = 0; i < artworkInfos.Length; ++i)
            {
                var artworkInfo = artworkInfos[i];
                if (!ReferenceEquals(artworkInfo.thumbnail, null))
                {

                    var textureGetter = MeumDB.Get().GetTextureCoroutine(baseURL+artworkInfo.thumbnail);
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
                    var textureGetter = MeumDB.Get().GetTextureCoroutine(baseURL + artworkInfo.thumbnail);
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
