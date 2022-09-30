using InventorySystem;
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
                Vector2 mousePosition = Mouse.current.position.ReadValue();
                transform.position = mousePosition;

                if (Input.GetKeyDown(KeyCode.Mouse0) && !InputManager.IsPointerOverUIObject()) {
                    for (int i = 0; i < _assignedInventorySlot.GetStackSize(); i++) {
                        GameObject item = Instantiate(_assignedInventorySlot.GetItem().GetPrefab(), playerPosition.position + Vector3.up, Quaternion.Euler(Vector3.zero));
                        item.GetComponent<Pickupable>().SetPickUpDelay(4);
                    }
                    
                    this.ClearSlot();
                    this.Hide();
                    
                    // Ray ray = _camera.ScreenPointToRay(mousePosition);
                    // if (Physics.Raycast(ray, out RaycastHit hit, 100, _layerMask)) {
                    //     Instantiate(_assignedInventorySlot.GetItem().GetPrefab(), hit.point + Vector3.up, Quaternion.Euler(Vector3.zero));
                    //     ClearSlot();
                    // }
                }
            }
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
