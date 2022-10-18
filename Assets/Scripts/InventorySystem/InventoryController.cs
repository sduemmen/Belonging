using InventorySystem.Items;
using InventorySystem.UI;
using UnityEngine;

namespace InventorySystem
{
    public class InventoryController : MonoBehaviour
    {
        [SerializeField] private Inventory _inventory;
        [SerializeField] private InventoryDisplay _inventoryDisplay;
        [SerializeField] private MouseUIInventorySlot _mouseUISlot;

        public Inventory Inventory => _inventory;
        public InventoryDisplay InventoryDisplay => _inventoryDisplay;

        private void Awake()
        {
            if (_inventory == null) Debug.LogError("No inventory set in InventoryController");
            if (_inventoryDisplay == null) Debug.LogError("No inventoryDisplay set in InventoryController");

            for (int i = 0; i < _inventory.Size; i++) {
                UIInventorySlot uiSlot = _inventoryDisplay.AddSlot();
                uiSlot.Initialize(_inventory.InventorySlots[i]);
            }

            _inventory.OnSlotChanged += UpdateUISlot;
            _inventoryDisplay.OnSlotClicked += InteractWithSlot;
        }

        private void OnDestroy()
        {
            _inventory.OnSlotChanged -= UpdateUISlot;
            _inventoryDisplay.OnSlotClicked -= InteractWithSlot;
        }

        public void InitializeUISlots()
        {
            for (int i = 0; i < _inventory.Size; i++) {
                _inventoryDisplay.InventorySlots[i].Initialize(_inventory.InventorySlots[i]);
            }
        }

        public void UpdateUISlot(InventorySlot newSlot)
        {
            UIInventorySlot uiSlot = _inventoryDisplay.GetSlotAtIndex(newSlot.Index);
            uiSlot.Initialize(newSlot);
        }

        private void InteractWithSlot(UIInventorySlot clickedUISlot)
        {
            // get clicked slot index and corresponding inventory slot
            int clickedSlotIndex = clickedUISlot.Index;
            InventorySlot clickedSlot = _inventory.GetSlotAtIndex(clickedSlotIndex);

            // get current state of clicked slot and mouse slot
            ItemObject clickedSlotItem = clickedSlot.Item;
            int clickedSlotStackSize = clickedSlot.StackSize;
            ItemObject mouseSlotItem = _mouseUISlot.assignedInventorySlot?.Item;
            int mouseSlotStackSize = _mouseUISlot.assignedInventorySlot?.StackSize ?? -1;
            
            // check if slots are empty or equal
            bool clickedSlotIsEmpty = clickedSlot.IsEmpty();
            bool mouseSlotIsEmpty = _mouseUISlot.assignedInventorySlot?.IsEmpty() ?? true;
            bool slotContentsAreEqual = clickedSlotItem == mouseSlotItem;

            // take from clicked slot
            if (!clickedSlotIsEmpty && mouseSlotIsEmpty) {
                _inventory.InventorySlots[clickedSlotIndex].ClearSlot();
                _inventoryDisplay.InventorySlots[clickedSlotIndex].ClearSlot();
                
                _mouseUISlot.assignedInventorySlot = new InventorySlot(clickedSlotItem, clickedSlotStackSize, clickedSlotIndex);
                _mouseUISlot.Initialize(_mouseUISlot.assignedInventorySlot);
            }

            // place on clicked slot
            if (clickedSlotIsEmpty && !mouseSlotIsEmpty) {
                _inventory.InventorySlots[clickedSlotIndex] = new InventorySlot(mouseSlotItem, mouseSlotStackSize, clickedSlotIndex);
                _inventoryDisplay.InventorySlots[clickedSlotIndex].Initialize(_inventory.InventorySlots[clickedSlotIndex]);
                
                _mouseUISlot.assignedInventorySlot.ClearSlot();
                _mouseUISlot.Initialize(_mouseUISlot.assignedInventorySlot);
            }
            
            if (!clickedSlotIsEmpty && !mouseSlotIsEmpty) {
                
                // fill up slot
                if (slotContentsAreEqual) {
                    _inventory.InventorySlots[clickedSlotIndex].AddToStack(mouseSlotStackSize, out int remainingAmount);
                    _inventoryDisplay.InventorySlots[clickedSlotIndex].Initialize(_inventory.InventorySlots[clickedSlotIndex]);
                    
                    if (remainingAmount > 0) 
                        _mouseUISlot.assignedInventorySlot.StackSize = remainingAmount;
                    else 
                        _mouseUISlot.assignedInventorySlot.ClearSlot();
                    
                    _mouseUISlot.Initialize(_mouseUISlot.assignedInventorySlot);
                } 
                // swap slots
                else {
                    (_inventory.InventorySlots[clickedSlotIndex], _mouseUISlot.assignedInventorySlot) = (_mouseUISlot.assignedInventorySlot, _inventory.InventorySlots[clickedSlotIndex]);

                    _inventoryDisplay.InventorySlots[clickedSlotIndex].Initialize(_inventory.InventorySlots[clickedSlotIndex]);
                    _mouseUISlot.Initialize(_mouseUISlot.assignedInventorySlot);
                }
            }
        }
    }
}