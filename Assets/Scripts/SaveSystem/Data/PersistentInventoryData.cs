using System;
using System.Collections.Generic;
using InventorySystem;

namespace SaveSystem.Data
{
    [Serializable]
    public class PersistentInventoryData
    {
        public string identifier;
        public List<InventorySlot> inventorySlots;

        public PersistentInventoryData(string id, List<InventorySlot> inventorySlots)
        {
            identifier = id;
            this.inventorySlots = inventorySlots;
        }

        public PersistentInventoryData(Inventory inventory)
        {
            identifier = inventory.identifier;
            inventorySlots = inventory.InventorySlots;
        }
    }
}