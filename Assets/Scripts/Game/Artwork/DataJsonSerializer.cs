using System;
using System.Collections;
using System.Collections.Generic;
using Core;
using UnityEngine;
using UnityEngine.Assertions;

namespace Game.Artwork
{
    /*
     * @brief DB의 roomData 속의 data_json 에 저장되는 정보 구조체
     */
    [Serializable]
    struct RoomData
    {
        public ArtworkData[] artworks;
        public LandInfo[] lands;

        public RoomData(int n)
        {
            artworks = new ArtworkData[n];
            lands = null;
        }
    }

    /*
     * @brief (DB -> Artworks, LandsInfo), (Artworks, LandsInfo -> DB) 변환을 수행하는 컴포넌트 
     */
    public class DataJsonSerializer : MonoBehaviour
    {
        [SerializeField] public GameObject paintPrefab;
        [SerializeField] public GameObject object3DPrefab;
        [SerializeField] public GameObject videoPrefab;

        private string _resetCheckpoint = "";

        [SerializeField] public List<GameObject> tempList = new List<GameObject>();

        /*
         * @brief Awake 함수
         * @details Artwork들이 자식으로 들어가기때문에 Transform을 초기화해줌
         */
        private void Awake()
        {
            transform.position = Vector3.zero;
            transform.eulerAngles = Vector3.zero;
            transform.localScale = new Vector3(1, 1, 1);
        }

        /*
         * @brief Artworks, LandsInfo -> json data 변환 함수
         */
        private string GetJson()
        {
            var selfTransform = transform;
            var data = new RoomData(selfTransform.childCount);
            for (int i = 0; i < selfTransform.childCount; ++i)
                data.artworks[i] = selfTransform.GetChild(i).GetComponent<ArtworkInfo>().GetArtworkData();

            var proceduralSpace = GameObject.Find("proceduralSpace");
            Assert.IsNotNull(proceduralSpace);
            data.lands = proceduralSpace.GetComponent<ProceduralGalleryBuilder>().GetLandInfos();

            return JsonUtility.ToJson(data);
        }
        
        /*
         * @brief json_data -> Artworks, LandInfo 변환 함수
         */
        public void SetJson(string json)
        {
            var data = JsonUtility.FromJson<RoomData>(json);
            
            var proceduralSpace = GameObject.Find("proceduralSpace");
            Assert.IsNotNull(proceduralSpace);
            proceduralSpace.GetComponent<ProceduralGalleryBuilder>().Build(data.lands);

            ClearArtworks();
            
            for (var i = 0; i < data.artworks.Length; ++i)
            {
                ArtworkInfo artworkInfo = null;
                if(data.artworks[i].artwork_type == 0)
                    artworkInfo = Instantiate(paintPrefab, transform).GetComponent<ArtworkInfo>();
                else if (data.artworks[i].artwork_type == 1)
                    artworkInfo = Instantiate(object3DPrefab, transform).GetComponent<ArtworkInfo>();
                Assert.IsNotNull(artworkInfo);
                artworkInfo.UpdateWithArtworkData(data.artworks[i]);
            }

            //영상 테스트 용 (블랙 겔러리에서만 보이도록 수정)
            if (MeumDB.Get().currentRoomInfo.id == 146)
            {
                ArtworkInfo videoArtworkInfo = Instantiate(videoPrefab, transform).GetComponent<ArtworkInfo>();

                videoArtworkInfo.transform.position = new Vector3(10.2f, 2.4f, -4.6f);
                videoArtworkInfo.transform.eulerAngles = new Vector3(270, 0, 270);
                videoArtworkInfo.transform.localScale = new Vector3(6.5f, 1, 4);
                videoArtworkInfo.LoadVideoCoroutine("https://www.youtube.com/watch?v=v1DWS9-0zSc");
            }

            //새로운 3D 오브젝트 임시 설치
            if (MeumDB.Get().currentRoomInfo.owner.user_id == 238)
            {
                for (int i = 0; i < tempList.Count; i++)
                {
                    tempList[i].gameObject.SetActive(true);
                }
            }

            _resetCheckpoint = json;
        }
        
        private void ClearArtworks()
        {
            var selfTransform = transform;
            for (var i = 0; i < selfTransform.childCount; ++i)
                Destroy(selfTransform.GetChild(i).gameObject);
        }

        public void Save()
        {
            StartCoroutine(PatchRoomJson2(GetJson()));
        }

        private IEnumerator PatchRoomJson2(string json)
        {
            new RoomRequest()
            {
                requestStatus = 4,
                uid = MeumDB.Get().GetToken(),
                data_json = json,
            }.RequestOn();

            Core.Socket.MeumSocket.Get().BroadCastUpdateArtworks();

            _resetCheckpoint = json;

            yield return null;
        }

        public void Clean()
        {
            var artworkPlacer = GetComponent<ArtworkPlacer>();
            Assert.IsNotNull(artworkPlacer);
            artworkPlacer.Deselect();
            
            for(var i=0; i<transform.childCount; ++i)
                Destroy(transform.GetChild(i).gameObject);
        }

        public void Reset()
        {
            var artworkPlacer = GetComponent<ArtworkPlacer>();
            Assert.IsNotNull(artworkPlacer);
            artworkPlacer.Deselect();
            
            SetJson(_resetCheckpoint);
        }
    }
}
