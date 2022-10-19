using System;
using System.Collections.Generic;
using System.Linq;
using InventorySystem.Items;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace InventorySystem
{
    [CreateAssetMenu(menuName = "Inventory/Inventory"), Serializable]
    public class Inventory : ScriptableObject
    {
        [ReadOnly] public string identifier = Guid.NewGuid().ToString();
        [SerializeField] private int _inventorySize;
        [SerializeField] private bool _isStatic;
        [SerializeField] private List<InventorySlot> _inventorySlots;
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

        [Button("Initialize Slots")]
        public void Awake()
        {
            if (_isStatic) return;
            
            _inventorySlots = new List<InventorySlot>();
            for (int i = 0; i < _inventorySize; i++) {
                _inventorySlots.Add(new InventorySlot());
            }
        }

        public List<InventorySlot> InventorySlots {
            get => _inventorySlots;
            set => _inventorySlots = value;
        }

        public bool IsStatic => _isStatic;

        public int Size => _inventorySize;

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
    }
}

