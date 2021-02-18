﻿using System.Collections;
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

        private Core.MeumDB.GuestBookInfo _info;

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

        public void Setup(Core.MeumDB.GuestBookInfo info)
        {
            _info = info;
            var type = _info.stamp_type - 1;

            var result = ColorUtility.TryParseHtmlString(_colorByTypes[type], out var bgColor);
            Assert.IsTrue(result);
            background.color = bgColor;

            icon.sprite = iconTextures[type];

            title.text = _titleByTypes[type];
            detail.text = _info.content;
            writer.text = _info.writer.nickname;
            createdAt.text = _info.created_at;

            var meumSocket = Core.Socket.MeumSocket.Get();
            if(_info.writer.primaryKey == meumSocket.GetPlayerPk())
                deleteButton.gameObject.SetActive(true);
            else if(meumSocket.IsInRoomOwn())
                deleteButton.gameObject.SetActive(true);
        }

        public void Delete()
        {
            //StartCoroutine(DeleteCoroutine());
            StartCoroutine(DeleteCoroutine2());
        }

        private IEnumerator DeleteCoroutine()
        {
            yield return StartCoroutine(Core.MeumDB.Get().DeleteGuestBook(_info.pk));
            Destroy(gameObject);
        }

        private IEnumerator DeleteCoroutine2()
        {
            yield return StartCoroutine(Core.MeumDB.Get().DeleteGuestBook2(_info.pk));
            Destroy(gameObject);
        }
    }
}
