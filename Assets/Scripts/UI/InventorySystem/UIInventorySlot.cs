using System;
using InventorySystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.InventorySystem
{
    public class UIInventorySlot : MonoBehaviour
    {
        [SerializeField] protected InventoryDisplay _parentDisplay;
        [SerializeField] protected InventorySlot _assignedInventorySlot;
        [SerializeField] protected Image _icon;
        [SerializeField] protected TextMeshProUGUI _stackSizeLabel;

        #region -- Getters, Setters --

        public InventorySlot AssignedInventorySlot {
            get => _assignedInventorySlot;
            set {
                _assignedInventorySlot = value ?? InventorySlot.EMPTY;
                _icon.sprite = value != null ? value.Item != null ? value.Item.Icon : null : null;
                _icon.color = value != null ? value.Item != null ? Color.white : Color.clear : Color.clear;
                _stackSizeLabel.text = value is { StackSize: > 1 } ? value.StackSize.ToString() : "";
            }
        }

        #endregion

        #region -- Lifecycle --

        private void Awake()
        {
            _parentDisplay = transform.parent.GetComponent<InventoryDisplay>();
        }

        #endregion

        public void OnInventorySlotClicked()
        {
            _parentDisplay.OnSlotClicked(this);
            Debug.Log("slot clicked");
        }
        
        public void Initialize(InventorySlot inventorySlot)
        {
            AssignedInventorySlot = inventorySlot;
        }

        // public void AssignIcon(Sprite icon)
        // {
        //     _icon.sprite = icon;
        //     _icon.color = Color.white;
        // }
        //
        // public void ClearIcon()
        // {
        //     _icon.sprite = null;
        //     _icon.color = Color.clear;
        // }
        //
        // public void Refresh()
        // {
        //     if (_assignedInventorySlot.GetItem() != null) {
        //         AssignIcon(_assignedInventorySlot.GetItem().GetIcon());
        //         int itemStackSize = _assignedInventorySlot.GetStackSize();
        //         string itemStackSizeString = itemStackSize > 1 ? itemStackSize.ToString() : "";
        //         _stackSizeLabel.SetText(itemStackSizeString);
        //     } else {
        //         ClearSlot();
        //     }
        // }
    }
}