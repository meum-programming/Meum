using Game.Artwork;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

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
        [SerializeField] private List<RectTransform> btnList = new List<RectTransform>();

        private Transform _selected;

        private void Awake()
        {
            Assert.IsNotNull(artworkPlacer);
            Assert.IsNotNull(cam);
            Assert.IsNotNull(linkModal);
        }

        private void Update()
        {
            var pos = cam.WorldToScreenPoint(_selected.transform.position);
            if (pos.z < 0)
                pos = new Vector3(1e10f, 1e10f, 0);    // invisible
            transform.position = pos;
        }

        public void SetSelected(Transform obj)
        {
            gameObject.SetActive(true);
            _selected = obj;
            transform.position = cam.WorldToScreenPoint(_selected.transform.position);

            for (int i = 0; i < btnList.Count; i++)
            {
                bool activeOn = true;

                if (i > 1 && _selected.name == "Startpoint")
                {
                    activeOn = false;
                }

                btnList[i].gameObject.SetActive(activeOn);
            }

        }

        public void OnMoveButton()
        {
            artworkPlacer.StartMovingObj();
            
            gameObject.SetActive(false);
            _selected = null;
        }

        public void OnRotateButton()
        {
            artworkPlacer.RotateSelected();
        }

        public void OnLinkButton()
        {
            var selected = artworkPlacer.GetSelected();
            Assert.IsNotNull(selected);
            var artworkInfo = selected.GetComponent<ArtworkInfo>();
            Assert.IsNotNull(artworkInfo);
            
            linkModal.Show(artworkInfo);
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
    }
}
