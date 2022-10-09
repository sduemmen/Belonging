using Flags;
using InventorySystem;
using Items;
using Player.Input;
using UnityEngine;
using UnityEngine.InputSystem;

namespace UI.InventorySystem
{
    public class MouseInventory : UIInventorySlot
    {
        [SerializeField] private Transform playerPosition;
        
        private void Awake()
        {
            AssignedInventorySlot = null;
            InventoryHolder.OnDynamicInventoryDisplayContextClosed += OnCloseInventory;
        }

        private void OnDestroy()
        {
            InventoryHolder.OnDynamicInventoryDisplayContextClosed -= OnCloseInventory;
        }

        private void Update()
        {
            if (!AssignedInventorySlot.IsEmpty() && !GameFlags.SLOT_EQUIPPED) { 
                FollowCursor();

                if (UserInputFlags.LEFT_MOUSE_BUTTON_WAS_PRESSED && !InputManager.IsPointerOverUIObject()) {
                    for (int i = 0; i < AssignedInventorySlot.StackSize; i++) {
                        GameObject item = Instantiate(AssignedInventorySlot.Item.Prefab, playerPosition.position + Vector3.up, Quaternion.Euler(Vector3.zero));
                        Pickupable pickupable = item.GetComponent<Pickupable>();
                        pickupable.PickupDelay = 4;
                    }

                    AssignedInventorySlot = null;
                }
            } else if (GameFlags.SLOT_EQUIPPED) {
                FollowCursor();
            }
        }

        private void FollowCursor()
        {
            Vector2 mousePosition = Mouse.current.position.ReadValue();
            transform.position = mousePosition;
        }

        public void OnCloseInventory()
        {
            if (!AssignedInventorySlot.IsEmpty() && !GameFlags.SLOT_EQUIPPED) {
                for (int i = 0; i < AssignedInventorySlot.StackSize; i++) {
                    GameObject item = Instantiate(AssignedInventorySlot.Item.Prefab, playerPosition.position + Vector3.up, Quaternion.Euler(Vector3.zero));
                    Pickupable pickupable = item.GetComponent<Pickupable>();
                    pickupable.PickupDelay = 2;
                }
                
                AssignedInventorySlot = null;
            }
        }
    }
}
