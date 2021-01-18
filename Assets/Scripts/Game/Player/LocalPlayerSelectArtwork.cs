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

        public void OnSelect(InputAction.CallbackContext ctx)
        {
            if (!ctx.performed) return;

            var ray = camera.ScreenPointToRay(Mouse.current.position.ReadValue());
            var layerMask = ~0;
            if (Physics.Raycast(ray, out var hit, 100.0f, layerMask))
            {
                if (hit.transform.CompareTag("Paint"))
                {
                    var artworkInfo = hit.transform.GetComponent<Artwork.ArtworkInfo>();
#if UNITY_WEBGL
                    if (artworkInfo.BannerUrl != "")
                    {
                        Debug.Log(artworkInfo.BannerUrl);
                        OpenURLNewTab(artworkInfo.BannerUrl);
                    }
#endif
                    if (artworkInfo.BannerUrl == "")
                    {
                        StartCoroutine(SetArtworkDescription(artworkInfo));
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
    }
}
