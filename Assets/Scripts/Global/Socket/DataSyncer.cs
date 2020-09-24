using System.Collections.Generic;
using UnityEngine;

namespace Global.Socket
{
    public class DataSyncer : MonoBehaviour
    {
        [Header("Require Prefabs")] 
        [SerializeField] private List<GameObject> playerPrefabs;
        [SerializeField] private List<GameObject> otherPlayerPrefabs;

        private Transform _player = null;
        private Animator _playerAnimator = null;

        private OtherPlayerController[] _others = null;

        #region Singleton
        private static DataSyncer _globalInstance = null;
        public static DataSyncer Get()
        {
            return _globalInstance;
        }
        private void Awake()
        {
            if (_globalInstance == null)
            {
                DontDestroyOnLoad(gameObject);
                _globalInstance = this;
            }
            else
                Destroy(gameObject);
        }
        #endregion

        public void Setup(int maxN)
        {
            _others = new OtherPlayerController[maxN];

            var spawnTransform = GameObject.Find("SpawnSite").transform;
            var spawnPos = spawnTransform.position;
            var spawnRot = spawnTransform.rotation;
            for (int i = 0; i < _others.Length; ++i)
            {
                _others[i] = Instantiate(otherPlayerPrefabs[0], transform).GetComponent<OtherPlayerController>();
                _others[i].gameObject.SetActive(false);
                
                _others[i].UpdateTransform(spawnPos, spawnRot.eulerAngles);
                _others[i].SetOriginalTransform(spawnPos, spawnRot);
            }
            
            // setting player
            _player = Instantiate(playerPrefabs[0], transform).transform;
            _playerAnimator = _player.GetComponent<Animator>();
            _player.position = spawnTransform.position;
            _player.rotation = spawnTransform.rotation;
        }

        public void Reset()
        {
            for (var i = 0; i < _others.Length; ++i)
            {
                if(_others[i] != null)
                    Destroy(_others[i].gameObject);
            }
            Destroy(_player.gameObject);

            _others = null;
            _player = null;
            _playerAnimator = null;
        }

        public void DeactivatePlayer(int id)
        {
            if (id >= _others.Length)
            {
                Debug.LogError("Globa.Socket.DataSyncer - Deactivate : idx out of range");
                Application.Quit(-1);
            }
            _others[id].UserPrimaryKey = -1;
            _others[id].Nickname = "";
            _others[id].gameObject.SetActive(false);
        }

        public void UpdateOtherPlayer(SocketEventHandler.UserInfoEventData data)
        {
            var obj = _others[data.id];
            obj.gameObject.SetActive(true);
            obj.UpdateTransform(data.position, data.rotation);

            obj.UserPrimaryKey = data.userKey;
            obj.Nickname = data.nickname;
        }

        public void AnimTrigger(int id, string triggerName)
        {
            var obj = _others[id];
            if (obj.isActiveAndEnabled)
                obj.AnimTrigger(Animator.StringToHash(triggerName));
        }

        public void AnimBoolChange(int id, string boolName, bool value)
        {
            var obj = _others[id];
            if (obj.isActiveAndEnabled)
                obj.AnimBoolChange(Animator.StringToHash(boolName), value);
        }

        public void AnimFloatChange(int id, string floatName, float value)
        {
            var obj = _others[id];
            if (obj.isActiveAndEnabled)
                obj.AnimFloatChange(Animator.StringToHash(floatName), value);
        }

        public void ChangePlayerCharacter(int charId)
        {
            var playerOld = _player;
            var pPosition = playerOld.position;
            var pRotation = playerOld.rotation;
            
            _player = Instantiate(playerPrefabs[charId]).transform;
            _playerAnimator = _player.GetComponent<Animator>();
            _player.position = pPosition;
            _player.rotation = pRotation;
            
            Destroy(playerOld.gameObject);
        }

        public void ChangeOtherPlayerCharacter(int id, int charId)
        {
            var playerOld = _others[id].transform;
            var pPosition = playerOld.position;
            var pRotation = playerOld.rotation;
            
            _others[id] = Instantiate(otherPlayerPrefabs[charId], transform)
                .GetComponent<OtherPlayerController>();
            _others[id].UpdateTransform(pPosition, pRotation.eulerAngles);
            _others[id].SetOriginalTransform(pPosition, pRotation);
            
            Destroy(playerOld.gameObject);
        }

        public MeumSocket.BroadCastUserInfoData GetPlayerInfoData()
        {
            MeumSocket.BroadCastUserInfoData result;
            result.position = _player.position;
            result.rotation = _player.eulerAngles;

            var playerInfo = MeumSocket.Get().PlayerInfo;
            result.userKey = playerInfo.primaryKey;
            result.nickname = playerInfo.nickname;
            
            return result;
        }

        public int ID2UserPK(int id)
        {
            return _others[id].UserPrimaryKey;
        }

        public string ID2Nickname(int id)
        {
            return _others[id].Nickname;
        }

        public void Deactivate()
        {
            for (var i = 0; i < _others.Length; ++i)
            {
                if(_others[i] != null)
                    _others[i].SetRendererEnabled(false);
            }
            _player.gameObject.SetActive(false);
        }

        public void Activate()
        {
            for (var i = 0; i < _others.Length; ++i)
            {
                if(_others[i] != null)
                    _others[i].SetRendererEnabled(true);
            }
            _player.gameObject.SetActive(true);
        }

    }
}

