using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

namespace UI.GalleryScene
{
    [RequireComponent(typeof(Button))]
    public class TutorialButton : MonoBehaviour
    {
        [DllImport("__Internal")]
        private static extern void OpenURLNewTab(string url);

        private void Awake()
        {
            GetComponent<Button>().onClick.AddListener(ButtonAction);
        }

        private void ButtonAction()
        {
#if UNITY_WEBGL

            string uri = "https://meum.me/tutorial";
    #if dev
            uri = "https://dev.meum.me/tutorial";
    #endif
            OpenURLNewTab(uri);

#endif
        }
    }
}