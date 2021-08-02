using Game.Artwork;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI.BuilderScene
{
    /*
     * @brief Build scene에서 Artwork를 클릭할 시에 나타나는 action selector (move, delete) 담당 컴포넌트
     */
    public class PlacerActionSelector : MonoBehaviour
    {
        [SerializeField] private ArtworkPlacer artworkPlacer;
        [SerializeField] private Camera cam;
        [SerializeField] private LinkModal linkModal;
        [SerializeField] private List<Button> btnList = new List<Button>();

        [SerializeField] private List<Sprite> normalIconSpriteList = new List<Sprite>();
        [SerializeField] private List<Sprite> expertIconSpriteList = new List<Sprite>();

        [SerializeField] private RectTransform easeScaleSetBtnPanel;

        private Transform _selected;

        int selectState = -1;

        [SerializeField] private RectTransform selectOnIcon;

        bool expertModeOn = false;

        [SerializeField] private RectTransform expertModePanel;

        [SerializeField] private RectTransform expertMovePanel;
        [SerializeField] private RectTransform expertRotPanel;
        [SerializeField] private RectTransform expertScalePanel;

        public bool dragOn = false;

        public Arrow.ArrowEnum arrowEnum = Arrow.ArrowEnum.None;

        private void Awake()
        {
            Assert.IsNotNull(artworkPlacer);
            Assert.IsNotNull(cam);
            Assert.IsNotNull(linkModal);
            EventSet();
            easeScaleSetBtnPanel.gameObject.SetActive(false);
            selectOnIcon.gameObject.SetActive(false);
            expertModePanel.gameObject.SetActive(false);
        }

        void EventSet()
        {
            for (int i = 0; i < btnList.Count; i++)
            {
                int index = i;
                btnList[i].onClick.AddListener(() => SelectorBtnClick(index));
            }
        }

        void DragEventOn(Arrow.ArrowEnum arrowEnum, Vector2 delta)
        {
            switch (selectState) 
            {
                case 0: ExportMovePosSet(arrowEnum, delta); break;
                case 1: ExportRotationSet(arrowEnum, delta); break;
                case 2: ExportScaleSet(arrowEnum, delta); break;
            }
        }

        void ExportMovePosSet(Arrow.ArrowEnum arrowEnum, Vector2 delta)
        {
            Vector3 pos = _selected.transform.position;

            if (arrowEnum == Arrow.ArrowEnum.X)
            {
                pos.x += delta.x * 0.1f;
            }
            else if (arrowEnum == Arrow.ArrowEnum.Y)
            {
                pos.y += delta.y * 0.1f;
            }
            else if (arrowEnum == Arrow.ArrowEnum.Z)
            {
                pos.z -= delta.x * 0.1f;
            }

            _selected.transform.position = pos;
            expertModePanel.transform.position = _selected.transform.position;
        }

        void ExportRotationSet(Arrow.ArrowEnum arrowEnum, Vector2 delta)
        {
            Vector3 eulerAngles = _selected.transform.rotation.eulerAngles;

            if (arrowEnum == Arrow.ArrowEnum.X)
            {
                eulerAngles.x += delta.x;
            }
            else if (arrowEnum == Arrow.ArrowEnum.Y)
            {
                eulerAngles.y += delta.y;
            }
            else if (arrowEnum == Arrow.ArrowEnum.Z)
            {
                eulerAngles.z -= delta.x;
            }

            _selected.transform.rotation = Quaternion.Euler(eulerAngles);
        }

        void ExportScaleSet(Arrow.ArrowEnum arrowEnum, Vector2 delta)
        {
            Vector3 scaleValue = _selected.transform.localScale;

            if (arrowEnum == Arrow.ArrowEnum.X)
            {
                scaleValue.x += delta.x;
            }
            else if (arrowEnum == Arrow.ArrowEnum.Y)
            {
                scaleValue.y += delta.y;
            }
            else if (arrowEnum == Arrow.ArrowEnum.Z)
            {
                scaleValue.z -= delta.x;
            }

            _selected.transform.localScale = scaleValue;
        }


        private void Update()
        {
            SelectOnIconPosReset();

            if (expertModeOn)
            {
                RayTest();
                DragCheck();
            }
        }

        void RayTest()
        {
            var ray = cam.ScreenPointToRay(Input.mousePosition);

            foreach (var hit in Physics.RaycastAll(ray))
            {
                if (hit.transform.gameObject.layer == LayerMask.NameToLayer("UI"))
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        if (hit.transform.gameObject.GetComponent<Arrow>())
                        {
                            dragOn = true;
                            arrowEnum = hit.transform.gameObject.GetComponent<Arrow>().arrowEnum;
                        }
                    }
                    else if (Input.GetMouseButtonUp(0))
                    {
                        arrowEnum = Arrow.ArrowEnum.None;
                    }
                }
            }
        }

        void DragCheck()
        {
            if (dragOn == false || !Input.GetMouseButton(0) || arrowEnum == Arrow.ArrowEnum.None)
                return;
            
            Vector2 delta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
            DragEventOn(arrowEnum, delta);
            
        }

        void SelectOnIconPosReset()
        {
            if (_selected == null)
                return;

            selectOnIcon.gameObject.SetActive(!expertModeOn);

            if (expertModeOn)
                return;
            
            var pos = cam.WorldToScreenPoint(_selected.transform.position);
            if (pos.z < 0)
                pos = new Vector3(1e10f, 1e10f, 0);    // invisible
            
            pos.y += 100;
            selectOnIcon.transform.position = pos;
        }
        
        public void SetSelected(Transform obj)
        {
            gameObject.SetActive(true);
            _selected = obj;
            //transform.position = cam.WorldToScreenPoint(_selected.transform.position);

            for (int i = 0; i < btnList.Count; i++)
            {
                bool activeOn = true;

                if (i > 1 && _selected.name == "Startpoint")
                {
                    activeOn = false;
                }

                btnList[i].gameObject.SetActive(activeOn);
            }

            //expertModePanel.transform.SetParent(_selected);
            expertModePanel.transform.position = _selected.transform.position;

            //expertModePanel.transform.position = cam.WorldToScreenPoint(_selected.transform.position);
            SelectOnIconPosReset();
        }

        public void SelectorBtnClick(int status)
        {
            selectState = status;

            switch (selectState)
            {
                case 0: 
                    OnMoveButton();
                    break;
                case 1:
                    OnRotateButton();
                    break;
                case 2:
                    break;
                case 3:
                    OnLinkButton();
                    break;
                case 4:
                    OnDeleteButton();
                    break;
            }

            easeScaleSetBtnPanel.gameObject.SetActive(expertModeOn == false && selectState == 2);
            expertModePanel.gameObject.SetActive(expertModeOn && selectState <= 2);

            expertMovePanel.gameObject.SetActive(expertModeOn && selectState == 0);
            expertRotPanel.gameObject.SetActive(expertModeOn && selectState == 1);
            expertScalePanel.gameObject.SetActive(expertModeOn && selectState == 2);
        }

        public void OnMoveButton()
        {
            if (expertModeOn)
                return;

            artworkPlacer.StartMovingObj();
            
            gameObject.SetActive(false);
            _selected = null;
        }

        public void OnRotateButton()
        {
            if (expertModeOn)
                return;

            artworkPlacer.RotateSelected();
        }

        public void OnScaleSetOn(bool scaleUp)
        {
            if (expertModeOn)
                return;

            float value = scaleUp ? 0.1f : -0.1f;
            artworkPlacer.SetResize(value);
        }

        public void OnLinkButton()
        {
            var selected = artworkPlacer.GetSelected();
            Assert.IsNotNull(selected);
            var artworkInfo = selected.GetComponentInParent<ArtworkInfo>();

            if (artworkInfo != null)
            {
                linkModal.Show(artworkInfo);
            }
        }

        public void OnDeleteButton()
        {
            artworkPlacer.DeleteSelected();
            
            gameObject.SetActive(false);
            _selected = null;
        }

        public void Deselect()
        {
            gameObject.SetActive(false);
            _selected = null;
        }

        public void ExpertModeOnToggleClick(Toggle toggle)
        {
            expertModeOn = toggle.isOn;

            if (_selected != null && selectState == -1)
            {
                selectState = 0;
            }

            SelectorBtnClick(selectState);
            ExpertModeChange();
        }

        void ExpertModeChange()
        {
            for (int i = 0; i < 3; i++)
            {
                Sprite sprite = expertModeOn ? expertIconSpriteList[i] : normalIconSpriteList[i];
                btnList[i].transform.Find("Icon").GetComponent<Image>().sprite = sprite;
            }
        }

        public void ExpertModeXMoveOn(BaseEventData eventData)
        {
            PointerEventData peData = (PointerEventData)eventData;

            if (_selected == null)
                return;

            Vector3 pos = _selected.transform.position;
            pos.x -= peData.delta.x * 0.0045f;
            _selected.transform.position = pos;

            expertModePanel.transform.position = cam.WorldToScreenPoint(_selected.transform.position);


            //Vector3 pos = expertModePanel.transform.position;
            //pos.x -= peData.delta.x;
            //expertModePanel.transform.position = pos;

            //_selected.transform.position = cam.ScreenToWorldPoint(expertModePanel.transform.position);

        }
        public void ExpertModeYMoveOn(BaseEventData eventData)
        {
            PointerEventData peData = (PointerEventData)eventData;

            if (_selected == null)
                return;

            Vector3 pos = _selected.transform.position;
            pos.y += peData.delta.y * 0.0045f;
            _selected.transform.position = pos;

            expertModePanel.transform.position = cam.WorldToScreenPoint(_selected.transform.position);

            //Vector3 pos = expertModePanel.transform.position;
            //pos.y += peData.delta.y;
            //expertModePanel.transform.position = pos;

            //_selected.transform.position = cam.ScreenToWorldPoint(expertModePanel.transform.position);
        }

        public void ExpertModeZMoveOn(BaseEventData eventData)
        {
            PointerEventData peData = (PointerEventData)eventData;

            if (_selected == null)
                return;

            Vector3 pos = _selected.transform.position;
            pos.z += peData.delta.x * 0.0045f;
            _selected.transform.position = pos;

            expertModePanel.transform.position = cam.WorldToScreenPoint(_selected.transform.position);

            //Vector3 pos = expertModePanel.transform.position;
            //pos.z += peData.delta.x;
            //expertModePanel.transform.position = pos;

            //_selected.transform.position = cam.ScreenToWorldPoint(expertModePanel.transform.position);

        }

        
    }
}
