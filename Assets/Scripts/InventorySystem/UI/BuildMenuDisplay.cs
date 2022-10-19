using UnityEngine;

namespace InventorySystem.UI
{
    public class BuildMenuDisplay : InventoryDisplay
    {
        public GameObject UICollectionRowPrefab;
        
        public new UIBuildMenuSlot AddSlot()
        {
            return (UIBuildMenuSlot)Instantiate(_uiSlotPrefab, this.transform, false);
        }
    }
}