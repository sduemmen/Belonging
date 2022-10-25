using System;
using InventorySystem.Items;

namespace BuildSystem
{
    [Serializable]
    public class BuildCost
    {
        public ItemObject item;
        public int amount;

        public override string ToString()
        {
            return $"Item: {item.displayName} Amount: {amount}";
        }

        public override bool Equals(object obj)
        {
            if (!(obj is BuildCost)) return false;

            BuildCost other = (BuildCost)obj;
            return other.item == this.item && other.amount == this.amount;
        }
    }
}