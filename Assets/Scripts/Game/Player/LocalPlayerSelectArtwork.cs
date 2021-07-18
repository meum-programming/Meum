using Game.Artwork;
using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;

namespace Game.Player
{
    /*
     * @brief 플레이어가 클릭시에 Artwork의 설명창을 보여주거나 연결된 링크를 여는 컴포넌트
     */
    public class LocalPlayerSelectArtwork : MonoBehaviour
    {
        [SerializeField] private Camera camera;

        [DllImport("__Internal")]
        private static extern void OpenURLNewTab(string url);

        public void Awake()
        {
        }

        private void Update()
        {
            InputCheck();
        }

        void InputCheck() 
        {
            if (Input.GetMouseButtonUp(0))
            {
                OnSelect();
            }
        }

        public void OnSelect()
        {
            //UI가 터치되었을때는 리턴
            if (EventSystem.current.currentSelectedGameObject != null)
                return;

            var ray = camera.ScreenPointToRay(Input.mousePosition);
            var layerMask = ~0;
            if (Physics.Raycast(ray, out var hit, 100.0f, layerMask))
            {
                if (hit.transform.CompareTag("Paint"))
                {
                    var artworkInfo = hit.transform.GetComponent<Artwork.ArtworkInfo>();

                    if (artworkInfo == null)
                    {
                        artworkInfo = hit.transform.GetComponentInParent<Artwork.ArtworkInfo>();
                    }

                    if (artworkInfo == null)
                    {
                        return;
                    }

#if UNITY_WEBGL
                    if (artworkInfo.bannerUrl != "")
                    {
                        string contensStr = string.Format("외부링크({0})로 \n이동합니다.정말 이동하시겠습니까?", FitURLMaxLength(artworkInfo.bannerUrl));

                        PopupManager.Instance.OkPopupCreate(
                            contensStr,
                            okBtnStr: "링크 이동",
                            cancelBtnStr: "정보 보기",
                            maskClickDestoryOn: true,
                            okBtnClickEvent: () => OpenURLNewTab(artworkInfo.bannerUrl),
                            cancelBtnClickEvent: () => StartCoroutine(SetArtworkDescription(artworkInfo))
                        );
                        Debug.Log(artworkInfo.bannerUrl);
                    }
#endif
                    if (artworkInfo.bannerUrl == "")
                    {
                        StartCoroutine(SetArtworkDescription(artworkInfo));
                    }
                }
            }
        }

        private string FitURLMaxLength(string s)
        {
            if (s.Length > 30)
                return s.Substring(0, 30) + "...";
            else
                return s;
        }

        /*
         * @brief 주어진 ArtworkInfo를 통해 ArtworkDescription UI를 보여줌
         */
        private IEnumerator SetArtworkDescription(Artwork.ArtworkInfo info)
        {
            var cd = new CoroutineWithData(this, info.GetArtworkInfo2());
            yield return cd.coroutine;

            ArtWorkData artworkInfo = (ArtWorkData)cd.result;
            
            Assert.IsNotNull(artworkInfo);

            var artworkDescriptionUi = UI.GalleryScene.ArtworkDescription.Get();
            Assert.IsNotNull(artworkDescriptionUi);
            artworkDescriptionUi.Show();
            artworkDescriptionUi.SetDescription(artworkInfo);
            
        }

    }
}
