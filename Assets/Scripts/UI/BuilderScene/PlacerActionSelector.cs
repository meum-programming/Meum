using System;
using System.Collections;
using System.Collections.Generic;
using Gallery.Builder;
using UnityEngine;

namespace UI.BuilderScene
{
    public class PlacerActionSelector : MonoBehaviour
    {
        [SerializeField] private ArtworkPlacer artworkPlacer;
        [SerializeField] private Camera cam;

        private Transform _selected;

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
        }

        public void OnMoveButton()
        {
            artworkPlacer.StartMovingObj();
            
            gameObject.SetActive(false);
            _selected = null;
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
