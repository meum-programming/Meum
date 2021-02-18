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
            StartCoroutine(LoadContentsCoroutine2());
            StartCoroutine(LoadStampCountCoroutine2());
        }

        private IEnumerator LoadContentsCoroutine2()
        {
            var roomId = Core.Socket.MeumSocket.Get().GetRoomId();

            bool nextOn = false;

            List<GuestBooksData> guestBooksDataList = new List<GuestBooksData>();

            GuestBooksRequest guestBooksRequest = new GuestBooksRequest()
            {
                requestStatus = 0,
                roomId = roomId,
                successOn = ResultData =>
                {
                    guestBooksDataList = ((GuestBooksRespons)ResultData).result;
                    nextOn = true;
                }
            };
            guestBooksRequest.RequestOn();

            yield return new WaitUntil(() => nextOn);


            for (var i = 0; i < contents.childCount; ++i)
            {
                var child = contents.GetChild(i);
                if (!ReferenceEquals(child, null))
                    Destroy(child.gameObject);
            }

            for (var i = 0; i < guestBooksDataList.Count; ++i)
            {
                var content = Instantiate(guestBookContent, contents).GetComponent<GuestBookContent>();
                Assert.IsNotNull(content);
                content.Setup(guestBooksDataList[i]);
            }
        }

        private IEnumerator LoadStampCountCoroutine2()
        {
            var roomId = Core.Socket.MeumSocket.Get().GetRoomId();

            bool nextOn = false;

            GuestBooksStempData guestBooksStempData = null;

            GuestBooksRequest guestBooksRequest = new GuestBooksRequest()
            {
                requestStatus = 1,
                roomId = roomId,
                successOn = ResultData =>
                {
                    guestBooksStempData = ((GuestBooksStempRespons)ResultData).result;
                    nextOn = true;
                }
            };
            guestBooksRequest.RequestOn();

            yield return new WaitUntil(() => nextOn);

            stampCountTexts[0].text = guestBooksStempData.one_queryset.ToString();
            stampCountTexts[1].text = guestBooksStempData.two_queryset.ToString();
            stampCountTexts[2].text = guestBooksStempData.three_queryset.ToString();
            stampCountTexts[3].text = guestBooksStempData.four_queryset.ToString();
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