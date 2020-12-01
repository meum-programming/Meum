using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI.ChattingUI.TextExtension
{
    public class NsbpText : Text
    {
        public override string text
        {
            get => base.text;
            set
            {
                var nsbp = value.Replace(' ', '\u00A0');
                base.text = nsbp;
            }
        }
    }
}
