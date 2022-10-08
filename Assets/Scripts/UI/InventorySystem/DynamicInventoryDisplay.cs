using System.Collections.Generic;
using System.Linq;
using InventorySystem;
using UnityEngine;

namespace UI.InventorySystem
{
    public class DynamicInventoryDisplay : InventoryDisplay
    {
        [SerializeField] private UIInventorySlot _UIInventorySlotPrefab;

        #region -- Getters, Setters --

        public Inventory Inventory {
            get => _inventory;
            set {
                _inventory = value;
                ClearSlots();
                if (_inventory != null) _inventory.OnInventorySlotChanged += UpdateSlot;
                InitializeInventorySlots(value);
            }
        }

        #endregion

        protected override void Start() {}

        private void OnDisable()
        {
            _inventory.OnInventorySlotChanged -= UpdateSlot;
        }

        public override void InitializeInventorySlots(Inventory inventory)
        {
            if (inventory == null) return;
            
            _inventorySlotDict = new Dictionary<UIInventorySlot, InventorySlot>();

            for (int i = 0; i < inventory.Size; i++) {
                var uiInventorySlot = Instantiate(_UIInventorySlotPrefab, transform);
                _inventorySlotDict.Add(uiInventorySlot, inventory.InventorySlots[i]);
                uiInventorySlot.AssignedInventorySlot = inventory.InventorySlots[i];
            }
        }

        private void ClearSlots()
        {
            foreach (var item in transform.Cast<Transform>()) {
                Destroy(item.gameObject);
            }

            _inventorySlotDict?.Clear();
        }
    }
}
