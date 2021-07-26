using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class ProgressBar : MonoBehaviour
    {
        [SerializeField] private RectTransform progress;
        [SerializeField] private Text text;

        public void SetProgress(float v)
        {
            v = Mathf.Clamp(v, 0, 1);
            var localScale = progress.localScale;
            localScale.x = v;
            progress.localScale = localScale;
        }

        public float GetProgress()
        {
            return progress.localScale.x;
        }

        int index = 0;

        float delay = 0;

        private void Update()
        {
            DelayCheck();
        }

        void DelayCheck()
        {
            delay -= Time.deltaTime;

            if (delay < 0)
            {
                delay = 0.4f;
                index = (index + 1) % 4;
                TextSet();
            }
        }

        void TextSet()
        {
            string textValue = "로딩중";

            for (int i = 0; i < index; i++)
            {
                textValue += ".";
            }

            text.text = textValue;
        }


    }
}