using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIO;
using UnityEngine.SceneManagement;

public class DataSyncer : MonoBehaviour
{
    [Header("Connection Info")]
    [SerializeField] private float sendInterval;
    [SerializeField] private float receiveInterval;
    private float _sendIntervalCounter = 0.0f;
    private float _receiveIntervalCounter = 0.0f;

    [Header("Require Prefabs")] 
    [SerializeField] private List<GameObject> playerPrefabs;
    [SerializeField] private List<GameObject> otherPlayerPrefabs;
    
    [Header("Scenes")]
    [SerializeField] private List<string> scenes;
    
    private Transform _player = null;
    private Animator _playerAnimator = null;
    private int _currentCharId = 0;
    public int currentCharId { get { return _currentCharId;  } }
    private OtherPlayerController[] _others = null;

    private SocketIOComponentCoroutine _socket = null;
    private int _id = -1;
    private Dictionary<string, string> _dataCache = new Dictionary<string, string>();
    private JSONObject _jsonCache = new JSONObject();

    private void Awake()
    {
        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;
        transform.localScale = new Vector3(1, 1, 1);
        
        //_socket = GetComponent<SocketIOComponent>();
        _socket = GetComponent<SocketIOComponentCoroutine>();
        _socket.On("enteringSuccess", OnEnteringSuccess);
        _socket.On("userData", OnUserData);
        _socket.On("animTrigger", OnAnimTrigger);
        _socket.On("changeCharacter", OnChangeCharacter);
    }

    public void BroadCastAnimTrigger(string paramName)
    {
        _jsonCache.Clear();
        _jsonCache.AddField("name", paramName);
        _socket.Emit("broadCastAnimTrigger", _jsonCache);
    }

    public void ChangeCharacter(int charId)
    {
        _jsonCache.Clear();
        _jsonCache.AddField("charId", charId);
        _socket.Emit("broadCastChangeCharacter", _jsonCache);
    }

    public void ResetRoom()
    {
        _player = null;
        _playerAnimator = null;
        _id = -1;
        for (int i = 0; i < _others.Length; ++i)
        {
            if(_others[i] == null) continue;
            Destroy(_others[i].gameObject);
        }
    }

    private void Update()
    {
        if (_id != -1)
        {
            _sendIntervalCounter += Time.deltaTime;
            _receiveIntervalCounter += Time.deltaTime;
            if (_sendIntervalCounter > sendInterval)
            {
                _jsonCache.Clear();
                _jsonCache.AddField("position", Vector2Json(_player.position));
                _jsonCache.AddField("rotation", Vector2Json(_player.eulerAngles));
                _jsonCache.AddField("walking", _playerAnimator.GetBool(Animator.StringToHash("walking")));
                _socket.Emit("updateUserInfo", _jsonCache);
                _sendIntervalCounter = 0.0f;
            }

            if (_receiveIntervalCounter > receiveInterval)
            {
                _socket.Emit("getUsersInfo");
                _receiveIntervalCounter = 0.0f;
            }
        }
    }

    private JSONObject Vector2Json(Vector3 v)
    {
        var ret = new JSONObject();
        ret.AddField("x", v.x);
        ret.AddField("y", v.y);
        ret.AddField("z", v.z);
        return ret;
    }

    private IEnumerator WaitLoadingGallery(SocketIOEvent e)
    {
        var sceneName = scenes[Mathf.RoundToInt(e.data.GetField("roomType").n)];
        var loading = SceneManager.LoadSceneAsync(sceneName);
        while (!loading.isDone) yield return null;
        
        _id = Convert.ToInt32(e.data.GetField("id").str);
        _others = new OtherPlayerController[Mathf.RoundToInt(e.data.GetField("maxN").n)];

        var spawnTransform = GameObject.Find("SpawnSite").transform;
        var spawnPos = spawnTransform.position;
        var spawnRot = spawnTransform.rotation;
        for (int i = 0; i < _others.Length; ++i)
        {
            if (i == _id) continue;
            _others[i] = Instantiate(otherPlayerPrefabs[0], transform).GetComponent<OtherPlayerController>();
            _others[i].gameObject.SetActive(false);
            _others[i].UpdateTransform(spawnPos, spawnRot.eulerAngles);
            _others[i].SetOriginalTransform(spawnPos, spawnRot);
        }

        Debug.Log("My id is " + _id);
        Debug.Log("maxN is " + _others.Length);
        Debug.Log("room id is " + e.data.GetField("roomId"));

        _player = Instantiate(playerPrefabs[0]).transform;
        _playerAnimator = _player.GetComponent<Animator>();
        _currentCharId = 0;
        _player.position = spawnTransform.position;
        _player.rotation = spawnTransform.rotation;
    }
    private void OnEnteringSuccess(SocketIOEvent e)
    {
        StartCoroutine(WaitLoadingGallery(e));
    }

    private Vector3 JsonObj2Vec3(JSONObject obj)
    {
        return new Vector3(obj.GetField("x").n, 
                           obj.GetField("y").n,
                           obj.GetField("z").n);
    }

    private void OnUserData(SocketIOEvent e)
    {
        for (var i = 0; i < _others.Length; ++i)
        {
            if (i == _id) continue;
            var fieldName = i.ToString();
            if (e.data.HasField(fieldName))
            {
                var obj = e.data.GetField(fieldName);
                _others[i].gameObject.SetActive(true);
                _others[i].UpdateTransform(JsonObj2Vec3(obj.GetField("position")),
                                            JsonObj2Vec3(obj.GetField("rotation")));
                _others[i].AnimBoolChange(Animator.StringToHash("walking"), obj.GetField("walking").b);
            }
            else
                _others[i].gameObject.SetActive(false);
        }
    }

    private void OnAnimTrigger(SocketIOEvent e)
    {
        var id = Convert.ToInt32(e.data.GetField("id").str);
        if (id == _id) return;
        var obj = _others[id];
        if(obj.isActiveAndEnabled)
            obj.AnimTrigger(Animator.StringToHash(e.data.GetField("name").str));
    }
    
    private void OnChangeCharacter(SocketIOEvent e)
    {
        var id = Convert.ToInt32(e.data.GetField("id").str);
        var charId = Mathf.RoundToInt(e.data.GetField("charId").n);
        
        if (id == _id)
        {
            if (_currentCharId == charId) return;
            var playerOld = _player;
            var pPosition = playerOld.position;
            var pRotation = playerOld.rotation;
            _player = Instantiate(playerPrefabs[charId]).transform;
            _playerAnimator = _player.GetComponent<Animator>();
            _currentCharId = charId;
            _player.position = pPosition;
            _player.rotation = pRotation;
            Destroy(playerOld.gameObject);
        }
        else
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
    }
}
