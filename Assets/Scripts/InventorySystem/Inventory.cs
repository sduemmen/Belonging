using System;
using System.Collections.Generic;
using System.Linq;
using Events.Events;
using InventorySystem.Items;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace InventorySystem
{
    [CreateAssetMenu(menuName = "Inventory/Inventory")]
    [Serializable]
    public class Inventory : ScriptableObject
    {
        [ReadOnly] public string identifier = Guid.NewGuid().ToString();
        [SerializeField] private int _inventorySize;
        [SerializeField] private List<InventorySlot> _inventorySlots;
        [SerializeField] private bool _isStatic;

        [ShowIf("@_isStatic == false")] [SerializeField]
        private SimpleEvent woodCollectedEvent;

        [ShowIf("@_isStatic == false")] [SerializeField]
        private SimpleEvent stoneCollectedEvent;

        [ShowIf("@_isStatic == false")] [SerializeField]
        private SimpleEvent anyCollectedEvent;

        public UnityAction<InventorySlot> OnSlotChangedDelegate;

        public List<InventorySlot> InventorySlots {
            get => _inventorySlots;
            set => _inventorySlots = value;
        }

        public int Size => _inventorySize;

        [Button("Clear Inventory")]
        public void ClearInventory()
        {
            foreach (InventorySlot inventorySlot in _inventorySlots)
            {
                inventorySlot.ClearSlot();
            }
        }

        [Button("Setup Slot indices")]
        public void SetupSlotIndices()
        {
            for (int i = 0; i < Size; i++) _inventorySlots[i].Index = i;
        }

        [Button("Initialize Slots")]
        public void Initialize()
        {
            _inventorySlots = new List<InventorySlot>();
            
            for (int i = 0; i < _inventorySize; i++)
            {
                _inventorySlots.Add(new InventorySlot(null, -1, i));
            }
        }

        public InventorySlot GetSlotAtIndex(int index)
        {
            if (index < 0 || index >= _inventorySlots.Count) return null;
            
            return _inventorySlots[index];
        }

        public bool AddItem(ItemObject itemToAdd, int amountToAdd, bool wasDroppedByPlayer)
        {
            if (this.Contains(itemToAdd, out var slots))
            {
                foreach (InventorySlot slot in slots)
                {
                    if (slot.HasRoomFor(amountToAdd))
                    {
                        slot.AddToStack(amountToAdd);

                        if (!wasDroppedByPlayer)
                        {
                            anyCollectedEvent.Raise();
                            if (itemToAdd.DisplayName == "Wood")
                            {
                                woodCollectedEvent.Raise();
                            }
                            else if (itemToAdd.DisplayName == "Stone")
                            {
                                stoneCollectedEvent.Raise();
                            }
                        }

                        OnSlotChangedDelegate?.Invoke(slot);
                        return true;
                    }
                } 
            }

            if (this.HasFreeInventorySlot(out InventorySlot freeSlot))
            {
                freeSlot.Item = itemToAdd;
                freeSlot.StackSize = amountToAdd;

                if (!wasDroppedByPlayer)
                {
                    anyCollectedEvent.Raise();
                    if (itemToAdd.DisplayName == "Wood")
                    {
                        woodCollectedEvent.Raise();
                    }
                    else if (itemToAdd.DisplayName == "Stone")
                    {
                        stoneCollectedEvent.Raise();
                    }
                }

                OnSlotChangedDelegate?.Invoke(freeSlot);
                return true;
            }

            return false;
        }

        public void RemoveItem(ItemObject item, int amount)
        {
            for (int i = _inventorySize - 1; i >= 0; i--)
                if (_inventorySlots[i].Item == item)
                {
                    if (_inventorySlots[i].StackSize <= amount)
                    {
                        amount -= _inventorySlots[i].StackSize;
                        _inventorySlots[i].ClearSlot();
                    }
                    else
                    {
                        _inventorySlots[i].RemoveFromStack(amount);
                    }

                    OnSlotChangedDelegate?.Invoke(_inventorySlots[i]);
                }
        }

        public bool Contains(ItemObject item, out List<InventorySlot> slots)
        {
            slots = _inventorySlots.Where(inventorySlot => inventorySlot.Item == item).ToList();
            return slots.Count >= 1;
        }

        public bool Contains(ItemObject item, int amount, out int totalAmount)
        {
            totalAmount = 0;
            foreach (InventorySlot inventorySlot in _inventorySlots)
                if (inventorySlot.Item == item)
                    totalAmount += inventorySlot.StackSize;

            return totalAmount >= amount;
        }

        public bool Contains(ItemObject item, int amount)
        {
            int amountInInventory = 0;
            foreach (InventorySlot inventorySlot in _inventorySlots)
                if (inventorySlot.Item == item)
                    amountInInventory += inventorySlot.StackSize;

            return amountInInventory >= amount;
        }

        public bool HasFreeInventorySlot(out InventorySlot freeSlot)
        {
            freeSlot = _inventorySlots.FirstOrDefault(inventorySlot => inventorySlot.Item == null);
            return freeSlot != null;
        }
    }
}