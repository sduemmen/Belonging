using System;
using Environment;
using Flags;
using InventorySystem.Items;
using Player.Input;
using UnityEngine;
using UnityEngine.InputSystem;

namespace InventorySystem.UI
{
    public class MouseUIInventorySlot : UIInventorySlot
    {
        [SerializeField] private Transform playerPosition;
        [SerializeField] public InventorySlot assignedInventorySlot;

        private void Awake()
        {
            assignedInventorySlot = new InventorySlot();
            clickable = false;
            ClearSlot();
        }

        private void Update()
        {
            if (assignedInventorySlot != null && !assignedInventorySlot.IsEmpty() && !GameFlags.SLOT_EQUIPPED) { 
                FollowCursor();

                if (UserInputFlags.LEFT_MOUSE_BUTTON_WAS_PRESSED && !InputManager.IsPointerOverUIObject()) {
                    for (int i = 0; i < assignedInventorySlot.StackSize; i++) {
                        MaterialItemObject materialItem = (MaterialItemObject)assignedInventorySlot.Item;
                        GameObject item = Instantiate(materialItem.prefab, playerPosition.position + Vector3.up, Quaternion.identity);
                        Pickupable pickupable = item.GetComponent<Pickupable>();
                        pickupable.PickupDelay = 4;
                    }

                    ClearSlot();
                }
            } else if (GameFlags.SLOT_EQUIPPED) {
                FollowCursor();
            }
        }

        public override void Initialize(InventorySlot inventorySlot)
        {
            base.Initialize(inventorySlot);
            assignedInventorySlot = inventorySlot;
        }

        private void FollowCursor()
        {
            Vector2 mousePosition = Mouse.current.position.ReadValue();
            transform.position = mousePosition;
        }

        public void OnCloseInventory()
        {
            if (!assignedInventorySlot.IsEmpty() && !GameFlags.SLOT_EQUIPPED) {
                for (int i = 0; i < assignedInventorySlot.StackSize; i++) {
                    MaterialItemObject materialItem = (MaterialItemObject)assignedInventorySlot.Item;
                    GameObject item = Instantiate(materialItem.prefab, playerPosition.position + Vector3.up, Quaternion.identity);
                    Pickupable pickupable = item.GetComponent<Pickupable>();
                    pickupable.PickupDelay = 2;
                }
                
                ClearSlot();
            }
        }
    }
}
