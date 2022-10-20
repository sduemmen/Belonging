using System;
using InventorySystem.Items;

namespace BuildSystem
{
    [Serializable]
    public class BuildCost
    {
        public ItemObject item;
        public int amount;
    }
}