using UnityEngine;
using UnityEngine.Assertions;

namespace UI.BuilderScene
{
    /*
     * @brief 저장, 저장성공, 리셋, 클린, 나가기 버튼을 누를 시 나오는 모달을 담당하는 컴포넌트
     */
    public class BuilderSceneVerifyModals : MonoBehaviour
    {
        [SerializeField] private GameObject verifySave;
        [SerializeField] private GameObject verifySaveSuccess;
        [SerializeField] private GameObject verifyReset;
        [SerializeField] private GameObject verifyClean;
        [SerializeField] private GameObject verifyExit;

        public bool showingModal { get; private set; } = false;
        
        public void ShowVerfiySaveModal()
        {
            Assert.IsFalse(showingModal);
            verifySave.SetActive(true);
            gameObject.SetActive(true);
            showingModal = true;
        }
        
        public void ShowVerfiySaveSuccessModal()
        {
            Assert.IsFalse(showingModal);
            verifySaveSuccess.SetActive(true);
            gameObject.SetActive(true);
            showingModal = true;

            DataManager.Instance.roomSaveOn = true;
        }

        public void ShowVerfiyResetModal()
        {
            Assert.IsFalse(showingModal);
            verifyReset.SetActive(true);
            gameObject.SetActive(true);
            showingModal = true;
        }

        public void ShowVerfiyCleanModal()
        {
            Assert.IsFalse(showingModal);
            verifyClean.SetActive(true);
            gameObject.SetActive(true);
            showingModal = true;
        }

        public void ShowVerfiyExitModal()
        {
            Assert.IsFalse(showingModal);
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
