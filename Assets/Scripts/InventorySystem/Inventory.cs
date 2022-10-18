using System;
using System.Collections.Generic;
using System.Linq;
using Flags;
using InventorySystem.Items;
using SaveSystem;
using SaveSystem.Data;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace InventorySystem
{
    [CreateAssetMenu(menuName = "Inventory/Inventory"), Serializable]
    public class Inventory : ScriptableObject, IDataPersistence
    {
        [ReadOnly] public string identifier = Guid.NewGuid().ToString();
        public UnityAction<InventorySlot> OnSlotChanged;
        
        [Button("Clear Inventory")]
        public void ClearInventory()
        {
            foreach (InventorySlot inventorySlot in _inventorySlots) {
                inventorySlot.ClearSlot();
            }
        }
        
        [Button("Setup Slot indices")]
        public void SetupSlotIndices()
        {
            for (int i = 0; i < Size; i++) {
                _inventorySlots[i].Index = i;
            }
        }
        
        [SerializeField] private List<InventorySlot> _inventorySlots;

        public List<InventorySlot> InventorySlots => _inventorySlots;
        public int Size => _inventorySlots.Count;

        public InventorySlot GetSlotAtIndex(int index)
        {
            if (index < 0 || index >= _inventorySlots.Count) return null;
            return _inventorySlots[index];
        }

        public bool AddToInventory(ItemObject itemToAdd, int amountToAdd)
        {
            if (this.Contains(itemToAdd, out List<InventorySlot> slots)) {
                foreach (InventorySlot slot in slots) {
                    if (slot.HasRoomFor(amountToAdd)) {
                        slot.AddToStack(amountToAdd);
                        Debug.Log("picked up item");
                        OnSlotChanged?.Invoke(slot);
                        return true;
                    }
                }
            }

            if (this.HasFreeInventorySlot(out InventorySlot freeSlot)) {
                freeSlot.Item = itemToAdd;
                freeSlot.StackSize = amountToAdd;
                Debug.Log("picked up item");
                OnSlotChanged?.Invoke(freeSlot);
                return true;
            }

            return false;
        }

        public bool Contains(ItemObject item, out List<InventorySlot> slots)
        {
            slots = _inventorySlots.Where(inventorySlot => inventorySlot.Item == item).ToList();
            return slots.Count >= 1;
        }

        public bool HasFreeInventorySlot(out InventorySlot freeSlot)
        {
            freeSlot = _inventorySlots.FirstOrDefault(inventorySlot => inventorySlot.Item == null);
            return freeSlot != null;
        }
        
        public void LoadData(GameData data)
        {
            if (GameFlags.MAIN_MENU_ACTIVE) return;
            PersistentInventoryData inventoryData = data.persistentInventoryData.Find(entry => entry.identifier == this.identifier);

            if (inventoryData != null) {
                _inventorySlots = inventoryData.inventorySlots;
            } else {
                _inventorySlots = new List<InventorySlot>();
            }
        }

        public void SaveData(ref GameData data)
        {
            if (GameFlags.MAIN_MENU_ACTIVE) return;
            PersistentInventoryData existingInventoryData = data.persistentInventoryData.Find(entry => entry.identifier == this.identifier);
            if (existingInventoryData != null) {
                data.persistentInventoryData.Remove(existingInventoryData);
            }
            data.persistentInventoryData.Add(new PersistentInventoryData(this));
        }
    }
}

