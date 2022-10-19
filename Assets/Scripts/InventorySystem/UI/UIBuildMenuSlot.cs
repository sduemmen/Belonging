using InventorySystem.Collections;
using UnityEngine;

namespace InventorySystem.UI
{
    public class UIBuildMenuSlot : UIInventorySlot
    {
        public string prefabName;
        public GameObject prefab;

        public override void OnInventorySlotClicked()
        {
            base.OnInventorySlotClicked();
        }

        public void Initialize(CollectionEntry entry)
        {
            SegmentCollectionEntry segmentCollectionEntry = (SegmentCollectionEntry) entry;
            prefabName = segmentCollectionEntry.prefabName;
            prefab = segmentCollectionEntry.prefab;
            _image.sprite = segmentCollectionEntry.previewImage;
            this._stackSizeLabel.text = "";
        }
    }
}