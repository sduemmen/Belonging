using System.Collections.Generic;
using System.Linq;
using Environment;
using Flags;
using InventorySystem.Items;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

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

                if (UserInputFlags.LEFT_MOUSE_BUTTON_WAS_PRESSED && !IsPointerOverUIObject()) {
                    for (int i = 0; i < assignedInventorySlot.StackSize; i++) {
                        MaterialItemObject materialItem = (MaterialItemObject)assignedInventorySlot.Item;
                        GameObject item = Instantiate(materialItem.prefab, playerPosition.position + new Vector3(Random.Range(-.5f, .5f), Random.Range(.2f, .5f), Random.Range(-.5f, .5f)), Quaternion.identity);
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
            if (assignedInventorySlot != null && !assignedInventorySlot.IsEmpty()) {
                for (int i = 0; i < assignedInventorySlot.StackSize; i++) {
                    MaterialItemObject materialItem = (MaterialItemObject)assignedInventorySlot.Item;
                    GameObject item = Instantiate(materialItem.prefab, playerPosition.position + new Vector3(Random.Range(-.5f, .5f), Random.Range(.2f, .5f), Random.Range(-.5f, .5f)), Quaternion.identity);
                    Pickupable pickupable = item.GetComponent<Pickupable>();
                    pickupable.PickupDelay = 2;
                }
                
                ClearSlot();
            }
        }
        
        public static bool IsPointerOverUIObject()
        {
            PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current) {
                position = Mouse.current.position.ReadValue()
            };
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventDataCurrentPosition, results);

            return results.Where(result => result.gameObject.layer == LayerMask.NameToLayer("UI")).ToArray().Length > 0;
        }
    }
}
