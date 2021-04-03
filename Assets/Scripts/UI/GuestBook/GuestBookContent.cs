using System;
using System.Collections;
using Core;
using Core.Socket;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace UI.GuestBook
{
    public class GuestBookContent : MonoBehaviour
    {
        private readonly string[] _colorByTypes =
        {
            "#EB5757FF",
            "#FFDA79FF",
            "#CEA7FFFF",
            "#79AFFFFF"
        };

        private readonly string[] _titleByTypes =
        {
            "좋아요!",
            "즐거워요!",
            "놀라워요!",
            "기대되요!"
        };

        [SerializeField] private Sprite[] iconTextures = new Sprite[4];
        [SerializeField] private Image background;
        [SerializeField] private Image icon;
        [SerializeField] private TextMeshProUGUI title;
        [SerializeField] private TextMeshProUGUI detail;
        [SerializeField] private TextMeshProUGUI writer;
        [SerializeField] private TextMeshProUGUI createdAt;
        [SerializeField] private Button deleteButton;

        public GuestBooksData _info;

        private void Awake()
        {
            Assert.AreEqual(iconTextures.Length, 4);
            Assert.IsNotNull(background);
            Assert.IsNotNull(icon);
            Assert.IsNotNull(title);
            Assert.IsNotNull(detail);
            Assert.IsNotNull(writer);
            Assert.IsNotNull(createdAt);
        }

        public void Setup(GuestBooksData info)
        {
            _info = info;
            var type = _info.stamp_type - 1;

            var result = ColorUtility.TryParseHtmlString(_colorByTypes[type], out var bgColor);
            Assert.IsTrue(result);
            background.color = bgColor;

            icon.sprite = iconTextures[type];

            title.text = _titleByTypes[type];
            detail.text = _info.content;
            writer.text = _info.owner.nickname;
            createdAt.text = GetDataTime(_info.created_at).ToString();

            var meumSocket = Core.Socket.MeumSocket.Get();

            bool isOwnerRoom = MeumDB.Get().myRoomInfo.owner.user_id == MeumDB.Get().currentRoomInfo.owner.user_id;
            bool isOwnerData = _info.owner.user_id == meumSocket.GetPlayerPk();

            deleteButton.gameObject.SetActive(isOwnerRoom || isOwnerData);
        }

        public static DateTime GetDataTime(string data)
        {
            DateTime dateTime = DateTime.Now;

            if (data != string.Empty)
            {
                string dateStr = data.Split('T')[0];
                string timeStr = data.Split('T')[1].Split('.')[0];

                string[] dateData = dateStr.Split('-');
                int year = int.Parse(dateData[0]);
                int month = int.Parse(dateData[1]);
                int day = int.Parse(dateData[2]);

                string[] timeData = timeStr.Split(':');
                int hour = int.Parse(timeData[0]);
                int min = int.Parse(timeData[1]);
                int sec = int.Parse(timeData[2]);

                dateTime = new DateTime(year, month, day, hour, min, sec);
            }

            return dateTime.AddHours(9);
        }


        public void Delete()
        {
            transform.GetComponentInParent<GuestBook>().DeletePopupOpen(this);

            //StartCoroutine(DeleteCoroutine2());
        }

        private IEnumerator DeleteCoroutine2()
        {
            bool nextOn = false;

            GuestBooksRequest guestBooksRequest = new GuestBooksRequest()
            {
                requestStatus = 3,
                id = _info.id,
                successOn = ResultData =>
                {
                    nextOn = true;
                }
            };
            guestBooksRequest.RequestOn();

            yield return new WaitUntil(() => nextOn);

            Destroy(gameObject);
        }
    }
}
