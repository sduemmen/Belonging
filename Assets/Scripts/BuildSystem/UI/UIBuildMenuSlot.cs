using System.Collections.Generic;
using Collections;
using Events.Events;
using InventorySystem.UI;
using UnityEngine;

namespace BuildSystem.UI
{
    public class UIBuildMenuSlot : UIInventorySlot
    {
        public string displayName;
        public string prefabName;
        public GameObject prefab;
        public List<BuildCost> buildCosts;
        public BuildCostTooltip buildCostTooltipPrefab;
        public TooltipEvent tooltipShowEvent;
        public SimpleEvent tooltipHideEvent;

        public override void OnInventorySlotClicked()
        {
            base.OnInventorySlotClicked();
        }

        public void Initialize(CollectionEntry entry)
        {
            SegmentCollectionEntry segmentCollectionEntry = (SegmentCollectionEntry) entry;
            displayName = segmentCollectionEntry.displayName;
            prefabName = segmentCollectionEntry.prefabName;
            prefab = segmentCollectionEntry.prefab;
            buildCosts = segmentCollectionEntry.buildCosts;
            
            _image.sprite = segmentCollectionEntry.previewImage;
            this._stackSizeLabel.text = "";
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