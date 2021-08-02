using Core;
using Core.Socket;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Assertions;
using UnityEngine.Networking;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace Game
{
    [Serializable]
    public struct LandInfo
    {
        public int x;
        public int y;
        public int type;
    }
    
    /*
     * @brief 방 정보에 따라 알맞은 공간을 알맞은 위치에 생성하는 컴포넌트
     */
    public class ProceduralGalleryBuilder : MonoBehaviour
    {
        [FormerlySerializedAs("edge_length")] [SerializeField] private float edgeLength;

        private LandInfo[] _landInfos;
        public RoomInfoData roomInfoData;
        private Transform _walls;
        private Transform _floors;

        private void Awake()
        {
            Init();
        }

        void Init()
        {
            _walls = transform.Find("walls");
            _floors = transform.Find("floors");
        }

        public LandInfo[] GetLandInfos()
        {
            return _landInfos;
        }

        public void Build(LandInfo[] landInfos)
        {
            _landInfos = landInfos;

            for (var i = 0; i < _landInfos.Length; i++)
            {
                int index = i;
                int type = _landInfos[i].type;

                string objName = string.Format("gallery_type_{0}", type);

                AddressableManager.Insatnce.GetObj(objName, (GameObject obj) =>
                {
                    BuildBlock(_landInfos[index], obj);
                });
            }
        }

        /// <summary>
        /// 랜드타입으로 건물 로드
        /// </summary>
        /// <param name="roomInfoData"></param>
        public void Build_ToRandType(RoomInfoData roomInfoData)
        {
            this.roomInfoData = roomInfoData;

            int type = roomInfoData.land_type;

            string objName = string.Format("gallery_type_{0}", type);

            LandInfo[] pos = new LandInfo[1];
            pos[0] = new LandInfo();
            pos[0].type = type;

            _landInfos = pos;

            AddressableManager.Insatnce.GetObj(objName, (GameObject obj) =>
            {
                BuildBlock(pos[0], obj);
            });

            SkyBoxSet();
        }

        private void BuildBlock(LandInfo pos , GameObject resultObj)
        {
            var position = new Vector3(pos.x * edgeLength, 0, pos.y * edgeLength);
            var floor = Instantiate(resultObj, _floors);
            floor.transform.position = position;
            floor.gameObject.SetActive(true);
        }

        bool NotCreateWallOn(int type)
        {
            bool value = false;

            //화이트 큐브 (gallery_type_6)
            if (type == 6 || type == 13)
            {
                value = true;
            }

            return value;
        }

        private bool Has(int x, int y)
        {
            if (x == 0 && y == 0) return true;
            for (var i = 0; i < _landInfos.Length; ++i)
            {
                if (_landInfos[i].x == x && _landInfos[i].y == y)
                    return true;
            }

            return false;
        }

        public void SkyBoxSet()
        {
            int index = MeumDB.Get().currentRoomInfo.sky_type_int;
            SkyBoxSaveData skydata = Resources.Load<MeumSaveData>("MeumSaveData").GetSKYData((SkyBoxEnum)index);

            if (skydata != null)
            {
                RenderSettings.skybox = skydata.material;
            }
        }

        public void SkyBoxChange(int index)
        {
            MeumDB.Get().currentRoomInfo.sky_type_int = index;
            SkyBoxSaveData skydata = Resources.Load<MeumSaveData>("MeumSaveData").GetSKYData((SkyBoxEnum)index);
            RenderSettings.skybox = skydata.material;
        }

    }
}