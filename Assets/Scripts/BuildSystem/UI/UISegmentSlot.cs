using System;
using System.Collections.Generic;
using UI;
using UI.MouseHover;
using UnityEngine;
using UnityEngine.UI;

namespace BuildSystem.UI
{
    [Serializable]
    public class UISegmentSlot : Hoverable
    {
        [SerializeField] private string _segmentName;
        [SerializeField] private List<ItemStack> _buildCosts;
        [SerializeField] private Image _previewImage;

        public string SegmentName => _segmentName;

        public void OnSlotClicked()
        {
            if (!BuildingController.Instance.IsSegmentUnlocked(_segmentName)) return;

            BuildingController.OnSegmentSlotClickedDelegate?.Invoke(this);
            MouseTooltip.Instance.Hide();
        }

        public void Initialize(Segment segment)
        {
            _segmentName = segment.Name;
            _buildCosts = segment.BuildCosts;

            _previewImage.sprite = segment.PreviewImage;
            _previewImage.color = BuildingController.Instance.IsSegmentUnlocked(_segmentName) ? Color.white : Color.gray;
        }

        public void OnSegmentUnlocked()
        {
            _previewImage.color = BuildingController.Instance.IsSegmentUnlocked(_segmentName) ? Color.white : Color.gray;
        }

        public override void OnTooltipVisible(Tooltip tooltip)
        {
            if (tooltip.GetType() != typeof(BuildCostTooltip))
            {
                throw new ArgumentException($"Tooltip is of type \"{tooltip.GetType()}\". Expected \"{typeof(BuildCostTooltip)}\"");
            }

            ((BuildCostTooltip)tooltip).Initialize(_segmentName, _buildCosts);
        }
    }
}