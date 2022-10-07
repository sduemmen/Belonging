using Flags;
using InventorySystem;
using Items;
using Player.Input;
using UnityEngine;
using UnityEngine.InputSystem;

namespace UI.InventorySystem
{
    public class MouseInventory : UIInventorySlotBase
    {
        [SerializeField] private Transform playerPosition;
        private Camera _camera;
        private Vector3 worldPosition;
        [SerializeField] private LayerMask _layerMask;
        
        private void Awake()
        {
            _camera = Camera.main;
            this.gameObject.SetActive(true);
            this.Hide();
            this.Refresh();
            InventoryHolder.OnDynamicInventoryDisplayContextClosed += OnCloseInventory;
        }

        private void OnDestroy()
        {
            InventoryHolder.OnDynamicInventoryDisplayContextClosed -= OnCloseInventory;
        }

        private void Update()
        {
            if (!_assignedInventorySlot.IsEmpty()) { 
                FollowCursor();

                if (UserInputFlags.LEFT_MOUSE_BUTTON_WAS_PRESSED && !InputManager.IsPointerOverUIObject()) {
                    for (int i = 0; i < _assignedInventorySlot.GetStackSize(); i++) {
                        GameObject item = Instantiate(_assignedInventorySlot.GetItem().GetPrefab(), playerPosition.position + Vector3.up, Quaternion.Euler(Vector3.zero));
                        item.GetComponent<Pickupable>().SetPickUpDelay(4);
                    }
                    
                    this.ClearSlot();
                    this.Hide();
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

        public void Show()
        {
            _icon.gameObject.SetActive(true);
            _stackSizeLabel.gameObject.SetActive(true);
        }
        
        public void Hide()
        {
            _icon.gameObject.SetActive(false);
            _stackSizeLabel.gameObject.SetActive(false);
        }

        public void OnCloseInventory()
        {
            if (!GetAssignedInventorySlot().IsEmpty()) {
                for (int i = 0; i < _assignedInventorySlot.GetStackSize(); i++) {
                    GameObject item = Instantiate(_assignedInventorySlot.GetItem().GetPrefab(), playerPosition.position + Vector3.up, Quaternion.Euler(Vector3.zero));
                    item.GetComponent<Pickupable>().SetPickUpDelay(2);
                }
                this.ClearSlot();
                this.Hide();
            }
        }
    }
}
