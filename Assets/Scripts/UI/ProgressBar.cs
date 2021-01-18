using UnityEngine;

namespace UI
{
    public class ProgressBar : MonoBehaviour
    {
        [SerializeField] private RectTransform progress;

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
    }
}