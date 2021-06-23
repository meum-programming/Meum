﻿using Core;
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
        [SerializeField] private GameObject wallPrefab;
        [FormerlySerializedAs("edge_length")] [SerializeField] private float edgeLength;

        private LandInfo[] _landInfos;
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

        private void BuildBlock(LandInfo pos , GameObject resultObj)
        {
            var position = new Vector3(pos.x * edgeLength, 0, pos.y * edgeLength);
            var floor = Instantiate(resultObj, _floors);
            floor.transform.position = position;
            floor.gameObject.SetActive(true);

            WallCreate(pos , position);
        }

        /// <summary>
        /// 바다로 못나가게 막는 벽 생성
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="position"></param>
        void WallCreate(LandInfo pos , Vector3 position)
        {
            //생성 안할것인지 체크
            if (NotCreateWallOn(pos.type))
                return;
            
            var wallY = wallPrefab.transform.position.y;
            // x axis walls
            if (!Has(pos.x - 1, pos.y))
            {
                var wall = Instantiate(wallPrefab, _walls);
                var wallPos = new Vector3(position.x - edgeLength / 2.0f, wallY, position.z);
                wall.transform.position = wallPos;
            }

            if (!Has(pos.x + 1, pos.y))
            {
                var wall = Instantiate(wallPrefab, _walls);
                var wallPos = new Vector3(position.x + edgeLength / 2.0f, wallY, position.z);
                wall.transform.position = wallPos;
            }

            // y axis walls
            if (!Has(pos.x, pos.y - 1))
            {
                var wall = Instantiate(wallPrefab, _walls);
                var wallPos = new Vector3(position.x, wallY, position.z - edgeLength / 2.0f);
                wall.transform.position = wallPos;
                wall.transform.Rotate(Vector3.up, 90.0f);
            }

            if (!Has(pos.x, pos.y + 1))
            {
                var wall = Instantiate(wallPrefab, _walls);
                var wallPos = new Vector3(position.x, wallY, position.z + edgeLength / 2.0f);
                wall.transform.position = wallPos;
                wall.transform.Rotate(Vector3.up, -90.0f);
            }
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

    }
}