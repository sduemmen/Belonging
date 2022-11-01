using System;
using InventorySystem.Items;
using UnityEngine;

namespace BuildSystem
{
    [Serializable]
    public class ItemStack
    {
        [SerializeField] private ItemObject _item;
        [SerializeField] private int _amount;

        public ItemObject Item => _item;
        public int Amount => _amount;

        public override string ToString()
        {
            return $"Item: {_item.DisplayName} Amount: {_amount}";
        }

        public override bool Equals(object o)
        {
            if (o.GetType() != typeof(ItemStack)) return false;
            
            return Equals((ItemStack)o);
        }

        public bool Equals(ItemStack other)
        {
            return Equals(_item, other._item) && _amount == other._amount;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(_item, _amount);
        }
    }
}