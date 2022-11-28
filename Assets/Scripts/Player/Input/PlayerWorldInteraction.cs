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
        private float _destructibleAttackCooldown;
        private float _segmentAttackCooldown;
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
            _segmentAttackCooldown = Mathf.Max(_segmentAttackCooldown - 1 * Time.deltaTime, 0);
            _destructibleAttackCooldown = Mathf.Max(_destructibleAttackCooldown - 1 * Time.deltaTime, 0);
            _cooldownIndicator.value = _destructibleAttackCooldown;
            _cooldownIndicator.gameObject.SetActive((GameFlags.AXE_EQUIPPED || GameFlags.PICKAXE_EQUIPPED));
        }

        private void UpdateDestructibleHoveredOver()
        {
            if (GameFlags.SLOT_EQUIPPED && Destructible.GetDestructibleHoveredOver(_destructibleLayerMask, 40, out RaycastHit hit, out Destructible destructibleHoveredOver))
            {
                if (destructibleHoveredOver != _currentDestructibleHoveredOver)
                {
                    ResetCurrentDestructibleHoveredOver();
                }
                
                bool usingCorrectTool = (destructibleHoveredOver.m_requiredTool.DisplayName == "Axe" && GameFlags.AXE_EQUIPPED) || 
                                        (destructibleHoveredOver.m_requiredTool.DisplayName == "Pickaxe" && GameFlags.PICKAXE_EQUIPPED) ||
                                        (destructibleHoveredOver.m_requiredTool.DisplayName == "Hammer" && GameFlags.HAMMER_EQUIPPED && BuildingController.Instance.m_inDeleteMode);

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
            
            BuildingController.Instance.DestroyGhostSegment();
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
            if ((GameFlags.AXE_EQUIPPED || GameFlags.PICKAXE_EQUIPPED) && _destructibleAttackCooldown <= 0)
            {
                CheckDestructibleHit();
                _destructibleAttackCooldown = 1;
            }

            if (GameFlags.HAMMER_EQUIPPED && GameFlags.BUILD_MENU_CLOSED && BuildingController.Instance.m_inDeleteMode && _segmentAttackCooldown <= 0)
            {
                BuildingController.Instance.TryDeleteSegment();
                _segmentAttackCooldown = .2f;
            }
            
            if (GameFlags.HAMMER_EQUIPPED && GameFlags.BUILD_MENU_CLOSED && !BuildingController.Instance.m_inDeleteMode && !InputController.LeftMouseButtonHeldDown)
            {
                BuildingController.Instance.TryPlaceSegment();
            }
        }

        private void CheckDestructibleHit()
        {
            if (Destructible.GetDestructibleHoveredOver(_destructibleLayerMask, 30, out RaycastHit hit, out Destructible hitDestructible))
            {
                ToolItemObject equippedTool = (ToolItemObject)ToolbarInventoryController.Instance.ToolbarInventory.InventorySlots[_selectedSlotIndex].Item;
                if (equippedTool == null || equippedTool.DisplayName.Equals("Hammer")) return;

                PlayerSkills playerSkills = GetComponent<Player>().m_playerSkills;
                float damageAmount = Mathf.Pow(1.1f, playerSkills.GetLevel(equippedTool.DisplayName));
                
                if (hitDestructible.OnDamaged(new DamageData(hit.point, (hit.point - _player.position).normalized, equippedTool, damageAmount)));
                {
                    playerSkills.AddToSkill(equippedTool.DisplayName, 0.05f);
                }
            }
        }
    }
}