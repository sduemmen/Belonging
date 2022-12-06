using System;
using System.Collections.Generic;
using BuildSystem;
using InventorySystem.Items;
using SaveSystem;
using SaveSystem.Data;
using UnityEngine;
using Utility;
using Random = UnityEngine.Random;

[Serializable]
public class Destructible : MonoBehaviour, IDataPersistence
{
    public enum DestructibleCategory
    {
        Tree,
        Stone,
        Segment,
    }
    
    public string m_prefabName;
    
    public DestructibleCategory m_category;

    public ToolItemObject m_requiredTool;
    
    public FXList m_damageFX;

    public FXList m_destructionFX;
    
    public float m_health;

    public List<ItemStack> m_itemDrops;

    public bool m_hasHoverEffect;

    public Material m_hoverMaterial;

    public bool m_useOutlineInsteadOfMaterial;
    
    public Vector2Int m_chunkPosition;
    
    public Vector2Int m_positionInChunk;

    public bool m_recordWorldAlterations = true;

    public bool m_countsTowardsPlacedSegments;

    public bool m_isPersistent;


    public static Destructible Load(string name, DestructibleCategory category)
    {
        return Resources.Load<Destructible>($"Prefabs/Models/{category}/{name}");
    }

    public static bool GetDestructibleHoveredOver(LayerMask layerMask, float distance, out RaycastHit hit, out Destructible destructible)
    {
        destructible = null;
        if (Raycast.GetMouseRayHit(layerMask, distance, out hit))
        {
            destructible = hit.collider.GetComponentInParent<Destructible>();
        }

        return destructible != null;
    }

    public void Initialize(PersistentDestructibleData data)
    {
        m_health = data.m_health;
    }

    public bool OnDamaged(DamageData damageData)
    {
        if (damageData.m_usedTool != m_requiredTool) return false;

        if (m_damageFX)
        {
            m_damageFX.PlayFX(damageData.m_hitPoint);
        }

        m_health -= damageData.m_damageAmount;
        if (m_health <= 0)
        {
            OnDestroyed(damageData);
        }

        return true;
    }

    public void OnDestroyed(DamageData damageData)
    {
        if (m_destructionFX)
        {
            m_destructionFX.PlayFX(damageData.m_hitPoint);
        }

        foreach (ItemStack itemDrop in m_itemDrops)
        {
            for (int i = 0; i < itemDrop.Amount; i++)
            {
                Instantiate(((MaterialItemObject)itemDrop.Item).Prefab, Random.insideUnitSphere + transform.position + Vector3.up, Quaternion.identity);
            }
        }
        
        if (m_recordWorldAlterations)
        {
            World.World.Instance.worldAlterations.AddAlteration(m_positionInChunk.x, m_chunkPosition.y, m_positionInChunk.x, m_positionInChunk.y);
        }
        
        if (m_countsTowardsPlacedSegments)
        {
            World.World.Instance.placedSegments -= 1;
        }
        
        Destroy(this.gameObject, 3);
        this.gameObject.SetActive(false);
    }

    public void LoadData(GameData data)
    {
        Destroy(this.gameObject);
    }

    public void SaveData(ref GameData data)
    {
        if (Flags.MAIN_MENU_ACTIVE || !m_isPersistent)
        {
            return;
        }
        data.persistentDestructibleData.Add(new PersistentDestructibleData(this));
    }
}