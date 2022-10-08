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

        public Item Item {
            get => _item;
            set => _item = value;
        }

        public int StackSize {
            get => _stackSize;
            set => _stackSize = value;
        }

        public static InventorySlot EMPTY => new InventorySlot();

        #endregion
        
        public InventorySlot(Item item, int stackSize)
        {
            Item = item;
            StackSize = stackSize;
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
            roomLeft = Item.MaxStackSize - StackSize;
            return StackSize + amountToAdd <= Item.MaxStackSize;
        }

        public bool HasRoomFor(int amountToAdd)
        {
            return StackSize + amountToAdd <= Item.MaxStackSize;
        }

        public void AddToStack(int amount)
        {
            if (!HasRoomFor(amount)) return;
            StackSize += amount;
        }

        public void AddToStack(int amountToAdd, out int remainingAmount)
        {
            bool roomLeft = HasRoomFor(amountToAdd);
            
            remainingAmount = roomLeft ? 0 : amountToAdd + StackSize - Item.MaxStackSize;
            amountToAdd = roomLeft ? amountToAdd : Item.MaxStackSize -StackSize;
            
            AddToStack(amountToAdd);
        }

        public void RemoveFromStack(int amount)
        {
            StackSize -= amount;
        }
    }
}
