using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.ChattingUI
{
    public class ChattingLog : MonoBehaviour
    {
        //[SerializeField] private TextMeshProUGUI nickname;
        //[SerializeField] private TextMeshProUGUI content;

        [SerializeField] private Text nickname;
        [SerializeField] private Text content;

        [SerializeField] private Image badgeImage;
        [SerializeField] private Color[] colorByTypes;

        private RectTransform _transform;

        private void Awake()
        {
            _transform = GetComponent<RectTransform>();
        }

        public void SetData(string senderStr, string contentStr, int type)
        {
            if (ReferenceEquals(_transform, null))
                _transform = GetComponent<RectTransform>();
            nickname.text = senderStr;
            content.text = contentStr;

            nickname.color = colorByTypes[type];
            content.color = colorByTypes[type];

            var sizeDelta = _transform.sizeDelta;
            sizeDelta.y = content.preferredHeight;
            _transform.sizeDelta = sizeDelta;
        }
    }
}