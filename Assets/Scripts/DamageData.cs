using InventorySystem.Items;
using UnityEngine;

public class DamageData
{
    public Vector3 m_hitPoint;

    public Vector3 m_hitDirection;

    public ToolItemObject m_usedTool;

    public int m_damageAmount;
    

    public DamageData(Vector3 hitPoint, Vector3 hitDirection, ToolItemObject usedTool, int damageAmount)
    {
        m_hitPoint = hitPoint;
        m_hitDirection = hitDirection;
        m_usedTool = usedTool;
        m_damageAmount = damageAmount;
    }
}