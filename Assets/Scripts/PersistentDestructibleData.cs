using System;
using BuildSystem;
using UnityEngine;

[Serializable]
public class PersistentDestructibleData
{
    public string m_prefabName;

    public Destructible.DestructibleCategory m_category;
    
    public Vector3 m_position;
    
    public Quaternion m_rotation;

    public int m_health;

    public PersistentDestructibleData(string prefabName, Destructible.DestructibleCategory category, Vector3 position, Quaternion rotation, int health)
    {
        m_prefabName = prefabName;
        m_category = category;
        m_position = position;
        m_rotation = rotation;
        m_health = health;
    }
    
    public PersistentDestructibleData(string prefabName, Destructible.DestructibleCategory category, Transform transform, int health)
    {
        m_prefabName = prefabName;
        m_category = category;
        m_position = transform.position;
        m_rotation = transform.rotation;
        m_health = health;
    }
    
    public PersistentDestructibleData(Destructible d)
    {
        m_prefabName = d.m_prefabName;
        m_category = d.m_category;
        m_position = d.transform.position;
        m_rotation = d.transform.rotation;
        m_health = d.m_health;
    }
}