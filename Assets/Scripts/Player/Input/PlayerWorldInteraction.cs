using BuildSystem;
using Flags;
using InventorySystem;
using InventorySystem.Items;
using InventorySystem.UI;
using UnityEngine;
using Utility;
using World;

namespace Player.Input
{
    public class PlayerWorldInteraction : MonoBehaviour
    {
        [SerializeField] private Transform _player;
        [SerializeField] private float _maxInteractionDistance;
        [SerializeField] private LayerMask _destroyablesLayerMask;
        private Camera _camera;
        private int _selectedSlotIndex = -1;
        private Destroyable _outlinedDestroyable;
        private SegmentPreview _outlinedSegment;

        private void Awake()
        {
            _camera = Camera.main;
            _player = transform;
        }

        private void Update()
        {
            if ((GameFlags.AXE_EQUIPPED || GameFlags.PICKAXE_EQUIPPED) && Raycast.GetMouseRayHit(_camera, _destroyablesLayerMask, out RaycastHit hit, 40))
            {
                Destroyable destroyableHoveredOver = hit.transform.GetComponentInParent<Destroyable>();
                if (destroyableHoveredOver == null) return;

                bool equippedToolMatchingRequiredTool = (destroyableHoveredOver.requiredTool.DisplayName == "Axe" && GameFlags.AXE_EQUIPPED) ||
                                                        (destroyableHoveredOver.requiredTool.DisplayName == "Pickaxe" && GameFlags.PICKAXE_EQUIPPED);
                
                if (equippedToolMatchingRequiredTool && destroyableHoveredOver != _outlinedDestroyable)
                {
                    if (destroyableHoveredOver.TryGetComponent(out SegmentPreview segmentPreview))
                    {
                        if (_outlinedSegment != null)
                        {
                            _outlinedSegment.canBeDestroyed = false;
                            _outlinedSegment.ResetMaterial();
                        }

                        segmentPreview.canBeDestroyed = true;
                        segmentPreview.UpdateMaterial();
                        _outlinedDestroyable = destroyableHoveredOver;
                        _outlinedSegment = segmentPreview;
                    }
                    else
                    {
                        if (_outlinedDestroyable != null)
                        {
                            _outlinedDestroyable.outline.enabled = false;
                        }
                        
                        destroyableHoveredOver.outline.enabled = true;
                        _outlinedDestroyable = destroyableHoveredOver;
                    }
                } 
                else if (!equippedToolMatchingRequiredTool)
                {
                    ResetOutlineOrPreviewMaterial();
                }
                    
                return;
            }
            
            // reset outline/segment preview if no tool is equipped or no object is hit
            ResetOutlineOrPreviewMaterial();
        }

        private void ResetOutlineOrPreviewMaterial()
        {
            if (_outlinedDestroyable != null)
            {
                if (_outlinedSegment != null)
                {
                    _outlinedSegment.canBeDestroyed = false;
                    _outlinedSegment.ResetMaterial();
                    _outlinedSegment = null;
                    _outlinedDestroyable = null;
                }
                else
                {
                    _outlinedDestroyable.outline.enabled = false;
                    _outlinedDestroyable = null;
                }
            }
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
            ToolbarInventoryController.Instance.DisableHighlight();
            MouseInventory.Instance.SetAssignedInventorySlot(new InventorySlot());
            
            if (BuildingController.Instance.BuildingPreviewEnabled)
            {
                BuildingController.Instance.DisableBuildingPreview();
            }
        }

        private void UpdateSelectedSlotHighlight()
        {
            ToolbarInventoryController.Instance.EnableHighlightAtIndex(_selectedSlotIndex);
            if (_selectedSlotIndex != 2)
            {
                InventorySlot selectedInventorySlot = ToolbarInventoryController.Instance.ToolbarInventory.InventorySlots[_selectedSlotIndex];
                MouseInventory.Instance.SetAssignedInventorySlot(selectedInventorySlot);
            }
        }

        public void OnToolUsed()
        {
            if (GameFlags.AXE_EQUIPPED || GameFlags.PICKAXE_EQUIPPED)
            {
                OnDestroyableClicked();
            }

            if (GameFlags.HAMMER_EQUIPPED && GameFlags.BUILD_MENU_CLOSED)
            {
                BuildingController.Instance.TryPlaceSegment();
            }
        }

        private void OnDestroyableClicked()
        {
            bool objectHit = Raycast.GetMouseRayHit(_camera, _destroyablesLayerMask, out RaycastHit hitResult, 40);
            if (!objectHit || hitResult.transform.gameObject == null) return;

            GameObject hitGameObject = hitResult.transform.gameObject;
            if ((_player.position - hitResult.point).magnitude > _maxInteractionDistance) return;

            ToolItemObject selectedTool = (ToolItemObject)ToolbarInventoryController.Instance.ToolbarInventory.InventorySlots[_selectedSlotIndex].Item;
            if (selectedTool == null) return;

            Destroyable destroyable = hitGameObject.GetComponentInParent<Destroyable>();
            if (destroyable == null || selectedTool != destroyable.requiredTool) return;
            
            destroyable.OnClick(_player.position, hitResult);
        }
    }
}