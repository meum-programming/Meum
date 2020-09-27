using System;
using System.Collections;
using System.Collections.Generic;
using Gallery.Builder;
using UnityEngine;

public class LoadRoomBuilder : MonoBehaviour
{
    private ArtworkSerializer _serializer;
    private Transform _cam;
    
    private void Awake()
    {
        _serializer = GameObject.Find("Artworks").GetComponent<ArtworkSerializer>();
        _cam = GameObject.Find("camera").transform;
    }

    private void Start()
    {
        StartCoroutine(Load());
    }

    private IEnumerator Load()
    {
        var meumDB = Global.MeumDB.Get();
        var cd = new CoroutineWithData(this, meumDB.GetUserInfo());
        yield return cd.coroutine;
        var userInfo = cd.result as Global.MeumDB.UserInfo;
        
        cd = new CoroutineWithData(this, meumDB.GetRoomInfoWithUser(userInfo.primaryKey));
        yield return cd.coroutine;
        var roomInfo = cd.result as Global.MeumDB.RoomInfo;
        if (null != roomInfo)
        {
            var spawnSite = GameObject.Find("SpawnSite").transform;
            _cam.position = spawnSite.position;
            _cam.rotation = spawnSite.rotation;
            
            _serializer.SetJson(roomInfo.data_json);
        }
    }
}
