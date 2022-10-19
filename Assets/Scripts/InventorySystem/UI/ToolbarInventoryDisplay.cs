using UnityEngine.UI;

namespace InventorySystem.UI
{
    public class ToolbarInventoryDisplay : InventoryDisplay
    {
        public Image highlightImage;
        
        private void Start()
        {
            highlightImage.gameObject.SetActive(false);
        }

        public override UIInventorySlot AddSlot()
        {
            UIInventorySlot uiSlot = Instantiate(_uiSlotPrefab, transform);
            uiSlot.Index = _inventorySlots.Count;
            uiSlot.clickable = false;
            _inventorySlots.Add(uiSlot);
            return uiSlot;
        }

        public void EnableHighlightAtIndex(int index)
        {
            highlightImage.gameObject.SetActive(true);
            highlightImage.transform.position = transform.GetChild(index).position;
        }
        
        public void DisableHighlight()
        {
            highlightImage.gameObject.SetActive(false);
        }
    }
}