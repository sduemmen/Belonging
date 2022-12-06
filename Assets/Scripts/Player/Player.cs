using System.Collections.Generic;
using System.Linq;
using BuildSystem;
using InventorySystem;
using InventorySystem.Items;
using SaveSystem;
using SaveSystem.Data;
using UnityEngine;
using UnityEngine.UI;
using Utility;

public class Player : Entity, IDataPersistence
{
    private static Player instance;
    
    public static Player Instance {
        get {
            if (instance == null)
            {
                instance = (Player)FindObjectOfType(typeof(Player));
            }

            return instance;
        }
    }

    public Transform m_player;
    
    public Transform m_playerObj;

    public Transform m_playerShoulder;

    public Vector3 m_spawnPoint;

    public PlayerSkills m_playerSkills = new PlayerSkills();

    public FXList m_levelUpFX;
    
    [SerializeField] private Slider m_attackCooldownIndicator;
    
    private float m_attackCooldown;
    
    private float m_destroySegmentCooldown;
    
    public int m_equippedSlot = -1;
    
    private LayerMask m_destructibleLayerMask;
    
    private Destructible m_currentDestructibleHoveredOver;
    
    private JumpFloodOutlineRenderer m_outlineRenderer;
    
    [SerializeField] private float m_movementSpeed;
    
    [SerializeField] private float m_sprintSpeedFactor;
    
    [SerializeField] private float m_rotationDamping;

    [SerializeField] private CharacterController m_characterController;
    
    private Animator m_animator;
    
    private float m_currentLookDirectionAngle;
    
    
    private void Awake()
    {
        m_destructibleLayerMask = LayerMask.GetMask("Segment", "Destructible");
        m_player = transform;
        m_attackCooldownIndicator.value = 0;
        m_outlineRenderer = GetComponent<JumpFloodOutlineRenderer>();
        m_playerSkills.skillLevelUpDelegate += OnLevelUp;
        m_animator = GetComponent<Animator>();
    }

    private void Update()
    {
        UpdatePlayerMovement();
        UpdateAttackCooldown();
        UpdateDestructibleHoveredOver();
        CheckInput();

        
    }

    private void CheckInput()
    {
        if (Flags.GAME_PAUSED)
        {
            return;
        }
        
        if (InputSystem.GetKeyDown(InputSystem.KeyBinds.Attack) && !Flags.UI_ELEMENT_OPEN)
        {
            OnToolUsed();
        }
        else if (InputSystem.GetKeyDown(InputSystem.KeyBinds.EquipUnequip_Axe))
        {
            EquipSlot(0);
        }
        else if (InputSystem.GetKeyDown(InputSystem.KeyBinds.EquipUnequip_Pickaxe))
        {
            EquipSlot(1);
        }
        else if (InputSystem.GetKeyDown(InputSystem.KeyBinds.Open_Build_Menu))
        {
            EquipSlot(2);
        }
        else if (InputSystem.GetKeysDown(InputSystem.KeyBinds.Toggle_Inventory, InputSystem.KeyBinds.Toggle_Quest_Display))
        {
            EquipSlot(-1);
        }
    }
    
    public Vector3 GetDirection(Vector3 to)
    {
        return to - m_player.position;
    }
    
    public Vector3 GetNormalizedDirection(Vector3 to)
    {
        return GetDirection(to).normalized;
    }

    private void UpdatePlayerMovement()
    {
        Vector2 movementInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")).normalized;
        bool playerIsMoving = movementInput != Vector2.zero;
        float lookDirection;

        if (playerIsMoving)
        {
            movementInput = Utils.Math.RotateVector2Deg(new Vector2(-movementInput.x, movementInput.y), m_playerShoulder.eulerAngles.y);
            lookDirection = Mathf.Acos(Vector2.Dot(Vector2.up, movementInput));
            lookDirection *= Mathf.Sign(movementInput.x);
            m_currentLookDirectionAngle = lookDirection;
        }
        else
        {
            lookDirection = m_currentLookDirectionAngle;
        }

        Quaternion lookDirectionRotation = Quaternion.Euler(0, lookDirection * -Mathf.Rad2Deg, 0);
        m_player.rotation = Quaternion.Lerp(m_player.rotation, lookDirectionRotation, m_rotationDamping);

        if (playerIsMoving)
        {
            float speed = m_movementSpeed * Time.fixedDeltaTime;
            
            if (InputSystem.GetKeyDown(InputSystem.KeyBinds.Sprint))
            {
                speed *= m_sprintSpeedFactor;
                m_animator.ResetTrigger("Idle");
                m_animator.ResetTrigger("Walking");
                m_animator.SetTrigger("Running");
            }
            else
            {
                m_animator.ResetTrigger("Idle");
                m_animator.ResetTrigger("Running");
                m_animator.SetTrigger("Walking");
            }
            
            Vector3 moveDirection = lookDirectionRotation * m_characterController.transform.forward;
            m_characterController.SimpleMove(moveDirection * speed);
        }
        else
        {
            m_animator.ResetTrigger("Walking");
            m_animator.ResetTrigger("Running");
            m_animator.SetTrigger("Idle");
        }
    }
    
    private void UpdateAttackCooldown()
    {
        m_destroySegmentCooldown = Mathf.Max(m_destroySegmentCooldown - Time.deltaTime, 0);
        m_attackCooldown = Mathf.Max(m_attackCooldown - Time.deltaTime, 0);
        m_attackCooldownIndicator.value = m_attackCooldown;
        m_attackCooldownIndicator.gameObject.SetActive(Flags.AXE_EQUIPPED || Flags.PICKAXE_EQUIPPED);
    }

    private void UpdateDestructibleHoveredOver()
    {
        if (Flags.SLOT_EQUIPPED && Destructible.GetDestructibleHoveredOver(m_destructibleLayerMask, 40, out RaycastHit hit, out Destructible destructibleHoveredOver))
        {
            if (destructibleHoveredOver != m_currentDestructibleHoveredOver)
            {
                ResetCurrentDestructibleHoveredOver();
            }
            
            bool usingCorrectTool = (destructibleHoveredOver.m_requiredTool.DisplayName == "Axe" && Flags.AXE_EQUIPPED) || 
                                    (destructibleHoveredOver.m_requiredTool.DisplayName == "Pickaxe" && Flags.PICKAXE_EQUIPPED) ||
                                    (destructibleHoveredOver.m_requiredTool.DisplayName == "Hammer" && Flags.HAMMER_EQUIPPED && BuildingController.Instance.m_inDeleteMode);

            if (usingCorrectTool && destructibleHoveredOver != m_currentDestructibleHoveredOver)
            {
                m_currentDestructibleHoveredOver = destructibleHoveredOver;

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
                    m_outlineRenderer.renderers = destructibleHoveredOver.GetComponentsInChildren<Renderer>().ToList();
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
        if (m_currentDestructibleHoveredOver)
        {
            if (m_currentDestructibleHoveredOver.m_hasHoverEffect)
            {
                MeshRenderer[] meshRenderers = m_currentDestructibleHoveredOver.GetComponentsInChildren<MeshRenderer>();

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
            else if (m_currentDestructibleHoveredOver.m_useOutlineInsteadOfMaterial)
            {
                m_outlineRenderer.renderers = new List<Renderer>();
            }
            
            m_currentDestructibleHoveredOver = null;
        }
    }

    public void EquipSlot(int slot)
    {
        ToolbarInventoryController.Instance.DisableHighlight();
        
        if (slot >= 0 && m_equippedSlot != slot)
        {
            m_equippedSlot = slot;
            ToolbarInventoryController.Instance.EnableHighlightAtIndex(slot);
        }
        else
        {
            m_equippedSlot = -1;
        }
    }

    public void OnToolUsed()
    {
        if ((Flags.AXE_EQUIPPED || Flags.PICKAXE_EQUIPPED) && m_attackCooldown <= 0)
        {
            CheckDestructibleHit();
            m_attackCooldown = 1;
        }

        if (Flags.HAMMER_EQUIPPED && Flags.BUILD_MENU_CLOSED && BuildingController.Instance.m_inDeleteMode && m_destroySegmentCooldown <= 0)
        {
            BuildingController.Instance.TryDeleteSegment();
            m_destroySegmentCooldown = .2f;
        }
        
        if (Flags.HAMMER_EQUIPPED && Flags.BUILD_MENU_CLOSED && !BuildingController.Instance.m_inDeleteMode)
        {
            BuildingController.Instance.TryPlaceSegment();
        }
    }

    private void CheckDestructibleHit()
    {
        if (Destructible.GetDestructibleHoveredOver(m_destructibleLayerMask, 30, out RaycastHit hit, out Destructible hitDestructible))
        {
            ToolItemObject equippedTool = (ToolItemObject)ToolbarInventoryController.Instance.ToolbarInventory.InventorySlots[m_equippedSlot].Item;
            if (equippedTool == null || equippedTool.DisplayName.Equals("Hammer")) return;

            float damageAmount = Mathf.Pow(1.1f, m_playerSkills.GetLevel(equippedTool.DisplayName));
            
            if (hitDestructible.OnDamaged(new DamageData(hit.point, GetNormalizedDirection(hit.point), equippedTool, damageAmount)))
            {
                m_playerSkills.AddToSkill(equippedTool.DisplayName, 0.05f);
            }
        }
    }

    private void OnLevelUp()
    {
        m_levelUpFX.PlayFX(transform.position + Vector3.up);
    }

    public void LoadData(GameData data)
    {
        m_spawnPoint = data.playerSpawnPosition;
        m_playerObj.position = data.firstLoad ? m_spawnPoint : data.playerPosition;
        m_playerObj.rotation = data.playerRotation;
        m_playerSkills.m_skills = data.firstLoad ? m_playerSkills.m_skills : data.playerSkills;
    }

    public void SaveData(ref GameData data)
    {
        data.playerSpawnPosition = m_spawnPoint;
        data.playerPosition = m_playerObj.position;
        data.playerRotation = m_playerObj.rotation;
        data.playerSkills = m_playerSkills.m_skills;
    }
}