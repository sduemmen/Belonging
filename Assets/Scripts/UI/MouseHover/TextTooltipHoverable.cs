using System;
using UnityEngine;

namespace UI.MouseHover
{
    public class TextTooltipHoverable : Hoverable
    {
        [SerializeField, TextArea] private string _tooltipText;
        
        public override void OnTooltipVisible(Tooltip tooltip)
        {
            if (tooltip.GetType() != typeof(TextTooltip))
            {
                throw new ArgumentException($"Tooltip is of type \"{tooltip.GetType()}\". Expected \"{typeof(TextTooltip)}\"");
            }
            
            ((TextTooltip)tooltip).Initialize(_tooltipText);
        }
    }
}