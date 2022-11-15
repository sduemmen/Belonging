using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.MouseHover
{
    public class TextTooltip : Tooltip
    {
        [SerializeField] private TextMeshProUGUI _label;
        [SerializeField] private LayoutElement _layoutElement;
        private const int _charWrapLimit = 100;
        
        public void Initialize(string text)
        {
            _label.text = text;
            _layoutElement.enabled = text.Length > _charWrapLimit;
            LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)_label.transform);
            LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)this.transform);
        }
    }
}