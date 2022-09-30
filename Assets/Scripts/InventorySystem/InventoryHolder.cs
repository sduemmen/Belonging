using System;
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

        private void Awake()
        {
            _inventory = new Inventory(_inventorySize);
        }

        public void LoadData(GameData data)
        {
            PersistentInventoryData inventoryData = data.persistentInventoryData.Find(entry => entry.identifier == this.identifier);

            if (inventoryData != null) {
                _inventory = inventoryData.inventory;
            } else {
                _inventory = new Inventory(_inventorySize);
            }
        }

        public void SaveData(ref GameData data)
        {
            data.persistentInventoryData.Add(new PersistentInventoryData(this));
        }
    }
}
