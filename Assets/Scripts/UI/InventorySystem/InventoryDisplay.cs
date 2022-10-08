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
        [SerializeField] protected bool _interactable = true;

        #region -- Getters, Setters --

        public bool IsInteractable {
            get => _interactable;
            private set => _interactable = value;
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
            
            if (_UIInventorySlots.Length != _inventory.Size) Debug.Log($"Inventory slots out of sync on {this.gameObject}");

            for (int i = 0; i < inventory.Size; i++) {
                _inventorySlotDict.Add(_UIInventorySlots[i], _inventory.InventorySlots[i]);
                _UIInventorySlots[i].AssignedInventorySlot = _inventory.InventorySlots[i];
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

        public void OnSlotClicked(UIInventorySlot clickedUIInventorySlot)
        {
            if (!IsInteractable) return;
            bool clickedSlotHasItem = !clickedUIInventorySlot.AssignedInventorySlot.IsEmpty();
            bool mouseInventoryIsEmpty = _mouseInventory.AssignedInventorySlot.IsEmpty();

            if (clickedSlotHasItem && mouseInventoryIsEmpty) {
                TakeSlot(clickedUIInventorySlot);
            }

            if (!clickedSlotHasItem && !mouseInventoryIsEmpty) {
                PlaceOnSlot(clickedUIInventorySlot);
            }

            if (clickedSlotHasItem && !mouseInventoryIsEmpty) {
                bool itemsAreEqual = _mouseInventory.AssignedInventorySlot.Item == clickedUIInventorySlot.AssignedInventorySlot.Item;
                
                if (itemsAreEqual) {
                    AddToSlot(clickedUIInventorySlot);
                } else {
                    SwapSlotWithMouseInventory(clickedUIInventorySlot);
                }
            }
        }

        protected void TakeSlot(UIInventorySlot source)
        {
            Item item = source.AssignedInventorySlot.Item;
            int stackSize = source.AssignedInventorySlot.StackSize;
            _mouseInventory.AssignedInventorySlot = new InventorySlot(item, stackSize);
            source.AssignedInventorySlot = null;
        }

        protected void PlaceOnSlot(UIInventorySlot target)
        {
            Item item = _mouseInventory.AssignedInventorySlot.Item;
            int stackSize = _mouseInventory.AssignedInventorySlot.StackSize;
            target.AssignedInventorySlot = new InventorySlot(item, stackSize);
            _mouseInventory.AssignedInventorySlot = null;
        }

        protected void AddToSlot(UIInventorySlot target)
        {
            target.AssignedInventorySlot.AddToStack(_mouseInventory.AssignedInventorySlot.StackSize, out int remainingAmount);
            if (remainingAmount > 0) {
                _mouseInventory.AssignedInventorySlot = new InventorySlot(_mouseInventory.AssignedInventorySlot.Item, remainingAmount);
            } else {
                _mouseInventory.AssignedInventorySlot = null;
            }
        }

        protected void SwapSlotWithMouseInventory(UIInventorySlot target)
        {
            Item targetItem = target.AssignedInventorySlot.Item;
            int targetStackSize = target.AssignedInventorySlot.StackSize;
            Item mouseItem = _mouseInventory.AssignedInventorySlot.Item;
            int mouseStackSize = _mouseInventory.AssignedInventorySlot.StackSize;
            target.AssignedInventorySlot = new InventorySlot(mouseItem, mouseStackSize);
            _mouseInventory.AssignedInventorySlot = new InventorySlot(targetItem, targetStackSize);
        }
    }
}
