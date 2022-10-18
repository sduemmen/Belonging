using System;
using InventorySystem.Items;
using UnityEngine;

namespace InventorySystem
{
    [Serializable]
    public class InventorySlot
    {
        [SerializeField] private ItemObject _item;
        [SerializeField] private int _stackSize;
        [SerializeField] private int _index;

        #region -- Getters, Setters --

        public ItemObject Item {
            get => _item;
            set => _item = value;
        }

        public int StackSize {
            get => _stackSize;
            set => _stackSize = value;
        }

        public int Index {
            get => _index;
            set => _index = value;
        }
        
        #endregion
        
        public InventorySlot(ItemObject item, int stackSize, int index)
        {
            Item = item;
            StackSize = stackSize;
            Index = index;
        }

        public InventorySlot()
        {
            ClearSlot();
        }

        public void AssignItem(InventorySlot other)
        {
            if (Item == other.Item) {
                StackSize += other.StackSize;
            } else {
                Item = other.Item;
                StackSize = other.StackSize;
            }
        }

        public void ClearSlot()
        {
            Item = null;
            StackSize = -1;
        }

        public bool IsEmpty()
        {
            return Item == null && _stackSize < 0;
        }
        
        public bool HasRoomFor(int amountToAdd, out int roomLeft)
        {
            roomLeft = Item.maxStackSize - StackSize;
            return StackSize + amountToAdd <= Item.maxStackSize;
        }

        public bool HasRoomFor(int amountToAdd)
        {
            return StackSize + amountToAdd <= Item.maxStackSize;
        }

        public void AddToStack(int amount)
        {
            if (!HasRoomFor(amount)) return;
            StackSize += amount;
        }

        public void AddToStack(int amountToAdd, out int remainingAmount)
        {
            bool roomLeft = HasRoomFor(amountToAdd);
            
            remainingAmount = roomLeft ? 0 : amountToAdd + StackSize - Item.maxStackSize;
            amountToAdd = roomLeft ? amountToAdd : Item.maxStackSize - StackSize;
            
            AddToStack(amountToAdd);
        }

        public void RemoveFromStack(int amount)
        {
            StackSize -= amount;
        }
    }
}