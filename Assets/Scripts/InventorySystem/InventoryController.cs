using Events.Events;
using Flags;
using InventorySystem.Items;
using InventorySystem.UI;
using SaveSystem;
using SaveSystem.Data;
using UnityEngine;

namespace InventorySystem
{
    public class InventoryController : MonoBehaviour, IDataPersistence
    {
        [SerializeField] private Inventory _inventory;
        [SerializeField] protected InventoryDisplay _inventoryDisplay;
        [SerializeField] private MouseUIInventorySlot _mouseUISlot;

        public Inventory Inventory => _inventory;
        public InventoryDisplay InventoryDisplay => _inventoryDisplay;

        private void Awake()
        {
            if (_inventory != null) _inventory.Awake();
        }

        protected virtual void Start()
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

        protected virtual void OnDestroy()
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

        protected virtual void InteractWithSlot(UIInventorySlot clickedUISlot)
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

        public void LoadData(GameData data)
        {
            if (GameFlags.MAIN_MENU_ACTIVE) return;
            if (_inventory == null || _inventory.IsStatic) return;
            
            PersistentInventoryData inventoryData = data.persistentInventoryData.Find(entry => entry.identifier == this._inventory.identifier);
            
            if (inventoryData != null && inventoryData.inventorySlots.Count == _inventory.Size) {
                _inventory.InventorySlots = inventoryData.inventorySlots;
            } else {
                _inventory.Awake();
                _inventory.SetupSlotIndices();
            }
        }

        public void SaveData(ref GameData data)
        {
            if (GameFlags.MAIN_MENU_ACTIVE) return;
            if (_inventory == null || _inventory.IsStatic) return;
            
            PersistentInventoryData existingInventoryData = data.persistentInventoryData.Find(entry => entry.identifier == this._inventory.identifier);
            if (existingInventoryData != null) {
                data.persistentInventoryData.Remove(existingInventoryData);
            }
            data.persistentInventoryData.Add(new PersistentInventoryData(_inventory));
        }
    }
}