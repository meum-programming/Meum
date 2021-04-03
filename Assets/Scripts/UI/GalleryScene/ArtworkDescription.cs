using System.Collections;
using TMPro;
using UI.ChattingUI.TextExtension;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections.Generic;

namespace UI.GalleryScene
{
    public class ArtworkDescription : Core.Singleton<ArtworkDescription>
    {
        [SerializeField] private TabControl tabControl;
        [SerializeField] private VerticalLayoutGroup descriptionPanel;
        [SerializeField] private GameObject deleteModal;
        
        [Header("Description")]
        [SerializeField] private TextMeshProUGUI title;
        [SerializeField] private TextMeshProUGUI author;
        [SerializeField] private TextMeshProUGUI year;
        [SerializeField] private NsbpText description;
        [SerializeField] private Button closeBtn;

        [Header("Comments")] 
        [SerializeField] private TextMeshProUGUI title_comments_tab;
        [SerializeField] private TMP_InputField inputField;
        [SerializeField] private Transform list;
        [SerializeField] private GameObject commentPrefab;
        
        private ArtWorkData _artworkInfo;
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
            Assert.IsNotNull(title_comments_tab);
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

        public void SetDescription(ArtWorkData info)
        {
            _artworkInfo = info;
            title.text = _artworkInfo.title;
            title_comments_tab.text = _artworkInfo.title;
            author.text = _artworkInfo.author;

            if (info.year != 0)
                year.text = _artworkInfo.year.ToString();
            else
                year.text = "";
            if (!ReferenceEquals(_artworkInfo.instruction, null))
                description.text = _artworkInfo.instruction;
            else
                description.text = "";

            StartCoroutine(SetActive());
        }

        IEnumerator SetActive()
        {
            yield return new WaitForSeconds(0.01f);

            descriptionPanel.SetLayoutVertical();

            descriptionPanel.gameObject.SetActive(false);
            descriptionPanel.gameObject.SetActive(true);
        }

        public void Show()
        {
            gameObject.SetActive(true);
            tabControl.ChangeTab(0);
        }

        public void LoadComments()
        {
            StartCoroutine(LoadCommentsCoroutine2());
        }

        private IEnumerator LoadCommentsCoroutine2()
        {
            foreach (Transform child in list)
            {
                Destroy(child.gameObject);
            }

            bool nextOn = false;

            List<ArtWorkCommentData> resultList = new List<ArtWorkCommentData>();

            ArtWorkRequest artWorkRequest = new ArtWorkRequest()
            {
                requestStatus = 3,
                id = _artworkInfo.id,
                successOn = ResultData =>
                {
                    resultList = ((ArtWorkCommentRespons)ResultData).result;
                    nextOn = true;
                }
            };
            artWorkRequest.RequestOn();

            yield return new WaitUntil(() => nextOn);

            for (var i = 0; i < resultList.Count; ++i)
            {
                var comment = Instantiate(commentPrefab, list).GetComponent<ArtworkDescriptionComment>();
                comment.SetContent(resultList[i], this);
            }
        }

        public void PostComment()
        {
            if (inputField.text.Equals(""))
                return;
            StartCoroutine(PostCommentCoroutine2());
        }

        private IEnumerator PostCommentCoroutine2()
        {
            bool nextOn = false;

            ArtWorkRequest artWorkRequest = new ArtWorkRequest()
            {
                requestStatus = 4,
                id = _artworkInfo.id,
                content = inputField.text,
                uid = Core.MeumDB.Get().GetToken(),
                successOn = ResultData =>
                {
                    nextOn = true;
                }
            };
            artWorkRequest.RequestOn();

            yield return new WaitUntil(() => nextOn);

            inputField.text = "";
            StartCoroutine(LoadCommentsCoroutine2());
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