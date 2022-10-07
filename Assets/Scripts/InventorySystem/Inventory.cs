using System;
using System.Collections.Generic;
using System.Linq;
using Items;
using UnityEngine;
using UnityEngine.Events;

namespace InventorySystem
{
    [Serializable]
    public class Inventory
    {
        [SerializeField] private List<InventorySlot> _inventorySlots;
        [HideInInspector] public UnityAction<InventorySlot> OnInventorySlotChanged;

        #region -- Getters --

        public List<InventorySlot> GetInventorySlots()
        {
            return _inventorySlots;
        }

        public int GetInventorySize()
        {
            return _inventorySlots.Count;
        }

        #endregion
        
        public Inventory(int size)
        {
            _inventorySlots = new List<InventorySlot>(size);

            for (int i = 0; i < size; i++) {
                _inventorySlots.Add(new InventorySlot());
            }
        }

        public bool AddToInventory(Item itemToAdd, int amountToAdd)
        {
            if (this.Contains(itemToAdd, out List<InventorySlot> slots)) {
                foreach (InventorySlot slot in slots) {
                    if (slot.HasRoomFor(amountToAdd)) {
                        slot.AddToStack(amountToAdd);
                        OnInventorySlotChanged?.Invoke(slot);
                        return true;
                    }
                }
            }

            if (this.HasFreeInventorySlot(out InventorySlot freeSlot)) {
                freeSlot.SetItemAndStackSize(itemToAdd, amountToAdd);
                OnInventorySlotChanged?.Invoke(freeSlot);
                return true;
            }

            return false;
        }

        public bool Contains(Item item, out List<InventorySlot> slots)
        {
            slots = _inventorySlots.Where(inventorySlot => inventorySlot.GetItem() == item).ToList();
            return slots.Count >= 1;
        }

        public bool HasFreeInventorySlot(out InventorySlot freeSlot)
        {
            freeSlot = _inventorySlots.FirstOrDefault(inventorySlot => inventorySlot.GetItem() == null);
            return freeSlot != null;
        }
    }
}

