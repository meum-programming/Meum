using UnityEngine;

namespace UI
{
    public class ButtonDescription : MonoBehaviour
    {
        [SerializeField] private GameObject descriptionObj;

        private void Awake()
        {
            descriptionObj.SetActive(false);
        }

        public void Show()
        {
            descriptionObj.SetActive(true);
        }

        public void Hide()
        {
            descriptionObj.SetActive(false);
        }
    }
}