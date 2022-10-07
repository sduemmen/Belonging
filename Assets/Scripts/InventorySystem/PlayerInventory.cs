using Flags;
using UnityEngine;

namespace InventorySystem
{
    public class PlayerInventory : InventoryHolder
    {
        public void Interact()
        {
            GameFlags.INVENTORY_OPEN = true;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            OnDynamicInventoryDisplayContextRequested?.Invoke(_inventory);
        }

        public void EndInteraction()
        {
            GameFlags.INVENTORY_OPEN = false;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            OnDynamicInventoryDisplayContextClosed?.Invoke();
        }

        private void Update()
        {
            if (UserInputFlags.OPEN_INVENTORY_KEY_WAS_PRESSED && !GameFlags.INVENTORY_OPEN) Interact();
            else if (UserInputFlags.CLOSE_INVENTORY_KEY_WAS_PRESSED && GameFlags.INVENTORY_OPEN) EndInteraction();
        }
    }
}