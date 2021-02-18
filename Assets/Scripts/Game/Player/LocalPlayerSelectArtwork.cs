using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;

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

        [SerializeField] Joystick joystick;

        public void Awake()
        {
            joystick = FindObjectOfType<Joystick>();
        }

        public void OnSelect(InputAction.CallbackContext ctx)
        {
            if (!ctx.canceled) return;

            if (joystick.moveOn)
               return;

            var ray = camera.ScreenPointToRay(Mouse.current.position.ReadValue());
            var layerMask = ~0;
            if (Physics.Raycast(ray, out var hit, 100.0f, layerMask))
            {
                if (hit.transform.CompareTag("Paint"))
                {
                    var artworkInfo = hit.transform.GetComponent<Artwork.ArtworkInfo>();
#if UNITY_WEBGL
                    if (artworkInfo.bannerUrl != "")
                    {
                        Debug.Log(artworkInfo.bannerUrl);
                        OpenURLNewTab(artworkInfo.bannerUrl);
                    }
#endif
                    if (artworkInfo.bannerUrl == "")
                    {
                        //StartCoroutine(SetArtworkDescription(artworkInfo));
                        StartCoroutine(SetArtworkDescription2(artworkInfo));
                    }
                }
            }
        }
        
        /*
         * @brief 주어진 ArtworkInfo를 통해 ArtworkDescription UI를 보여줌
         */
        private IEnumerator SetArtworkDescription(Artwork.ArtworkInfo info)
        {
            var cd = new CoroutineWithData(this, info.GetArtworkInfo());
            yield return cd.coroutine;
            var artworkInfo = cd.result as Core.MeumDB.ArtworkInfo;
            Assert.IsNotNull(artworkInfo);
            
            var artworkDescriptionUi = UI.GalleryScene.ArtworkDescription.Get();
            Assert.IsNotNull(artworkDescriptionUi);
            artworkDescriptionUi.SetDescription(artworkInfo);
            artworkDescriptionUi.Show();
        }
        private IEnumerator SetArtworkDescription2(Artwork.ArtworkInfo info)
        {
            var cd = new CoroutineWithData(this, info.GetArtworkInfo2());
            yield return cd.coroutine;

            Core.MeumDB.ArtworkInfo artworkInfo = ArtWorkDataToArtworkInfo((ArtWorkData)cd.result);
            
            Assert.IsNotNull(artworkInfo);

            var artworkDescriptionUi = UI.GalleryScene.ArtworkDescription.Get();
            Assert.IsNotNull(artworkDescriptionUi);
            artworkDescriptionUi.SetDescription(artworkInfo);
            artworkDescriptionUi.Show();
        }

        public static Core.MeumDB.ArtworkInfo ArtWorkDataToArtworkInfo(ArtWorkData artWorkData)
        {
            Core.MeumDB.ArtworkInfo artworkInfo = new Core.MeumDB.ArtworkInfo();
            artworkInfo.primaryKey = artWorkData.id;
            artworkInfo.author = artWorkData.author;
            artworkInfo.title = artWorkData.title;
            artworkInfo.size_w = artWorkData.size_w / 100.0f;
            artworkInfo.size_h = artWorkData.size_h / 100.0f;
            artworkInfo.year = artWorkData.year;
            artworkInfo.object_file = artWorkData.object_file;
            artworkInfo.image_file = artWorkData.image_file;
            artworkInfo.thumbnail = artWorkData.thumbnail;
            artworkInfo.instruction = artWorkData.instruction;
            artworkInfo.like = 0;
            artworkInfo.hate = 0;
            artworkInfo.type_artwork = artWorkData.type_artwork;
            return artworkInfo;
        }

    }
}
