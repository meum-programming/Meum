using System.Collections;
using TMPro;
using UI.ChattingUI.TextExtension;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.InputSystem;

namespace UI.GalleryScene
{
    public class ArtworkDescription : Core.Singleton<ArtworkDescription>
    {
        [SerializeField] private TabControl tabControl;
        [SerializeField] private GameObject deleteModal;
        
        [Header("Description")]
        [SerializeField] private TextMeshProUGUI title;
        [SerializeField] private TextMeshProUGUI author;
        [SerializeField] private TextMeshProUGUI year;
        [SerializeField] private NsbpText description;
        [SerializeField] private Button closeBtn;

        [Header("Comments")] 
        [SerializeField] private TMP_InputField inputField;
        [SerializeField] private Transform list;
        [SerializeField] private GameObject commentPrefab;
        
        private Core.MeumDB.ArtworkInfo _artworkInfo;
        private ArtworkDescriptionComment _deletingComment;

        private void Awake()
        {
            base.Awake();
            Init();
        }

        private void Init()
        {
            Assert.IsNotNull(tabControl);
            Assert.IsNotNull(title);
            Assert.IsNotNull(author);
            Assert.IsNotNull(year);
            Assert.IsNotNull(description);
            Assert.IsNotNull(closeBtn);
            Assert.IsNotNull(inputField);
            Assert.IsNotNull(list);
            Assert.IsNotNull(commentPrefab);
            
            closeBtn.onClick.RemoveAllListeners();
            closeBtn.onClick.AddListener(Close);
            
            gameObject.SetActive(false);
        }

        private void Close()
        {
            gameObject.SetActive(false);
            _artworkInfo = null;
        }

        public void SetDescription(Core.MeumDB.ArtworkInfo info)
        {
            _artworkInfo = info;
            title.text = _artworkInfo.title;
            author.text = _artworkInfo.author;
            if (info.year != 0)
                year.text = _artworkInfo.year.ToString();
            else
                year.text = "";
            if (!ReferenceEquals(_artworkInfo.instruction, null))
                description.text = _artworkInfo.instruction;
            else
                description.text = "";
        }

        public void Show()
        {
            gameObject.SetActive(true);
            tabControl.ChangeTab(0);
        }

        public void LoadComments()
        {
            StartCoroutine(LoadCommentsCoroutine());
        }

        private IEnumerator LoadCommentsCoroutine()
        {
            foreach (Transform child in list)
            {
                Destroy(child.gameObject);
            }
            
            Assert.IsNotNull(_artworkInfo);
            var cd = new CoroutineWithData(this, Core.MeumDB.Get().GetComments(_artworkInfo.primaryKey));
            yield return cd.coroutine;
            Assert.IsNotNull(cd.result);
            var commentInfos = cd.result as Core.MeumDB.CommentInfo[];
            Assert.IsNotNull(commentInfos);

            for (var i = 0; i < commentInfos.Length; ++i)
            {
                var comment = Instantiate(commentPrefab, list).GetComponent<ArtworkDescriptionComment>();
                Assert.IsNotNull(comment);
                comment.SetContent(commentInfos[i], this);
            }
        }

        public void PostComment()
        {
            if (inputField.text.Equals(""))
                return;
            StartCoroutine(PostCommentCoroutine());
        }

        private IEnumerator PostCommentCoroutine()
        {
            Assert.IsNotNull(_artworkInfo);
            var cd = new CoroutineWithData(this, 
                Core.MeumDB.Get().PostCommentCreate(_artworkInfo.primaryKey, inputField.text));
            yield return cd.coroutine;
            inputField.text = "";
            StartCoroutine(LoadCommentsCoroutine());
        }

        public void ShowDeleteModal(ArtworkDescriptionComment comment)
        {
            _deletingComment = comment;
            deleteModal.SetActive(true);
        }

        public void CloseModal()
        {
            _deletingComment = null;
            deleteModal.SetActive(false);   
        }

        public void AcceptDelete()
        {
            Assert.IsNotNull(_deletingComment);
            _deletingComment.Delete();
            CloseModal();
        }
        
        public void OnEndEdit()
        {
            if (!EventSystem.current.alreadySelecting)
            {
                PostComment();
                inputField.ActivateInputField();
            }
            else
            {
                inputField.GetComponent<WebGLSupport.WebGLInput>().DeactivateInputField();
            }
        }
    }
}