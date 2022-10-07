using System;
using Items;
using UnityEngine;

namespace InventorySystem
{
    [Serializable]
    public class InventorySlot
    {
        [SerializeField] private Item _item;
        [SerializeField] private int _stackSize;

        #region -- Getters, Setters --

        public Item GetItem()
        {
            return _item;
        }

        public int GetStackSize()
        {
            return _stackSize;
        }

        public void SetStackSize(int newStackSize)
        {
            _stackSize = newStackSize;
        }

        public void SetItemAndStackSize(Item item, int stackSize)
        {
            _item = item;
            _stackSize = stackSize;
        }

        #endregion
        
        public InventorySlot(Item item, int stackSize)
        {
            _item = item;
            _stackSize = stackSize;
        }

        public InventorySlot()
        {
            ClearSlot();
        }

        public void AssignItem(InventorySlot other)
        {
            if (_item == other.GetItem()) {
                AddToStack(other.GetStackSize());
            } else {
                SetItemAndStackSize(other.GetItem(), other.GetStackSize());
            }
        }

        public void ClearSlot()
        {
            _item = null;
            _stackSize = -1;
        }

        public bool IsEmpty()
        {
            return _item == null && _stackSize < 0;
        }
        
        public bool HasRoomFor(int amountToAdd, out int roomLeft)
        {
            roomLeft = _item.GetMaxStackSize() - _stackSize;
            return _stackSize + amountToAdd <= _item.GetMaxStackSize();
        }

        public bool HasRoomFor(int amountToAdd)
        {
            return _stackSize + amountToAdd <= _item.GetMaxStackSize();
        }

        public void AddToStack(int amount)
        {
            if (!HasRoomFor(amount)) return;
            _stackSize += amount;
        }

        public void AddToStack(int amountToAdd, out int remainingAmount)
        {
            bool roomLeft = HasRoomFor(amountToAdd);
            
            remainingAmount = roomLeft ? 0 : amountToAdd + _stackSize - _item.GetMaxStackSize();
            amountToAdd = roomLeft ? amountToAdd : _item.GetMaxStackSize() - _stackSize;
            
            AddToStack(amountToAdd);
        }

        public void RemoveFromStack(int amount)
        {
            _stackSize -= amount;
        }
    }
}
