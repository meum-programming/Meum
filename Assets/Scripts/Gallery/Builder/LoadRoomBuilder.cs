using System;
using System.Collections;
using System.Collections.Generic;
using Gallery.Builder;
using UnityEngine;

public class LoadRoomBuilder : MonoBehaviour
{
    [SerializeField] private GameObject[] mapPrefabs;

    private PaintsSerializer _serializer;
    private Transform _cam;
    
    private void Awake()
    {
        _serializer = GameObject.Find("Paints").GetComponent<PaintsSerializer>();
        _cam = GameObject.Find("camera").transform;
    }

    private void Start()
    {
        StartCoroutine(Load());
    }

    private IEnumerator Load()
    {
        var meumDB = MeumDB.Get();
        var cd = new CoroutineWithData(this, meumDB.GetUserInfo());
        yield return cd.coroutine;
        var userInfo = cd.result as MeumDB.UserInfo;
        
        cd = new CoroutineWithData(this, meumDB.GetRoomInfoWithUser(userInfo.primaryKey));
        yield return cd.coroutine;
        var roomInfo = cd.result as MeumDB.RoomInfo;
        if (null != roomInfo)
        {
            var map = Instantiate(mapPrefabs[roomInfo.type_int], Vector3.zero, Quaternion.identity);
            var spawnSite = map.transform.Find("SpawnSite").transform;
            _cam.position = spawnSite.position;
            _cam.rotation = spawnSite.rotation;
            
            _serializer.SetJson(roomInfo.data_json);
        }
    }
}
