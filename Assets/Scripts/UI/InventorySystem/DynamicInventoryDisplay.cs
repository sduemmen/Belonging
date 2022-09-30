using System;
using System.Collections.Generic;
using System.Linq;
using InventorySystem;
using UnityEngine;

namespace UI.InventorySystem
{
    public class DynamicInventoryDisplay : InventoryDisplay
    {
        [SerializeField] private UIInventorySlot _UIInventorySlotPrefab;

        protected override void Start() {}

        private void OnDisable()
        {
            _inventory.OnInventorySlotChanged -= UpdateSlot;
        }

        public void RefreshDynamicInventory(Inventory inventory)
        {
            ClearSlots();
            _inventory = inventory;
            if (_inventory != null) _inventory.OnInventorySlotChanged += UpdateSlot;
            InitializeInventorySlots(inventory);
        }

        public override void InitializeInventorySlots(Inventory inventory)
        {
            // Debug.Log($"initialize {inventory.GetInventorySize()} slots");
            _inventorySlotDict = new Dictionary<UIInventorySlot, InventorySlot>();
            
            if (inventory == null) return;

            for (int i = 0; i < inventory.GetInventorySize(); i++) {
                var uiInventorySlot = Instantiate(_UIInventorySlotPrefab, transform);
                _inventorySlotDict.Add(uiInventorySlot, inventory.GetInventorySlots()[i]);
                uiInventorySlot.Initialize(inventory.GetInventorySlots()[i]);
                uiInventorySlot.Refresh();
            }
        }

        private void ClearSlots()
        {
            // Debug.Log("clear slots");
            foreach (var item in transform.Cast<Transform>()) {
                Destroy(item.gameObject);
            }

            _inventorySlotDict?.Clear();
        }
    }
}
