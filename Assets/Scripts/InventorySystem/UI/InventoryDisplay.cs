using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace InventorySystem.UI
{
    public class InventoryDisplay : MonoBehaviour
    {
        [SerializeField] protected List<UIInventorySlot> _inventorySlots;
        [SerializeField] protected UIInventorySlot _uiSlotPrefab;
        public UnityAction<UIInventorySlot> OnSlotClicked;

        public List<UIInventorySlot> InventorySlots => _inventorySlots;

        public virtual UIInventorySlot AddSlot()
        {
            UIInventorySlot uiSlot = Instantiate(_uiSlotPrefab, transform);
            uiSlot.Index = _inventorySlots.Count;
            uiSlot.parentDisplay = this;
            _inventorySlots.Add(uiSlot);
            return uiSlot;
        }

        public UIInventorySlot GetSlotAtIndex(int index)
        {
            if (index < 0 || index >= _inventorySlots.Count) return null;
            return _inventorySlots[index];
        }

        public void InitializeSlotAtIndex(int index, InventorySlot inventorySlot)
        {
            UIInventorySlot uiSlot = GetSlotAtIndex(index);
            
            if (uiSlot == null) return;
            
            uiSlot.Initialize(inventorySlot);
        }
    }
}