using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class MultipleTintButton : Button
    {
        #region PrivateFields
        
        private TextMeshProUGUI[] _texts;
        private Image[] _images;
        
        #endregion
        
        protected virtual IEnumerator TweenColorFromCurrent (Color ToColor, float duration)
        {
            for (float f = 0; f <= duration; f = f + Time.deltaTime) 
            {
                for (var i = 0; i < _texts.Length; ++i)
                {
                    if (!ReferenceEquals(_texts[i], null))
                        _texts[i].color = Color.Lerp(_texts[i].color, ToColor, f);
                }

                for (var i = 0; i < _images.Length; ++i)
                {
                    if (!ReferenceEquals(_images[i], null))
                        _images[i].color = Color.Lerp(_images[i].color, ToColor, f);
                }

                yield return null;
 
            }
            for (var i = 0; i < _texts.Length; ++i)
            {
                if (!ReferenceEquals(_texts[i], null))
                    _texts[i].color = ToColor;
            }

            for (var i = 0; i < _images.Length; ++i)
            {
                if (!ReferenceEquals(_images[i], null))
                    _images[i].color = ToColor;
            }
        }
 
 
        protected override void DoStateTransition (Selectable.SelectionState state, bool instant)
        {
            if (ReferenceEquals(_texts, null))
                _texts = GetComponentsInChildren<TextMeshProUGUI>();
            if (ReferenceEquals(_images, null))
                _images = GetComponentsInChildren<Image>();
                
            if (state == SelectionState.Pressed) 
            {
                StopAllCoroutines ();
                StartCoroutine (TweenColorFromCurrent (this.colors.pressedColor, this.colors.fadeDuration));
            }
         
            if (state == Selectable.SelectionState.Highlighted) 
            {
                StopAllCoroutines ();
                StartCoroutine (TweenColorFromCurrent (this.colors.highlightedColor, this.colors.fadeDuration));
            }
            if (state == Selectable.SelectionState.Normal) 
            {
                StopAllCoroutines ();
                StartCoroutine (TweenColorFromCurrent (this.colors.normalColor, this.colors.fadeDuration));
            }

            if (state == Selectable.SelectionState.Selected)
            {
                StopAllCoroutines ();
                StartCoroutine (TweenColorFromCurrent (this.colors.selectedColor, this.colors.fadeDuration));
            }
            base.DoStateTransition(state, instant);
        }
    }
}
