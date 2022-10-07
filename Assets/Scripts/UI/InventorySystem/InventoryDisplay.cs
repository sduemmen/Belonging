using System.Collections.Generic;
using InventorySystem;
using Items;
using UnityEngine;

namespace UI.InventorySystem
{
    public class InventoryDisplay : MonoBehaviour
    {
        [SerializeField] protected MouseInventory _mouseInventory;
        [SerializeField] protected Inventory _inventory;
        [SerializeField] protected UIInventorySlot[] _UIInventorySlots;
        [SerializeField] protected Dictionary<UIInventorySlot, InventorySlot> _inventorySlotDict;
        [SerializeField] protected InventoryHolder _inventoryHolder;
        [SerializeField] protected bool interactable = true;

        #region -- Getters --

        public Inventory GetInventory()
        {
            return _inventory;
        }

        public UIInventorySlot[] GetUIInventorySlots()
        {
            return _UIInventorySlots;
        }

        public Dictionary<UIInventorySlot, InventorySlot> GetInventorySlotDict()
        {
            return _inventorySlotDict;
        }

        public InventoryHolder GetInventoryHolder()
        {
            return _inventoryHolder;
        }

        public bool IsInteractable()
        {
            return interactable;
        }

        #endregion

        protected virtual void Start()
        {
            if (_inventoryHolder != null) {
                _inventory = _inventoryHolder.GetInventory();
                _inventory.OnInventorySlotChanged += UpdateSlot;
            } else {
                Debug.Log($"No inventory assigned to {this.gameObject}");
            }
            
            InitializeInventorySlots(_inventory);
        }

        public virtual void InitializeInventorySlots(Inventory inventory)
        {
            _inventorySlotDict = new Dictionary<UIInventorySlot, InventorySlot>();
            
            if (_UIInventorySlots.Length != _inventory.GetInventorySize()) Debug.Log($"Inventory slots out of sync on {this.gameObject}");

            for (int i = 0; i < inventory.GetInventorySize(); i++) {
                _inventorySlotDict.Add(_UIInventorySlots[i], _inventory.GetInventorySlots()[i]);
                _UIInventorySlots[i].Initialize(_inventory.GetInventorySlots()[i]);
            }
        }

        public void UpdateSlot(InventorySlot newInventorySlot)
        {
            foreach (var slot in _inventorySlotDict) {
                if (slot.Value == newInventorySlot) {
                    slot.Key.Initialize(newInventorySlot);
                }
            }
        }

        public void EnableHighlightAtIndex(int index)
        {
            _UIInventorySlots[index].EnableHighlight();
        }

        public void DisableHighlightAtIndex(int index)
        {
            _UIInventorySlots[index].DisableHighlight();
        }

        public void OnSlotClicked(UIInventorySlot clickedUIInventorySlot)
        {
            bool clickedSlotHasItem = clickedUIInventorySlot.GetAssignedInventorySlot().GetItem() != null;
            bool mouseInventoryIsEmpty = _mouseInventory.GetAssignedInventorySlot().GetItem() == null;

            if (clickedSlotHasItem && mouseInventoryIsEmpty) {
                TakeSlot(clickedUIInventorySlot);
            }

            if (!clickedSlotHasItem && !mouseInventoryIsEmpty) {
                PlaceOnSlot(clickedUIInventorySlot);
            }

            if (clickedSlotHasItem && !mouseInventoryIsEmpty) {
                bool itemsAreEqual = _mouseInventory.GetAssignedInventorySlot().GetItem() == clickedUIInventorySlot.GetAssignedInventorySlot().GetItem();
                
                if (itemsAreEqual) {
                    AddToSlot(clickedUIInventorySlot);
                } else {
                    SwapSlotWithMouseInventory(clickedUIInventorySlot);
                }
            }
            
            _mouseInventory.Refresh();
            clickedUIInventorySlot.Refresh();
        }

        protected void TakeSlot(UIInventorySlot source)
        {
            Item item = source.GetAssignedInventorySlot().GetItem();
            int stackSize = source.GetAssignedInventorySlot().GetStackSize();
            _mouseInventory.GetAssignedInventorySlot().SetItemAndStackSize(item, stackSize);
            source.ClearSlot();
            _mouseInventory.Show();
        }

        protected void PlaceOnSlot(UIInventorySlot target)
        {
            Item item = _mouseInventory.GetAssignedInventorySlot().GetItem();
            int stackSize = _mouseInventory.GetAssignedInventorySlot().GetStackSize();
            target.GetAssignedInventorySlot().SetItemAndStackSize(item, stackSize);
            _mouseInventory.ClearSlot();
            _mouseInventory.Hide();
        }

        protected void AddToSlot(UIInventorySlot target)
        {
            target.GetAssignedInventorySlot().AddToStack(_mouseInventory.GetAssignedInventorySlot().GetStackSize(), out int remainingAmount);
            if (remainingAmount > 0) {
                _mouseInventory.GetAssignedInventorySlot().SetStackSize(remainingAmount);
            } else {
                _mouseInventory.ClearSlot();
                _mouseInventory.Hide();
            }
        }

        protected void SwapSlotWithMouseInventory(UIInventorySlot target)
        {
            Item item = target.GetAssignedInventorySlot().GetItem();
            int stackSize = target.GetAssignedInventorySlot().GetStackSize();
            Item mouseItem = _mouseInventory.GetAssignedInventorySlot().GetItem();
            int mouseStackSize = _mouseInventory.GetAssignedInventorySlot().GetStackSize();
            target.GetAssignedInventorySlot().SetItemAndStackSize(mouseItem, mouseStackSize);
            _mouseInventory.GetAssignedInventorySlot().SetItemAndStackSize(item, stackSize);
        }
    }
}
