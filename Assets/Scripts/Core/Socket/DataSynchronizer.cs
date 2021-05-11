using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace Core.Socket
{
    /*
     * @brief RemotePlayer, LocalPlayer를 관리
     * @details RemotePlayerController들과, LocalPlayer의 Transform을 저장하고 있음
     * MeumSocket으로부터 업데이트 요청을 받아 수행함
     */
    public class DataSynchronizer : Singleton<DataSynchronizer>
    {
        [Header("Require Prefabs")] 
        [SerializeField] private List<GameObject> playerPrefabs;
        [SerializeField] private List<GameObject> otherPlayerPrefabs;

        public Transform _localPlayer = null;
        private List<RemotePlayerController> _remotePlayers = null;

        public int maxN = 0;

        private void Awake()
        {
            base.Awake();
            DontDestroyOnLoad(gameObject);

            Init();

        }

        void Init()
        {
            _remotePlayers = new List<RemotePlayerController>();

            for (int i = 0; i < 10; i++)
            {
                CreateRemoteObj();
            }
        }

        void CreateRemoteObj()
        {
            RemotePlayerController obj = Instantiate(otherPlayerPrefabs[0], transform).GetComponent<RemotePlayerController>();
            obj.gameObject.SetActive(false);
            _remotePlayers.Add(obj);
        }


        /*
         * @brief DataSynchronizer의 초기화 함수
         * @details RemotePlayer들과 LocalPlayer를 생성 (DataSynchronizer의 자식으로 들어감)
         */
        public void Setup(int maxN)
        {
            //Clean();

            this.maxN = maxN;

            LocalPlayerSet();

        }

        public Vector3 GetSpawnPos()
        {
            //현재 룸 정보 가져오기
            RoomInfoData roomInfoData = MeumDB.Get().currentRoomInfo;

            //룸 정보에서 시작 위치 가져오기
            Vector3 spawnPos = new Vector3(-10f, 1.5f, 0);

            if (roomInfoData.startPoint_json != string.Empty) 
            {
                StartPointData startPointData = JsonUtility.FromJson<StartPointData>(roomInfoData.startPoint_json);
                spawnPos = startPointData.position;
            }

            return spawnPos;
        }

        public Quaternion GetSpawnRot()
        {
            //현재 룸 정보 가져오기
            RoomInfoData roomInfoData = MeumDB.Get().currentRoomInfo;

            //로테이트 값 세팅
            Quaternion spawnRot = Quaternion.Euler(new Vector3(0, 90, 0));

            if (roomInfoData.startPoint_json != string.Empty)
            {
                StartPointData startPointData = JsonUtility.FromJson<StartPointData>(roomInfoData.startPoint_json);
                spawnRot = Quaternion.Euler(startPointData.eulerAngle);
            }

            return spawnRot;
        }


        void LocalPlayerSet()
        {
            // setting localplayer
            _localPlayer.transform.localPosition = GetSpawnPos();
            _localPlayer.transform.rotation = GetSpawnRot();

            var playerInfo = MeumSocket.Get().LocalPlayerInfo;

            ChaCustomizingSaveData localPlayerData = new ChaCustomizingSaveData(playerInfo.hairIndex, playerInfo.maskIndex, playerInfo.dressIndex, playerInfo.skinIndex);

            DataManager.Instance.chaCustomizingSaveData = localPlayerData;

            _localPlayer.GetComponentInChildren<PlayerChaChange>().GetChaCustomizingSaveData();

            _localPlayer.gameObject.SetActive(true);
        }

        
        /*
         * @brief DataSynchronizer의 Clean 함수
         * @details RemotePlayer들과 LocalPlayer를 제거
         */
        public void Clean()
        {
            if (!ReferenceEquals(_remotePlayers, null))
            {
                for (var i = 0; i < _remotePlayers.Count; ++i)
                {
                    if (!ReferenceEquals(_remotePlayers[i], null))
                        Destroy(_remotePlayers[i].gameObject);
                }
            }

            if(!ReferenceEquals(_localPlayer, null))
                Destroy(_localPlayer.gameObject);

            _remotePlayers = null;
            _localPlayer = null;
        }
        
        /*
         * @brief 특정 RemotePlayer GameObject를 비활성화
         * @details 특정 플레이어가 방에서 나갔을 때 호출되어 해당 GameObject를 비활성화시킴
         * 추가적으로 UserListUI 에서도 제거함
         */
        public void DeactivatePlayer(int id)
        {
            if (id >= _remotePlayers.Count)
            {
                Debug.LogError("Global.Socket.DataSyncer - Deactivate : idx out of range");
                Application.Quit(-1);
            }

            var obj = GetRemotePlayer(id);

            obj.UserPrimaryKey = -1;
            obj.Nickname = "";
            obj.gameObject.SetActive(false);
            
            var userList = UI.UserList.UserList.Get();
            if(userList != null)
                userList.RemoveUser(id);
        }
        
        #region SocketEvent Recievers
        
        public void UpdateOtherPlayer(SocketEventHandler.UserInfoEventData data)
        {
            var obj = GetRemotePlayer(data.id);
            obj.gameObject.SetActive(true);
            obj.UpdateTransform(data.position, data.rotation);

            obj.UserPrimaryKey = data.userKey;
            obj.Nickname = data.nickname;

            obj.playerChaChange.AllChangeData(new ChaCustomizingSaveData(data.hairIndex, data.maskIndex, data.dressIndex, data.skinIndex));

            // Update UserList
            var userList = UI.UserList.UserList.Get();
            if (userList != null && !userList.HasUser(data.id))
                userList.AddUser(data.id, data.nickname);
        }

        public void AnimTrigger(int id, string triggerName)
        {
            var obj = GetRemotePlayer(id);
            if (obj.isActiveAndEnabled)
                obj.AnimTrigger(Animator.StringToHash(triggerName));
        }

        public void AnimGesture(int id, string animName)
        {
            var obj = GetRemotePlayer(id);
            if (obj.isActiveAndEnabled)
                obj.AnimGesture(animName);
        }

        public void AnimBoolChange(int id, string boolName, bool value)
        {
            var obj = GetRemotePlayer(id);
            if (obj.isActiveAndEnabled)
                obj.AnimBoolChange(Animator.StringToHash(boolName), value);
        }

        public void AnimFloatChange(int id, string floatName, float value)
        {
            var obj = GetRemotePlayer(id);
            if (obj.isActiveAndEnabled)
                obj.AnimFloatChange(Animator.StringToHash(floatName), value);
        }

        RemotePlayerController GetRemotePlayer(int id)
        {
            if (id >= _remotePlayers.Count)
            {
                var spawnTransform = GameObject.Find("SpawnSite").transform;
                Assert.IsNotNull(spawnTransform);
                var spawnPos = spawnTransform.position;
                var spawnRot = spawnTransform.rotation;

                for (int i = _remotePlayers.Count; i <= id; ++i)
                {
                    CreateRemoteObj();

                    _remotePlayers[i].UpdateTransform(spawnPos, spawnRot.eulerAngles);
                    _remotePlayers[i].SetOriginalTransform(spawnPos, spawnRot);
                }
            }

            return _remotePlayers[id];

        }

        public void ChangePlayerCharacter(int charId)
        {
            var playerOld = _localPlayer;
            var pPosition = playerOld.position;
            var pRotation = playerOld.rotation;
            
            _localPlayer = Instantiate(playerPrefabs[charId]).transform;
            _localPlayer.position = pPosition;
            _localPlayer.rotation = pRotation;
            
            Destroy(playerOld.gameObject);
        }

        public void ChangeOtherPlayerCharacter(int id, int charId)
        {
            var playerOld = _remotePlayers[id].transform;
            var pPosition = playerOld.position;
            var pRotation = playerOld.rotation;
            
            _remotePlayers[id] = Instantiate(otherPlayerPrefabs[charId], transform)
                .GetComponent<RemotePlayerController>();
            _remotePlayers[id].UpdateTransform(pPosition, pRotation.eulerAngles);
            _remotePlayers[id].SetOriginalTransform(pPosition, pRotation);
            
            Destroy(playerOld.gameObject);
        }
        
        #endregion
        
        /*
         * @brief LocalPlayer의 정보를 리턴함 MeumSocket에서 이를 Broadcasting
         */
        public MeumSocket.BroadCastUserInfoData GetPlayerInfoData()
        {
            MeumSocket.BroadCastUserInfoData result;
            result.position = _localPlayer.Find("Human_local").position;
            result.rotation = _localPlayer.Find("Human_local/character").eulerAngles;

            var playerInfo = MeumSocket.Get().LocalPlayerInfo;
            result.userKey = playerInfo.id;
            result.nickname = playerInfo.nickname;

            result.hairIndex = playerInfo.hairIndex;
            result.maskIndex = playerInfo.maskIndex;
            result.dressIndex = playerInfo.dressIndex;
            result.skinIndex = playerInfo.skinIndex;

            return result;
        }
        
        /*
         * @brief 방에서 내부적으로 사용되는 userID 를 DB에서 사용되는 user PrimaryKey 로 변환
         */
        public int Id2UserPK(int id)
        {
            return _remotePlayers[id].UserPrimaryKey;
        }
        
        /*
         * @brief 방에서 내부적으로 사용되는 userID를 Nickname으로 변환
         */
        public string Id2Nickname(int id)
        {
            return _remotePlayers[id].Nickname;
        }
        
        /*
         * @brief Nickname을 방에서 내부적으로 사용되는 userID로 변환
         */
        public int Nickname2Id(string nickname)
        {
            for (var i = 0; i < _remotePlayers.Count; ++i)
            {
                if (!ReferenceEquals(_remotePlayers[i], null))
                {
                    if (_remotePlayers[i].Nickname == nickname)
                        return i;
                }
            }
            return -1;
        }
        
        /*
         * @brief 플레이어들을 안보이도록 함
         * @details Edit Scene으로 갈 때 호출되어서 유저들이 보이지 않도록 함
         */
        public void HidePlayers()
        {
            for (var i = 0; i < _remotePlayers.Count; ++i)
            {
                if(!ReferenceEquals(_remotePlayers[i], null))
                    _remotePlayers[i].SetRendererEnabled(false);
            }
            _localPlayer.gameObject.SetActive(false);
        }
        
        /*
         * @brief 플레이어들을 다시 보이도록
         * @details Gallery Scene으로 돌아갈 때 호출되어서 유저들이 다시 보이도록 함
         */
        public void ShowPlayers()
        {
            for (var i = 0; i < _remotePlayers.Count; ++i)
            {
                if(!ReferenceEquals(_remotePlayers[i], null))
                    _remotePlayers[i].SetRendererEnabled(true);
            }
            _localPlayer.gameObject.SetActive(true);
            _localPlayer.GetComponentInChildren<PlayerChaChange>().GetChaCustomizingSaveData();

            //겔러리 돌아갔을때 발이 파묻혀서 임시로 중력을 없앰
            StartCoroutine(GravitySet());
        }

        private IEnumerator GravitySet()
        {
            GravitySet(_localPlayer.GetComponent<CharacterController>(), 0);

            yield return new WaitForSeconds(1);

            GravitySet(_localPlayer.GetComponent<CharacterController>(), 0.9f);
        }

        private void GravitySet(CharacterController controller , float value) 
        {
            Vector3 center = controller.center;
            center.y = value;
            controller.center = center;

            Vector3 pos = controller.transform.localPosition;
            pos.y += 0.01f;
            controller.transform.localPosition = pos;
        }

        public Transform GetLocalPlayer()
        {
            return _localPlayer;
        }

    }
}

