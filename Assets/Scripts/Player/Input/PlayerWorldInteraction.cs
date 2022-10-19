using Environment;
using Flags;
using InventorySystem;
using InventorySystem.Items;
using InventorySystem.UI;
using UnityEngine;

namespace Player.Input
{
    public class PlayerWorldInteraction : MonoBehaviour
    {
        public Inventory toolbar;
        public ToolbarInventoryDisplay toolbarDisplay;
        public MouseUIInventorySlot mouseInventory;
        public Transform player;
        public World World;
        private PlayerWorldBuilding _playerWorldBuilding;
        public float maxInteractionDistance;
        private int _selectedSlotIndex = -1;
        private Camera _camera;
        public LayerMask destroyablesLayerMask;

        private void Awake()
        {
            _camera = Camera.main;
            _playerWorldBuilding = GetComponent<PlayerWorldBuilding>();
            player = transform;
        }

        public void EquipSlot(int index)
        {
            ResetGameObjects();
            _selectedSlotIndex = index;
            if (index >= 0)
                UpdateSelectedSlotHighlight();
        }
        
        private void ResetGameObjects()
        {
            toolbarDisplay.DisableHighlight();
            mouseInventory.Initialize(null);
            if (_playerWorldBuilding.previewGameObject != null) Destroy(_playerWorldBuilding.previewGameObject);
            _playerWorldBuilding.previewGameObject = null;
        }

        private void UpdateSelectedSlotHighlight()
        {
            toolbarDisplay.EnableHighlightAtIndex(_selectedSlotIndex);
            mouseInventory.Initialize(toolbar.InventorySlots[_selectedSlotIndex]);
        }

        public void OnToolUsed()
        {
            if (GameFlags.AXE_EQUIPPED || GameFlags.PICKAXE_EQUIPPED) {
                OnDestroyableClicked();
            }
            
            if (GameFlags.HAMMER_EQUIPPED && GameFlags.BUILD_MENU_CLOSED && GetMouseRayHit(_playerWorldBuilding.BuildModeLayerMask, out RaycastHit raycastHit, _playerWorldBuilding.MaxInteractionDistance)) {
                _playerWorldBuilding.TryPlaceSegment(new Vector3(raycastHit.point.x, 0, raycastHit.point.z));
            }
        }

        private void OnDestroyableClicked()
        {
            bool objectHit = GetMouseRayHit(destroyablesLayerMask, out RaycastHit hitResult, maxInteractionDistance);
            if (!objectHit || hitResult.transform.gameObject == null) return;
            
            GameObject hitGameObject = hitResult.transform.gameObject;
            if ((player.position - hitResult.point).magnitude > maxInteractionDistance) return;
            
            Destroyable destroyable = hitGameObject.GetComponent<Destroyable>();
            ToolItemObject selectedTool = (ToolItemObject)toolbar.InventorySlots[_selectedSlotIndex].Item;
            if (selectedTool == null) return;
            
            if (destroyable == null || selectedTool != destroyable.requiredTool) return;
            destroyable.OnClick(player, hitResult);
        }
        
        private bool GetMouseRayHit(out RaycastHit raycastHit)
        {
            Ray ray = _camera.ViewportPointToRay(new Vector3(UnityEngine.Input.mousePosition.x / Screen.width, UnityEngine.Input.mousePosition.y / Screen.height, 0));
            return Physics.Raycast(ray, out raycastHit);
        }
        
        private bool GetMouseRayHit(LayerMask layerMask, out RaycastHit raycastHit, float distance)
        {
            Ray ray = _camera.ViewportPointToRay(new Vector3(UnityEngine.Input.mousePosition.x / Screen.width, UnityEngine.Input.mousePosition.y / Screen.height, 0));
            return Physics.Raycast(ray, out raycastHit, distance, layerMask);
        }
    }
}