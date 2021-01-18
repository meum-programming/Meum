using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class MultipleTintButton : Button
    {
        private TextMeshProUGUI[] _texts;
        private Image[] _images;

        protected virtual IEnumerator TweenColorFromCurrent (Color toColor, float duration)
        {
            for (float f = 0; f <= duration; f = f + Time.deltaTime) 
            {
                for (var i = 0; i < _texts.Length; ++i)
                {
                    if (!ReferenceEquals(_texts[i], null))
                        _texts[i].color = Color.Lerp(_texts[i].color, toColor, f);
                }

                for (var i = 0; i < _images.Length; ++i)
                {
                    if (!ReferenceEquals(_images[i], null))
                        _images[i].color = Color.Lerp(_images[i].color, toColor, f);
                }

                yield return null;
 
            }
            for (var i = 0; i < _texts.Length; ++i)
            {
                if (!ReferenceEquals(_texts[i], null))
                    _texts[i].color = toColor;
            }

            for (var i = 0; i < _images.Length; ++i)
            {
                if (!ReferenceEquals(_images[i], null))
                    _images[i].color = toColor;
            }
        }
 
 
        protected override void DoStateTransition (Selectable.SelectionState state, bool instant)
        {
            if (ReferenceEquals(_texts, null))
                _texts = GetComponentsInChildren<TextMeshProUGUI>();
            if (ReferenceEquals(_images, null))
                _images = GetComponentsInChildren<Image>();

            var colors = this.colors;
                
            if (state == SelectionState.Pressed) 
            {
                StopAllCoroutines ();
                StartCoroutine (TweenColorFromCurrent (colors.pressedColor, colors.fadeDuration));
            }
         
            if (state == Selectable.SelectionState.Highlighted) 
            {
                StopAllCoroutines ();
                StartCoroutine (TweenColorFromCurrent (colors.highlightedColor, colors.fadeDuration));
            }
            if (state == Selectable.SelectionState.Normal) 
            {
                StopAllCoroutines ();
                StartCoroutine (TweenColorFromCurrent (colors.normalColor, colors.fadeDuration));
            }

            if (state == Selectable.SelectionState.Selected)
            {
                StopAllCoroutines ();
                StartCoroutine (TweenColorFromCurrent (colors.selectedColor, colors.fadeDuration));
            }
            base.DoStateTransition(state, instant);
        }
    }
}
