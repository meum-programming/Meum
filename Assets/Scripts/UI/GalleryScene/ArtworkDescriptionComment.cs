using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace UI.GalleryScene
{
    public class ArtworkDescriptionComment : MonoBehaviour
    {
        [SerializeField] private Image badgeImage;
        [SerializeField] private Button deleteButton;
        [SerializeField] private TextMeshProUGUI writer;
        [SerializeField] private TextMeshProUGUI content;
        [SerializeField] private float heightWithoutContent;
 
        private RectTransform _transform;
        private Core.MeumDB.CommentInfo _commentInfo;
        private ArtworkDescription _descriptionUi;

        private void Awake()
        {
            _transform = GetComponent<RectTransform>();
        }

        public void SetContent(Core.MeumDB.CommentInfo info, ArtworkDescription descriptionUi)
        {
            _commentInfo = info;
            _descriptionUi = descriptionUi;
            
            writer.text = _commentInfo.writer.nickname;
            content.text = _commentInfo.content;
            
            var sizeDelta = _transform.sizeDelta;
            sizeDelta.y = content.preferredHeight + heightWithoutContent;
            _transform.sizeDelta = sizeDelta;

            var meumSocket = Core.Socket.MeumSocket.Get();
            if (_commentInfo.writer.primaryKey == meumSocket.GetPlayerPk())
            {
                deleteButton.gameObject.SetActive(true);
                deleteButton.onClick.AddListener(Delete);
            }
            else
            {
                deleteButton.gameObject.SetActive(false);
            }
        }

        private void Delete()
        {
            StartCoroutine(DeleteCoroutine());
        }

        private IEnumerator DeleteCoroutine()
        {
            Assert.IsNotNull(_descriptionUi);
            yield return Core.MeumDB.Get().DeleteComment(_commentInfo.pk);
            _descriptionUi.LoadComments();
        }
    }
}
