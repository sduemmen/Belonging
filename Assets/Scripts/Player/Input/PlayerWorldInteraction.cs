using System.Collections.Generic;
using System.Linq;
using BuildSystem;
using Flags;
using InventorySystem;
using InventorySystem.Items;
using UnityEngine;
using UnityEngine.UI;

namespace Player.Input
{
    public class PlayerWorldInteraction : MonoBehaviour
    {
        [SerializeField] private Transform _player;
        [SerializeField] private Slider _cooldownIndicator;
        private float _cooldown;
        private int _selectedSlotIndex = -1;
        private LayerMask _destructibleLayerMask;
        private Destructible _currentDestructibleHoveredOver;
        private JumpFloodOutlineRenderer _outlineRenderer;

        private void Awake()
        {
            _destructibleLayerMask = LayerMask.GetMask("Segment", "Destructible");
            _player = transform;
            _cooldownIndicator.value = 0;
            _outlineRenderer = GetComponent<JumpFloodOutlineRenderer>();
        }

        private void Update()
        {
            UpdateAttackCooldown();
            UpdateDestructibleHoveredOver();
        }

        private void UpdateAttackCooldown()
        {
            _cooldown = Mathf.Max(_cooldown - 1 * Time.deltaTime, 0);
            _cooldownIndicator.value = _cooldown;
            _cooldownIndicator.gameObject.SetActive((GameFlags.AXE_EQUIPPED || GameFlags.PICKAXE_EQUIPPED));
        }

        private void UpdateDestructibleHoveredOver()
        {
            if ((GameFlags.AXE_EQUIPPED || GameFlags.PICKAXE_EQUIPPED) && Destructible.GetDestructibleHoveredOver(_destructibleLayerMask, 40, out RaycastHit hit, out Destructible destructibleHoveredOver))
            {
                if (destructibleHoveredOver != _currentDestructibleHoveredOver)
                {
                    ResetCurrentDestructibleHoveredOver();
                }
                
                bool usingCorrectTool = (destructibleHoveredOver.m_requiredTool.DisplayName == "Axe" && GameFlags.AXE_EQUIPPED) || (destructibleHoveredOver.m_requiredTool.DisplayName == "Pickaxe" && GameFlags.PICKAXE_EQUIPPED);

                if (usingCorrectTool && destructibleHoveredOver != _currentDestructibleHoveredOver)
                {
                    _currentDestructibleHoveredOver = destructibleHoveredOver;

                    if (destructibleHoveredOver.m_hasHoverEffect)
                    {
                        MeshRenderer[] meshRenderers = destructibleHoveredOver.GetComponentsInChildren<MeshRenderer>();

                        foreach (MeshRenderer meshRenderer in meshRenderers)
                        {
                            if (meshRenderer.transform.name.StartsWith("Quad"))
                            {
                                continue;
                            }
                        
                            Material[] materials = meshRenderer.materials;
                            Material[] newMaterials = new Material[materials.Length + 1];
                            for (int i = 0; i < materials.Length; i++)
                            {
                                newMaterials[i] = materials[i];
                            }

                            newMaterials[^1] = destructibleHoveredOver.m_hoverMaterial;
                            meshRenderer.materials = newMaterials;
                        }
                    }
                    else if (destructibleHoveredOver.m_useOutlineInsteadOfMaterial)
                    {
                        _outlineRenderer.renderers = destructibleHoveredOver.GetComponentsInChildren<Renderer>().ToList();
                    }
                }
            }
            else
            {
                ResetCurrentDestructibleHoveredOver();
            }
        }

        private void ResetCurrentDestructibleHoveredOver()
        {
            if (_currentDestructibleHoveredOver)
            {
                if (_currentDestructibleHoveredOver.m_hasHoverEffect)
                {
                    MeshRenderer[] meshRenderers = _currentDestructibleHoveredOver.GetComponentsInChildren<MeshRenderer>();

                    foreach (MeshRenderer meshRenderer in meshRenderers)
                    {
                        if (meshRenderer.transform.name.StartsWith("Quad"))
                        {
                            continue;
                        }
                    
                        Material[] materials = meshRenderer.materials;
                        Material[] newMaterials = new Material[materials.Length - 1];
                        for (int i = 0; i < newMaterials.Length; i++)
                        {
                            newMaterials[i] = materials[i];
                        }

                        meshRenderer.materials = newMaterials;
                    }
                }
                else if (_currentDestructibleHoveredOver.m_useOutlineInsteadOfMaterial)
                {
                    _outlineRenderer.renderers = new List<Renderer>();
                }
                
                _currentDestructibleHoveredOver = null;
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
            
            if (BuildingController.Instance.GhostSegmentVisible)
            {
                BuildingController.Instance.DestroyGhostSegment();
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
            if ((GameFlags.AXE_EQUIPPED || GameFlags.PICKAXE_EQUIPPED) && _cooldown <= 0)
            {
                CheckDestructibleHit();
                _cooldown = 1;
            }

            if (GameFlags.HAMMER_EQUIPPED && GameFlags.BUILD_MENU_CLOSED && !InputController.LeftMouseButtonHeldDown)
            {
                BuildingController.Instance.TryPlaceSegment();
            }
        }

        private void CheckDestructibleHit()
        {
            if (Destructible.GetDestructibleHoveredOver(_destructibleLayerMask, 30, out RaycastHit hit, out Destructible hitDestructible))
            {
                ToolItemObject equippedTool = (ToolItemObject)ToolbarInventoryController.Instance.ToolbarInventory.InventorySlots[_selectedSlotIndex].Item;
                if (equippedTool == null) return;
                hitDestructible.OnDamaged(new DamageData(hit.point, (hit.point - _player.position).normalized, equippedTool, 1));
            }
        }
    }
}