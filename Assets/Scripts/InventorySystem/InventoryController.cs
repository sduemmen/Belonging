using System.Collections.Generic;
using InventorySystem.Items;
using InventorySystem.UI;
using SaveSystem.Data;
using Sirenix.OdinInspector;
using UI;
using UnityEngine;
using UnityEngine.Events;

namespace InventorySystem
{
    public class InventoryController : Controller, IDisplayContext
    {
        private static InventoryController instance;
        public static InventoryController Instance {
            get {
                if (instance == null)
                {
                    instance = (InventoryController)FindObjectOfType(typeof(InventoryController));
                }

                return instance;
            }
        }

        [SerializeField, TitleGroup("General")] private Inventory _playerInventory;
        
        [SerializeField, TitleGroup("UI")] private GameObject _uiInventoryDisplayContext;
        [SerializeField, TitleGroup("UI")] private Transform _uiInventoryTarget;
        [SerializeField, TitleGroup("UI")] private UIInventorySlot _uiInventorySlotPrefab;
        private List<UIInventorySlot> m_uiInventorySlots;
        private bool m_displayContextActive;
        
        public UnityAction<UIInventorySlot> OnSlotClickedDelegate;
        
        public Inventory PlayerInventory => _playerInventory;
        public bool DisplayContextActive => m_displayContextActive;
        

        [Button("Load Manually"), TitleGroup("Debugging")]
        protected override void OnLoadCompleted()
        {
            m_uiInventorySlots = new List<UIInventorySlot>();
            
            for (int i = 0; i < _playerInventory.Size; i++)
            {
                UIInventorySlot slot = Instantiate(_uiInventorySlotPrefab, _uiInventoryTarget, false);
                slot.Initialize(_playerInventory.InventorySlots[i], true, i);
                m_uiInventorySlots.Add(slot);
            }

            _playerInventory.OnSlotChangedDelegate += OnSlotChanged;
            OnSlotClickedDelegate += OnSlotClicked;
        }

        private void Update()
        {
            CheckInput();
        }

        private void CheckInput()
        {
            if (Flags.GAME_PAUSED)
            {
                return;
            }
            
            if (InputSystem.GetKeyDown(InputSystem.KeyBinds.Toggle_Inventory))
            {
                if (m_displayContextActive)
                {
                    HideDisplayContext();
                }
                else
                {
                    ShowDisplayContext();
                }
            }
            else if (InputSystem.GetKeysDown(InputSystem.KeyBinds.Toggle_Quest_Display, InputSystem.KeyBinds.EquipUnequip_Axe, InputSystem.KeyBinds.EquipUnequip_Pickaxe, InputSystem.KeyBinds.Open_Build_Menu))
            {
                HideDisplayContext();
            }
            else if (InputSystem.GetKeyDown(InputSystem.KeyBinds.Pause_Game) && m_displayContextActive)
            {
                HideDisplayContext();
            }
        }

        private void OnSlotChanged(InventorySlot slot)
        {
            UIInventorySlot uiSlot = m_uiInventorySlots[slot.Index];
            uiSlot.Initialize(slot);
        }

        private void OnSlotClicked(UIInventorySlot clickedUISlot)
        {
            InventorySlot newMouseInventorySlot = new InventorySlot();
            InventorySlot newUIInventorySlot = new InventorySlot();
            
            // get clicked slot index and corresponding inventory slot
            int clickedSlotIndex = clickedUISlot.Index;
            InventorySlot clickedSlot = _playerInventory.GetSlotAtIndex(clickedSlotIndex);

            // get current state of clicked slot and mouse slot
            ItemObject clickedSlotItem = clickedSlot.Item;
            int clickedSlotStackSize = clickedSlot.StackSize;
            ItemObject mouseSlotItem = MouseInventory.Instance.assignedInventorySlot?.Item;
            int mouseSlotStackSize = MouseInventory.Instance.assignedInventorySlot?.StackSize ?? -1;

            // check if slots are empty or equal
            bool clickedSlotIsEmpty = clickedSlot.IsEmpty();
            bool mouseSlotIsEmpty = MouseInventory.Instance.assignedInventorySlot?.IsEmpty() ?? true;
            bool slotContentsAreEqual = clickedSlotItem == mouseSlotItem;

            if (!clickedSlotIsEmpty && mouseSlotIsEmpty)
            {
                // take from clicked slot
                _playerInventory.InventorySlots[clickedSlotIndex].ClearSlot();
                
                newMouseInventorySlot = new InventorySlot(clickedSlotItem, clickedSlotStackSize, clickedSlotIndex);
                newUIInventorySlot = null;
            }
            else if (clickedSlotIsEmpty && !mouseSlotIsEmpty)
            {
                // place on clicked slot
                _playerInventory.InventorySlots[clickedSlotIndex] = new InventorySlot(mouseSlotItem, mouseSlotStackSize, clickedSlotIndex);
                
                newMouseInventorySlot = new InventorySlot();
                newUIInventorySlot = new InventorySlot(mouseSlotItem, mouseSlotStackSize, clickedSlotIndex);
            }
            else if (!clickedSlotIsEmpty)   // both slots contain items
            {
                if (slotContentsAreEqual)
                {
                    // fill up slot
                    _playerInventory.InventorySlots[clickedSlotIndex].AddToStack(mouseSlotStackSize, out int remainingAmount);

                    newMouseInventorySlot = remainingAmount > 0 ? new InventorySlot(mouseSlotItem, remainingAmount) : new InventorySlot();
                    newUIInventorySlot = _playerInventory.InventorySlots[clickedSlotIndex];
                }
                else
                {
                    // swap slots
                    _playerInventory.InventorySlots[clickedSlotIndex] = new InventorySlot(mouseSlotItem, mouseSlotStackSize, clickedSlotIndex);

                    newMouseInventorySlot = new InventorySlot(clickedSlotItem, clickedSlotStackSize);
                    newUIInventorySlot = new InventorySlot(mouseSlotItem, mouseSlotStackSize, clickedSlotIndex);
                }
            }
            
            MouseInventory.Instance.SetAssignedInventorySlot(newMouseInventorySlot);
            clickedUISlot.Initialize(newUIInventorySlot);
        }
        
        public void ShowDisplayContext()
        {
            m_displayContextActive = true;
            _uiInventoryDisplayContext.SetActive(true);
        }

        public void HideDisplayContext()
        {
            m_displayContextActive = false;
            _uiInventoryDisplayContext.SetActive(false);
            MouseInventory.Instance.OnCloseInventory();
            MouseTooltip.Instance.Hide();
        }
        
        public override void LoadData(GameData data)
        {
            PersistentInventoryData inventoryData = data.persistentInventoryData.Find(entry => entry.identifier == _playerInventory.identifier);

            if (inventoryData != null)
            {
                // load existing inventory data
                _playerInventory.InventorySlots = inventoryData.inventorySlots;
            }
            else
            {
                // initialize new inventory if no data exists
                _playerInventory.Initialize();
                _playerInventory.SetupSlotIndices();
            }
            
            base.LoadData(data);
        }

        public override void SaveData(ref GameData data)
        {
            PersistentInventoryData existingInventoryData = data.persistentInventoryData.Find(entry => entry.identifier == _playerInventory.identifier);
            
            if (existingInventoryData != null)
            {
                data.persistentInventoryData.Remove(existingInventoryData);
            }
            
            data.persistentInventoryData.Add(new PersistentInventoryData(_playerInventory));
        }
    }
}