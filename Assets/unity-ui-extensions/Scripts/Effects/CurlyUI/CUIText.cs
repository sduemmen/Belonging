/// Credit Titinious (https://github.com/Titinious)
/// Sourced from - https://github.com/Titinious/CurlyUI

using UnityEngine;
using UnityEngine.UI;

namespace unity_ui_extensions.Scripts.Effects.CurlyUI
{
    [RequireComponent(typeof(RectTransform))]
    [RequireComponent(typeof(Text))]
    [AddComponentMenu("UI/Effects/Extensions/Curly UI Text")]
    public class CUIText : CUIGraphic
    {
        public override void ReportSet()
        {
            if (uiGraphic == null)
                uiGraphic = GetComponent<Text>();

            base.ReportSet();
        }
    }
}