using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI.BuilderScene
{
    public class BuilderSceneVerifyModals : MonoBehaviour
    {
        [SerializeField] private GameObject verifySave;
        [SerializeField] private GameObject verifySaveSuccess;
        [SerializeField] private GameObject verifyReset;
        [SerializeField] private GameObject verifyClean;
        [SerializeField] private GameObject verifyExit;

        public bool showingModal { get; private set; } = false;

        private void CheckShowingOther()
        {
            if (showingModal)
            {
                Debug.LogError("cannot showing modal while showing other");
                Application.Quit(-1);
            }
        }

        public void ShowVerfiySaveModal()
        {
            CheckShowingOther();
            verifySave.SetActive(true);
            gameObject.SetActive(true);
            showingModal = true;
        }
        
        public void ShowVerfiySaveSuccessModal()
        {
            CheckShowingOther();
            verifySaveSuccess.SetActive(true);
            gameObject.SetActive(true);
            showingModal = true;
        }

        public void ShowVerfiyResetModal()
        {
            CheckShowingOther();
            verifyReset.SetActive(true);
            gameObject.SetActive(true);
            showingModal = true;
        }

        public void ShowVerfiyCleanModal()
        {
            CheckShowingOther();
            verifyClean.SetActive(true);
            gameObject.SetActive(true);
            showingModal = true;
        }

        public void ShowVerfiyExitModal()
        {
            CheckShowingOther();
            verifyExit.SetActive(true);
            gameObject.SetActive(true);
            showingModal = true;
        }

        public void CloseModal()
        {
            verifySave.SetActive(false);
            verifySaveSuccess.SetActive(false);
            verifyReset.SetActive(false);
            verifyClean.SetActive(false);
            verifyExit.SetActive(false);
            gameObject.SetActive(false);
            showingModal = false;
        }
    }
}
