using Flags;
using InventorySystem;
using Models;
using UI.InventorySystem;
using UnityEngine;

namespace Player.Input
{
    public class PlayerWorldInteraction : MonoBehaviour
    {
        public Inventory toolbar;
        public ToolbarInventoryDisplay toolbarDisplay;
        public MouseInventory mouseInventory;
        private int _selectedSlotIndex = -1;

        #region -- Getters, Setters --

        public int SelectedSlotIndex {
            get => _selectedSlotIndex;
            private set {
                _selectedSlotIndex = value;
                UpdateSelectedSlotHighlight();
            }
        }

        #endregion

        private void Update()
        {
            CheckSelectedSlotChanged();
            CheckToolUsed();
        }
        
        private void CheckSelectedSlotChanged()
        {
            if (GameFlags.INVENTORY_OPEN) {
                if (GameFlags.SLOT_EQUIPPED) ClearSelectedSlot();
                return;
            }
            
            if (UserInputFlags.SELECT_SLOT1_KEY_WAS_PRESSED) {
                if (!GameFlags.INVENTORY_SLOT1_EQUIPPED) {
                    SelectedSlotIndex = 0;
                    UpdateGameFlags(true, false, false);
                } else {
                    ClearSelectedSlot();
                }
            } else if (UserInputFlags.SELECT_SLOT2_KEY_WAS_PRESSED) {
                if (!GameFlags.INVENTORY_SLOT2_EQUIPPED) {
                    SelectedSlotIndex = 1;
                    UpdateGameFlags(false, true, false);
                } else {
                    ClearSelectedSlot();
                }
            } else if (UserInputFlags.SELECT_SLOT3_KEY_WAS_PRESSED) {
                if (!GameFlags.INVENTORY_SLOT3_EQUIPPED) {
                    SelectedSlotIndex = 2;
                    UpdateGameFlags(false, false, true);
                } else {
                    ClearSelectedSlot();
                }
            }
        }

        private void UpdateSelectedSlotHighlight()
        {
            toolbarDisplay.EnableHighlightAtIndex(SelectedSlotIndex);
            mouseInventory.AssignedInventorySlot = toolbar.InventorySlots[SelectedSlotIndex];
        }

        private void ClearSelectedSlot()
        {
            UpdateGameFlags(false, false, false);
            toolbarDisplay.DisableHighlight();
            mouseInventory.AssignedInventorySlot = null;
        }

        private void UpdateGameFlags(bool slot1, bool slot2, bool slot3)
        {
            GameFlags.INVENTORY_SLOT1_EQUIPPED = slot1;
            GameFlags.INVENTORY_SLOT2_EQUIPPED = slot2;
            GameFlags.INVENTORY_SLOT3_EQUIPPED = slot3;
        }

        private void CheckToolUsed()
        {
            if (GameFlags.SLOT_EQUIPPED && UserInputFlags.LEFT_MOUSE_BUTTON_WAS_PRESSED) {
                GameObject hitResult = InputManager.GetClickedGameObject();
                if (hitResult == null) return;
                
                Destroyable destroyable = hitResult.GetComponent<Destroyable>();
                InventorySlot selectedSlot = toolbar.InventorySlots[SelectedSlotIndex];
                if (destroyable == null || selectedSlot.Item.Type != destroyable.requiredTool) return;
                
                destroyable.OnClick();
            }
        }
    }
}