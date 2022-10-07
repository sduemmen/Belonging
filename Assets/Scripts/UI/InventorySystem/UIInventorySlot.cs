using UnityEngine;
using UnityEngine.UI;

namespace UI.InventorySystem
{
    public class UIInventorySlot : UIInventorySlotBase
    {
        [SerializeField] private InventoryDisplay _parentDisplay;
        [SerializeField] private Image borderHighlight;

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

        public void EnableHighlight()
        {
            borderHighlight.gameObject.SetActive(true);
        }

        public void DisableHighlight()
        {
            borderHighlight.gameObject.SetActive(false);
        }
    }
}
