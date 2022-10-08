using System;
using Flags;
using SaveSystem;
using SaveSystem.Data;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace InventorySystem
{
    [Serializable]
    public class InventoryHolder : MonoBehaviour, IDataPersistence
    {
        [ReadOnly] public string identifier = Guid.NewGuid().ToString();
        [SerializeField] protected int _inventorySize;
        [SerializeField] protected Inventory _inventory;

        public static UnityAction<Inventory> OnDynamicInventoryDisplayContextRequested;
        public static UnityAction OnDynamicInventoryDisplayContextClosed;

        #region -- Getters, Setters --

        public Inventory GetInventory()
        {
            return _inventory;
        }

        public int GetInventorySize()
        {
            return _inventorySize;
        }

        public void SetInventory(Inventory inventory)
        {
            _inventory = inventory;
        }

        #endregion

        private void Start()
        {
            if (!(_inventory.Size > 0)) _inventory = new Inventory(_inventorySize);
        }

        public void LoadData(GameData data)
        {
            if (GameFlags.MAIN_MENU_ACTIVE) return;
            PersistentInventoryData inventoryData = data.persistentInventoryData.Find(entry => entry.identifier == this.identifier);

            if (inventoryData != null) {
                _inventory = inventoryData.inventory;
            } else {
                _inventory = new Inventory(_inventorySize);
            }
        }

        public void SaveData(ref GameData data)
        {
            if (GameFlags.MAIN_MENU_ACTIVE) return;
            PersistentInventoryData existingInventoryData = data.persistentInventoryData.Find(entry => entry.identifier == this.identifier);
            if (existingInventoryData != null) {
                data.persistentInventoryData.Remove(existingInventoryData);
            }
            data.persistentInventoryData.Add(new PersistentInventoryData(this));
        }
    }
}
