using System;
using System.Collections;
using System.Collections.Generic;
using Gallery.Builder;
using Gallery.MultiPlay;
using UnityEngine;
using UnitySocketIO;
using Newtonsoft.Json;
using UnityEngine.SceneManagement;
using UnitySocketIO.Events;

public class MeumSocket : MonoBehaviour
{
    private SocketIOController _socket;
    
    private EnteringSuccessEventData _currentData = null;

    private static MeumSocket _globalInstance = null;
    public static MeumSocket Get()
    {
        return _globalInstance;
    }
    
    private void Awake()
    {
        if (_globalInstance == null)
        {
            var selfTransform = transform;
            selfTransform.position = Vector3.zero;
            selfTransform.rotation = Quaternion.identity;
            selfTransform.localScale = new Vector3(1, 1, 1);
                
            _socket = GetComponent<SocketIOController>();
                
            DontDestroyOnLoad(gameObject);
            _globalInstance = this;
        }
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        _socket.On("enteringSuccess", OnEnteringSuccess);
        _socket.On("squareEnteringSuccess", OnSquareEnteringSuccess);
        _socket.Connect();
    }

    private void OnApplicationQuit()
    {
        _socket.Emit("disconnect");
    }

    public void ReturnToRoom()
    {
        StartCoroutine(LoadingGallery(_currentData));
    }

    public void TemporaryLeaveGallery()
    {
        DataSyncer.Get().TemporaryLeaveRoom();
    }

    #region Socket Emitters
    
    private struct EnteringRoomData
    {
        public int roomId;
        public int maxN;
    }
    public void EnterRoom(string nickname)
    {
        if (DataSyncer.Get().IsInRoomOrSquare())
        {
            Debug.LogError("cannot go square When your in room or square");
            return;
        }
        
        StartCoroutine(EnterCoroutine(nickname));
    }
    private IEnumerator EnterCoroutine(string nickname)
    {
        var cd = new CoroutineWithData(this, MeumDB.Get().GetUserInfo(nickname));
        yield return cd.coroutine;
        var userInfo = cd.result as MeumDB.UserInfo;
        if (null != userInfo)
        {
            cd = new CoroutineWithData(this, MeumDB.Get().GetRoomInfoWithUser(userInfo.primaryKey));
            yield return cd.coroutine;
            var roomInfo = cd.result as MeumDB.RoomInfo;
            if (null != roomInfo)
            {
                EnteringRoomData data;
                data.roomId = roomInfo.primaryKey;
                data.maxN = roomInfo.max_people;
                _socket.Emit("enteringRoom", JsonConvert.SerializeObject(data));
            }
        }
    }

    public void EnterSquare()
    {
        if (DataSyncer.Get().IsInRoomOrSquare())
        {
            Debug.LogError("cannot go square When your in room or square");
            return;
        }

        _socket.Emit("enteringSquare");
    }

    public void QuitRoom()
    {
        DataSyncer.Get().Reset();
        _socket.Emit("quitRoom");
        _currentData = null;
    }
    
    #endregion
    
    #region Socket Event Handlers
    
    [System.Serializable]
    private class EnteringSuccessEventData
    {
        public int id;
        public int roomId;
        public int maxN;
    }
    private IEnumerator LoadingGallery(EnteringSuccessEventData data)
    {
        // wait load complete
        var loading = SceneManager.LoadSceneAsync("ProceduralGallery");
        while (!loading.isDone) yield return null;

        if(_currentData == null)
            DataSyncer.Get().SetupRoom(data.id, data.maxN, _socket);
        else
            DataSyncer.Get().ReturnToRoom();
        
        _currentData = data;
        
        // setting paints
        SerializeArtworks();
    }
    public void SerializeArtworks()
    {
        if (_currentData == null) return;
        StartCoroutine(SerializeArtworksCoroutine(_currentData.roomId));
    }
    private IEnumerator SerializeArtworksCoroutine(int roomId)
    {
        var paintsSerializer = GameObject.Find("Artworks").GetComponent<ArtworkSerializer>();
        var cd = new CoroutineWithData(this, MeumDB.Get().GetRoomInfo(roomId));
        yield return cd.coroutine;
        var roomInfo = cd.result as MeumDB.RoomInfo;
        Debug.Assert(roomInfo != null);
        paintsSerializer.SetJson(roomInfo.data_json);
    }
    private void OnEnteringSuccess(SocketIOEvent e)
    {
        var data = JsonConvert.DeserializeObject<EnteringSuccessEventData>(e.data);
        // print data
        // Debug.Log("My id is " + data.id);
        // Debug.Log("maxN is " + data.maxN);
        // Debug.Log("room id is " + data.roomId);
        
        StartCoroutine(LoadingGallery(data));
    }
    
    [System.Serializable]
    private class SquareEnteringSuccessEventData
    {
        public int id;
        public int maxN;
    }
    private IEnumerator LoadingSquare(SquareEnteringSuccessEventData data)
    {
        var loading = SceneManager.LoadSceneAsync("Square");
        while (!loading.isDone) yield return null;
        
        DataSyncer.Get().SetupRoom(data.id, data.maxN, _socket);
        
        // TODO: edit for temporary leave method
    }
    private void OnSquareEnteringSuccess(SocketIOEvent e)
    {
        var data = JsonConvert.DeserializeObject<SquareEnteringSuccessEventData>(e.data);
        StartCoroutine(LoadingSquare(data));
    }

    #endregion
}
