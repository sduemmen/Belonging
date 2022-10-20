using BuildSystem.UI;
using InventorySystem.UI;
using UnityEngine;

namespace BuildSystem
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