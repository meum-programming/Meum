using System.Collections;
using Core;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace UI.GuestBook
{
    public class GuestBookWriter : MonoBehaviour
    {
        [SerializeField] private Color activatedColor;
        [SerializeField] private Color deactivatedColor;
        [SerializeField] private Button[] stampButtons = new Button[4];
        [SerializeField] private TextMeshProUGUI[] stampButtonTexts = new TextMeshProUGUI[4];
        [SerializeField] private TMP_InputField contentInputField;
        [SerializeField] private GuestBook guestBook;

        [Header("Contents Toggle")] 
        [SerializeField] private RectTransform contents;
        [SerializeField] private GameObject showButton;
        [SerializeField] private Vector3 contentsHidePosition;
        [SerializeField] private float toggleTime;

        private Vector3 _contentsOriginalPosition;
        private int _currentStampIdx;

        private void Awake()
        {
            Assert.AreEqual(stampButtons.Length, 4);
            Assert.AreEqual(stampButtonTexts.Length, 4);
            Assert.IsNotNull(contentInputField);
            Assert.IsNotNull(guestBook);
            Assert.IsNotNull(contents);
            Assert.IsNotNull(showButton);

            _contentsOriginalPosition = contents.anchoredPosition;

            for (var i = 0; i < stampButtons.Length; ++i)
            {
                var temp = i;
                stampButtons[i].onClick.AddListener(() => { ActivateStampButton(temp); });
            }

            ActivateStampButton(0);

            gameObject.SetActive(false);
        }

        private void ActivateStampButton(int idx)
        {
            _currentStampIdx = idx + 1;

            for (var i = 0; i < stampButtons.Length; ++i)
            {
                var color = deactivatedColor;
                if (i == idx)
                    color = activatedColor;

                stampButtons[i].image.color = color;
                stampButtonTexts[i].color = color;
            }
        }

        public void Submit()
        {
            StartCoroutine(SubmitCoroutine2());
        }

        private IEnumerator SubmitCoroutine2()
        {
            var roomId = Core.Socket.MeumSocket.Get().GetRoomId();

            bool nextOn = false;

            GuestBooksRequest guestBooksRequest = new GuestBooksRequest()
            {
                requestStatus = 2,
                roomId = roomId,
                stamp_type = _currentStampIdx,
                content = contentInputField.text,
                writer_id = MeumDB.Get().GetToken(),
                successOn = ResultData =>
                {
                    nextOn = true;
                }
            };
            guestBooksRequest.RequestOn();

            yield return new WaitUntil(() => nextOn);

            Hide();
            guestBook.LoadContents();
        }

        public IEnumerator ShowCoroutine()
        {
            var t = 0.0f;
            while (t < toggleTime)
            {
                contents.anchoredPosition =
                    Vector3.Lerp(_contentsOriginalPosition, contentsHidePosition, t / toggleTime);
                t += Time.deltaTime;
                yield return null;
            }

            contents.anchoredPosition = contentsHidePosition;
            gameObject.SetActive(true);
            showButton.SetActive(false);
        }

        public void Hide()
        {
            StartCoroutine(HideCoroutine());
        }
        
        private IEnumerator HideCoroutine()
        {
            var t = 0.0f;
            while (t < toggleTime)
            {
                contents.anchoredPosition =
                    Vector3.Lerp(contentsHidePosition, _contentsOriginalPosition, t / toggleTime);
                t += Time.deltaTime;
                yield return null;
            }

            contents.anchoredPosition = _contentsOriginalPosition;
            gameObject.SetActive(false);
            showButton.SetActive(true);
        }
    }
}