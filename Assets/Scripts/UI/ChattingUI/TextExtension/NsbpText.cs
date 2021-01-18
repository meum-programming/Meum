using TMPro;

namespace UI.ChattingUI.TextExtension
{
    public class NsbpText : TextMeshProUGUI
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
