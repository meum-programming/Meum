using System;
using Core;
using UI.ChattingUI.TextExtension;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace UI
{
    public class ArtworkDescription : Core.Singleton<ArtworkDescription>
    {
        [SerializeField] private Text title;
        [SerializeField] private Text author;
        [SerializeField] private Text year;
        [SerializeField] private Text size;
        [SerializeField] private Text material;
        [SerializeField] private NsbpText description;
        [SerializeField] private Button closeBtn;

        private void Awake()
        {
            base.Awake();
            Init();
            Close();
        }

        private void Init()
        {
            Assert.IsNotNull(title);
            Assert.IsNotNull(author);
            Assert.IsNotNull(year);
            Assert.IsNotNull(size);
            Assert.IsNotNull(material);
            Assert.IsNotNull(description);
            Assert.IsNotNull(closeBtn);
            
            closeBtn.onClick.RemoveAllListeners();
            closeBtn.onClick.AddListener(Close);
        }

        private void Close()
        {
            gameObject.SetActive(false);
        }

        public void SetDescription(MeumDB.ArtworkInfo info)
        {
            title.text = info.title;
            author.text = info.author;
            if (info.year != 0)
                year.text = info.year.ToString();
            else
                year.text = "";
            size.text = String.Format("{0}cm x {1}cm", Mathf.RoundToInt(info.size_w*100), Mathf.RoundToInt(info.size_h*100));
            if (!ReferenceEquals(info.material, null))
                material.text = info.material;
            else
                material.text = "";
            if (!ReferenceEquals(info.instruction, null))
                description.text = info.instruction;
            else
                description.text = "";
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }
    }
}