using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Assertions;
using System.Collections.Generic;

namespace UI.GuestBook
{
    public class GuestBook : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI[] stampCountTexts = new TextMeshProUGUI[4];
        [SerializeField] private Transform contents;
        [SerializeField] private GameObject guestBookContent;
        [SerializeField] private GuestBookWriter writerUi;

        private void Awake()
        {
            Assert.AreEqual(stampCountTexts.Length, 4);
            Assert.IsNotNull(contents);
            Assert.IsNotNull(guestBookContent);
            Assert.IsNotNull(writerUi);
        }

        public void Show()
        {
            gameObject.SetActive(true);
            LoadContents();
        }
        
        public void LoadContents()
        {
            //StartCoroutine(LoadContentsCoroutine());
            //StartCoroutine(LoadStampCountCoroutine());

            StartCoroutine(LoadContentsCoroutine2());
            StartCoroutine(LoadStampCountCoroutine2());
        }

        private IEnumerator LoadContentsCoroutine()
        {
            var roomId = Core.Socket.MeumSocket.Get().GetRoomId();
            var cd = new CoroutineWithData(this, Core.MeumDB.Get().GetGuestBooks(roomId));
            yield return cd.coroutine;
            Assert.IsNotNull(cd.result);
            var output = cd.result as Core.MeumDB.GuestBookInfo[];
            Assert.IsNotNull(output);
            
            for (var i = 0; i < contents.childCount; ++i)
            {
                var child = contents.GetChild(i);
                if(!ReferenceEquals(child, null))
                    Destroy(child.gameObject);
            }
            
            for (var i = 0; i < output.Length; ++i)
            {
                var data = output[i];
                var content = Instantiate(guestBookContent, contents).GetComponent<GuestBookContent>();
                Assert.IsNotNull(content);
                content.Setup(data);
            }
        }
        private IEnumerator LoadContentsCoroutine2()
        {
            var roomId = Core.Socket.MeumSocket.Get().GetRoomId();
            var cd = new CoroutineWithData(this, Core.MeumDB.Get().GetGuestBooks2(roomId));
            yield return cd.coroutine;

            var output = cd.result as Core.MeumDB.GuestBookInfo[];

            for (var i = 0; i < contents.childCount; ++i)
            {
                var child = contents.GetChild(i);
                if (!ReferenceEquals(child, null))
                    Destroy(child.gameObject);
            }

            for (var i = 0; i < output.Length; ++i)
            {
                var data = output[i];
                var content = Instantiate(guestBookContent, contents).GetComponent<GuestBookContent>();
                Assert.IsNotNull(content);
                content.Setup(data);
            }
        }

        private IEnumerator LoadStampCountCoroutine()
        {
            var roomId = Core.Socket.MeumSocket.Get().GetRoomId();
            var cd = new CoroutineWithData(this, Core.MeumDB.Get().GetGuestBookStampCount(roomId));
            yield return cd.coroutine;
            Assert.IsNotNull(cd.result);
            var output = cd.result as Core.MeumDB.GuestBookStampCountInfo;
            Assert.IsNotNull(output);

            stampCountTexts[0].text = output.one.ToString();
            stampCountTexts[1].text = output.two.ToString();
            stampCountTexts[2].text = output.three.ToString();
            stampCountTexts[3].text = output.four.ToString();
        }


        private IEnumerator LoadStampCountCoroutine2()
        {
            var roomId = Core.Socket.MeumSocket.Get().GetRoomId();
            var cd = new CoroutineWithData(this, Core.MeumDB.Get().GetGuestBookStampCount2(roomId));
            yield return cd.coroutine;
            Assert.IsNotNull(cd.result);
            var output = cd.result as Core.MeumDB.GuestBookStampCountInfo;
            Assert.IsNotNull(output);

            stampCountTexts[0].text = output.one.ToString();
            stampCountTexts[1].text = output.two.ToString();
            stampCountTexts[2].text = output.three.ToString();
            stampCountTexts[3].text = output.four.ToString();
        }

        public void Close()
        {
            gameObject.SetActive(false);
        }

        public void ShowWriterUI()
        {
            StartCoroutine(writerUi.ShowCoroutine());
        }
    }
}