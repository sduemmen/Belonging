using InventorySystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.InventorySystem
{
    public abstract class UIInventorySlotBase : MonoBehaviour
    {
        [SerializeField] protected Image _icon;
        [SerializeField] protected TextMeshProUGUI _stackSizeLabel;
        [SerializeField] protected InventorySlot _assignedInventorySlot;

        #region -- Getters --

        public InventorySlot GetAssignedInventorySlot()
        {
            return _assignedInventorySlot;
        }

        public Image GetIcon()
        {
            return _icon;
        }

        public TextMeshProUGUI GetStackSizeLabel()
        {
            return _stackSizeLabel;
        }

        #endregion
        
        public void Initialize(InventorySlot inventorySlot)
        {
            _assignedInventorySlot = inventorySlot;
            Refresh();
        }

        public void AssignInventorySlot(InventorySlot inventorySlot)
        {
            _assignedInventorySlot.AssignItem(inventorySlot);
            Refresh();
        }
        
        public void Refresh()
        {
            if (_assignedInventorySlot.GetItem() != null) {
                _icon.sprite = _assignedInventorySlot.GetItem().GetIcon();
                _icon.color = Color.white;
                int itemStackSize = _assignedInventorySlot.GetStackSize();
                string itemStackSizeString = itemStackSize > 1 ? itemStackSize.ToString() : "";
                _stackSizeLabel.SetText(itemStackSizeString);
            } else {
                ClearSlot();
            }
        }
        
        public void ClearSlot()
        {
            _assignedInventorySlot.ClearSlot();
            _icon.sprite = null;
            _icon.color = Color.clear;
            _stackSizeLabel.SetText("");
        }
    }
}