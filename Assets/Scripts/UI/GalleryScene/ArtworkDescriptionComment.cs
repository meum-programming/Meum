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
        private ArtWorkCommentData _commentInfo;
        private ArtworkDescription _descriptionUi;

        private void Awake()
        {
            _transform = GetComponent<RectTransform>();
        }

        public void SetContent(ArtWorkCommentData info, ArtworkDescription descriptionUi)
        {
            _commentInfo = info;
            _descriptionUi = descriptionUi;
            
            writer.text = _commentInfo.owner.nickname;
            content.text = _commentInfo.content;
            
            var sizeDelta = _transform.sizeDelta;
            sizeDelta.y = content.preferredHeight + heightWithoutContent;
            _transform.sizeDelta = sizeDelta;

            var meumSocket = Core.Socket.MeumSocket.Get();
            if (_commentInfo.owner.user_id == meumSocket.GetPlayerPk())
            {
                deleteButton.gameObject.SetActive(true);
                deleteButton.onClick.AddListener(ShowDeleteModal);
            }
            else
            {
                deleteButton.gameObject.SetActive(false);
            }
        }

        private void ShowDeleteModal()
        {
            Assert.IsNotNull(_descriptionUi);
            _descriptionUi.ShowDeleteModal(this);
        }

        public void Delete()
        {
            StartCoroutine(DeleteCoroutine2());
        }

        private IEnumerator DeleteCoroutine2()
        {
            Assert.IsNotNull(_descriptionUi);

            bool nextOn = false;

            ArtWorkRequest artWorkRequest = new ArtWorkRequest()
            {
                requestStatus = 5,
                id = _commentInfo.id,
                successOn = ResultData =>
                {
                    nextOn = true;
                }
            };
            artWorkRequest.RequestOn();

            yield return new WaitUntil(() => nextOn);

            _descriptionUi.LoadComments();
        }
    }
}
