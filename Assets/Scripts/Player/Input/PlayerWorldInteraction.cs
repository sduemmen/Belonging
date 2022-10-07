using Flags;
using InventorySystem;
using Models;
using UI.InventorySystem;
using UnityEngine;

namespace Player.Input
{
    public class PlayerWorldInteraction : MonoBehaviour
    {
        public InventoryHolder toolbar;
        public InventoryDisplay toolbarDisplay;
        public MouseInventory mouseInventory;
        private int selectedSlotIndex = -1;
        
        private void Update()
        {
            CheckSelectedSlotChanged();
            CheckToolUsed();
        }
        
        private void CheckSelectedSlotChanged()
        {
            if (UserInputFlags.SELECT_SLOT1_KEY_WAS_PRESSED) {
                if (!GameFlags.INVENTORY_SLOT1_EQUIPPED) {
                    selectedSlotIndex = 0;
                    UpdateGameFlags(true, false, false);
                    UpdateSelectedSlotHighlight();
                } else {
                    ClearSelectedSlot();
                }
            } else if (UserInputFlags.SELECT_SLOT2_KEY_WAS_PRESSED) {
                if (!GameFlags.INVENTORY_SLOT2_EQUIPPED) {
                    selectedSlotIndex = 1;
                    UpdateGameFlags(false, true, false);
                    UpdateSelectedSlotHighlight();
                } else {
                    ClearSelectedSlot();
                }
            } else if (UserInputFlags.SELECT_SLOT3_KEY_WAS_PRESSED) {
                if (!GameFlags.INVENTORY_SLOT3_EQUIPPED) {
                    selectedSlotIndex = 2;
                    UpdateGameFlags(false, false, true);
                    UpdateSelectedSlotHighlight();
                } else {
                    ClearSelectedSlot();
                }
            }
        }

        private void UpdateSelectedSlotHighlight()
        {
            foreach (UIInventorySlot uiInventorySlot in toolbarDisplay.GetUIInventorySlots()) {
                uiInventorySlot.DisableHighlight();
            }
            toolbarDisplay.EnableHighlightAtIndex(selectedSlotIndex);
            mouseInventory.AssignIcon(toolbarDisplay.GetUIInventorySlots()[selectedSlotIndex].GetAssignedInventorySlot().GetItem().GetIcon());
            mouseInventory.Show();
        }

        private void ClearSelectedSlot()
        {
            UpdateGameFlags(false, false, false);
            toolbarDisplay.DisableHighlightAtIndex(selectedSlotIndex);
            selectedSlotIndex = -1;
            mouseInventory.ClearIcon();
            mouseInventory.Hide();
        }

        private void UpdateGameFlags(bool slot1, bool slot2, bool slot3)
        {
            GameFlags.INVENTORY_SLOT1_EQUIPPED = slot1;
            GameFlags.INVENTORY_SLOT2_EQUIPPED = slot2;
            GameFlags.INVENTORY_SLOT3_EQUIPPED = slot3;
        }

        private void CheckToolUsed()
        {
            if (selectedSlotIndex >= 0 && UserInputFlags.LEFT_MOUSE_BUTTON_WAS_PRESSED) {
                GameObject hitResult = InputManager.GetClickedGameObject();
                Destroyable destroyable = hitResult.GetComponent<Destroyable>();
                InventorySlot selectedSlot = toolbarDisplay.GetUIInventorySlots()[selectedSlotIndex].GetAssignedInventorySlot();
                
                if (destroyable == null || selectedSlot.GetItem().GetItemType() != destroyable.requiredTool) return;
                
                destroyable.OnClick();
            }
        }
    }
}