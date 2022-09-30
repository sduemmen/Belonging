using System;
using InventorySystem;

namespace SaveSystem.Data
{
    [Serializable]
    public class PersistentInventoryData
    {
        public string identifier;
        public Inventory inventory;

        public PersistentInventoryData(string id, Inventory inventory)
        {
            identifier = id;
            this.inventory = inventory;
        }

        public PersistentInventoryData(InventoryHolder inventoryHolder)
        {
            this.identifier = inventoryHolder.identifier.ToString();
            this.inventory = inventoryHolder.GetInventory();
        }
    }
}