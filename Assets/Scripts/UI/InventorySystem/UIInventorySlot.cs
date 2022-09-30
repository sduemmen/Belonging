using UnityEngine;

namespace UI.InventorySystem
{
    public class UIInventorySlot : UIInventorySlotBase
    {
        [SerializeField] private InventoryDisplay _parentDisplay;

        #region -- Getters --

        public InventoryDisplay GetParentDisplay()
        {
            return _parentDisplay;
        }

        #endregion

        private void Awake()
        {
            _parentDisplay = this.transform.parent.GetComponent<InventoryDisplay>();
        }

        public void OnInventorySlotClicked()
        {
            if (_parentDisplay.IsInteractable()) {
                _parentDisplay.OnSlotClicked(this);
            }
            Debug.Log("slot clicked");
        }
    }
}
