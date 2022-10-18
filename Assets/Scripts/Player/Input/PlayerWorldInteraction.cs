using Environment;
using Flags;
using InventorySystem;
using InventorySystem.Items;
using InventorySystem.UI;
using UI.InventorySystem;
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

        #region -- Getters, Setters --

        public int SelectedSlotIndex {
            get => _selectedSlotIndex;
            private set {
                _selectedSlotIndex = value;
                UpdateSelectedSlotHighlight();
            }
        }

        #endregion

        private void Awake()
        {
            _camera = Camera.main;
            _playerWorldBuilding = GetComponent<PlayerWorldBuilding>();
            player = transform;
        }
        
        private void CheckSelectedSlotChanged()
        {
            if (GameFlags.INVENTORY_OPEN) {
                if (GameFlags.SLOT_EQUIPPED) ClearSelectedSlot();
                return;
            }
        }

        public void OnSelectSlot1()
        {
            if (!GameFlags.AXE_EQUIPPED) {
                SelectedSlotIndex = 0;
                UpdateGameFlags(true, false, false);
                ClearPreviewGameObject();
            } else {
                ClearSelectedSlot();
            }
        }
        
        public void OnSelectSlot2()
        {
            if (!GameFlags.PICKAXE_EQUIPPED) {
                SelectedSlotIndex = 1;
                UpdateGameFlags(false, true, false);
                ClearPreviewGameObject();
            } else {
                ClearSelectedSlot();
            }
        }
        
        public void OnSelectSlot3()
        {
            if (!GameFlags.HAMMER_EQUIPPED) {
                SelectedSlotIndex = 2;
                UpdateGameFlags(false, false, true);
                if (_playerWorldBuilding.previewGameObject == null) {
                    _playerWorldBuilding.previewGameObject = Instantiate(_playerWorldBuilding.selectedSegment, Vector3.down, Quaternion.identity);
                }
            } else {
                ClearSelectedSlot();
            }
        }

        public void OnToolUsed()
        {
            if (GameFlags.AXE_EQUIPPED || GameFlags.PICKAXE_EQUIPPED) {
                OnDestroyableClicked();
            }
            
            if (GameFlags.HAMMER_EQUIPPED && GetMouseRayHit(_playerWorldBuilding.BuildModeLayerMask, out RaycastHit raycastHit, _playerWorldBuilding.MaxInteractionDistance)) {
                _playerWorldBuilding.TryPlaceSegment(new Vector3(raycastHit.point.x, 0, raycastHit.point.z));
            }
        }

        private void UpdateSelectedSlotHighlight()
        {
            toolbarDisplay.EnableHighlightAtIndex(SelectedSlotIndex);
            mouseInventory.Initialize(toolbar.InventorySlots[SelectedSlotIndex]);
        }

        public void ClearSelectedSlot()
        {
            UpdateGameFlags(false, false, false);
            toolbarDisplay.DisableHighlight();
            mouseInventory.Initialize(null);
            ClearPreviewGameObject();
        }

        private void ClearPreviewGameObject()
        {
            if (_playerWorldBuilding.previewGameObject != null) Destroy(_playerWorldBuilding.previewGameObject);
            _playerWorldBuilding.previewGameObject = null;
        }

        private void UpdateGameFlags(bool slot1, bool slot2, bool slot3)
        {
            GameFlags.AXE_EQUIPPED = slot1;
            GameFlags.PICKAXE_EQUIPPED = slot2;
            GameFlags.HAMMER_EQUIPPED = slot3;
        }

        private void OnDestroyableClicked()
        {
            bool objectHit = GetMouseRayHit(destroyablesLayerMask, out RaycastHit hitResult, maxInteractionDistance);
            if (!objectHit || hitResult.transform.gameObject == null) return;
            
            GameObject hitGameObject = hitResult.transform.gameObject;
            if ((player.position - hitResult.point).magnitude > maxInteractionDistance) return;
            
            Destroyable destroyable = hitGameObject.GetComponent<Destroyable>();
            ToolItemObject selectedTool = (ToolItemObject)toolbar.InventorySlots[SelectedSlotIndex].Item;
            if (selectedTool == null) return;
            
            if (destroyable == null || selectedTool != destroyable.requiredTool) return;
            destroyable.OnClick(player);
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