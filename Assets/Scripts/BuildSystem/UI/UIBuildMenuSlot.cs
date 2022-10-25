using System.Collections.Generic;
using Collections;
using Events.Events;
using InventorySystem;
using InventorySystem.UI;
using UnityEngine;

namespace BuildSystem.UI
{
    public class UIBuildMenuSlot : UIInventorySlot
    {
        public string displayName;
        public string prefabName;
        public List<BuildCost> buildCosts;
        public BuildCostTooltip buildCostTooltipPrefab;
        public TooltipEvent tooltipShowEvent;
        public SimpleEvent tooltipHideEvent;

        public override void OnInventorySlotClicked()
        {
            if (!UnlockSystem.Instance.SegmentUnlocked(displayName)) return;
            // invokes build menu display OnSlotClicked event
            base.OnInventorySlotClicked(); 
            RaiseTooltipHideEvent();
        }

        public void Initialize(CollectionEntry entry)
        {
            SegmentCollectionEntry segmentCollectionEntry = (SegmentCollectionEntry) entry;
            displayName = segmentCollectionEntry.displayName;
            prefabName = segmentCollectionEntry.prefabName;
            buildCosts = segmentCollectionEntry.buildCosts;
            
            _image.sprite = segmentCollectionEntry.previewImage;
            _image.color = UnlockSystem.Instance.SegmentUnlocked(displayName) ? Color.white : Color.gray;
            this._stackSizeLabel.text = "";
        }

        public void OnUnlockSegment()
        {
            _image.color = UnlockSystem.Instance.SegmentUnlocked(displayName) ? Color.white : Color.gray;
        }

        public void RaiseTooltipShowEvent()
        {
            Rect tooltipRect = buildCostTooltipPrefab.GetComponent<RectTransform>().rect;
            Vector2 offset = new Vector2(tooltipRect.width / 2 + 10, tooltipRect.height / 2 + 10);
            
            BuildCostTooltip tooltip = Instantiate(buildCostTooltipPrefab, offset, Quaternion.identity);
            tooltip.Initialize(displayName, buildCosts);
            
            tooltipShowEvent.Raise(tooltip.gameObject);
        }
        
        public void RaiseTooltipHideEvent()
        {
            tooltipHideEvent.Raise();
        }
    }
}