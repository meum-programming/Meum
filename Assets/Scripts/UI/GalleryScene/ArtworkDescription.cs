using System.Collections;
using TMPro;
using UI.ChattingUI.TextExtension;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;
using Core;
using DG.Tweening;

namespace UI.GalleryScene
{
    public class ArtworkDescription : Core.Singleton<ArtworkDescription>
    {
        [SerializeField] private TabControl tabControl;
        [SerializeField] private VerticalLayoutGroup descriptionPanel;
        [SerializeField] private GameObject deleteModal;

        float originWidth = 828;
        float originHeight = 695;

        [Header("Description")]
        [SerializeField] private RawImage image;
        [SerializeField] private Text title;
        [SerializeField] private Text description;
        [SerializeField] private Button closeBtn;

        [Header("Comments")] 
        [SerializeField] private TextMeshProUGUI title_comments_tab;
        [SerializeField] private InputField inputField;
        [SerializeField] private Transform list;
        [SerializeField] private GameObject commentPrefab;
        
        private ArtWorkData _artworkInfo;
        private ArtworkDescriptionComment _deletingComment;

        int tabState = 0;
        [SerializeField] private List<Image> tabBtnList = new List<Image>();
        [SerializeField] private List<RectTransform> tabObjList = new List<RectTransform>();
        

        private void Awake()
        {
            base.Awake();
            Init();
        }

        private void Init()
        {
            //closeBtn.onClick.RemoveAllListeners();
            //closeBtn.onClick.AddListener(Close);
            
            gameObject.SetActive(false);

            Vector2 sizeDelta = image.rectTransform.sizeDelta;
            originWidth = sizeDelta.x;
            originHeight = sizeDelta.y;

            SetTab();
        }

        public void Close()
        {
            gameObject.SetActive(false);
            _artworkInfo = null;
        }

        public void SetDescription(ArtWorkData info)
        {
            _artworkInfo = info;
            title.text = _artworkInfo.title;

            if (!ReferenceEquals(_artworkInfo.instruction, null))
                description.text = _artworkInfo.instruction;
            else
                description.text = "";

            StartCoroutine(SetActive());
        }

        IEnumerator SetActive()
        {
            image.gameObject.SetActive(false);

            yield return new WaitForSeconds(0.01f);

            descriptionPanel.SetLayoutVertical();

            descriptionPanel.gameObject.SetActive(false);
            descriptionPanel.gameObject.SetActive(true);

            string url = _artworkInfo.image_file;

            if (url == string.Empty)
            {
                url = _artworkInfo.thumbnail;
            }

            var textureGetter = MeumDB.Get().GetTextureCoroutine(url);
            yield return textureGetter.coroutine;
            
            var texture = textureGetter.result as Texture2D;

            image.gameObject.SetActive(true);

            image.texture = texture;

            float tempWidth = originWidth;
            float tempHeight = originHeight;

            if (texture.height > texture.width || texture.height == texture.width)
            {
                tempWidth = ((texture.width * originHeight) / texture.height);  
            }
            else
            {
                tempHeight = ((texture.height * originWidth) / texture.width);
            }

            image.rectTransform.sizeDelta = new Vector2(tempWidth, tempHeight);

        }

        public void Show()
        {
            gameObject.SetActive(true);
            TabBtnClick(0);
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

        public void TabBtnClick(int index)
        {
            tabState = index;
            SetTabBtnActive();
            SetTab();
        }

        void SetTabBtnActive() 
        {
            for (int i = 0; i < tabBtnList.Count; i++)
            {
                bool activeOn = i == tabState;

                Color cor = tabBtnList[i].color;
                cor.a = activeOn ? 1 : 0;
                tabBtnList[i].color = cor;

                Color contentCor = activeOn ? new Color(1, 1, 1) : new Color(0.8f, 0.8f, 0.8f);

                tabBtnList[i].transform.Find("Text").GetComponent<Text>().color = contentCor;
                tabBtnList[i].transform.Find("Icon").GetComponent<Image>().color = contentCor;
            }
        }

        void SetTab()
        {
            for (int i = 0; i < tabObjList.Count; i++)
            {
                tabObjList[i].gameObject.SetActive(i == tabState);
            }

            if (tabState == 0)
            {
                description.rectTransform.DOAnchorPosY(0, 0);
            }
            else if (tabState == 1)
            {
                StartCoroutine(LoadCommentsCoroutine2());
                inputField.text = string.Empty;
            }
        }
    }
}