using System;
using UnityEngine;

namespace InventorySystem.Items
{
    [CreateAssetMenu(menuName = "Inventory/Items/Tool Item"), Serializable]
    public class ToolItemObject : ItemObject
    {
        private void Awake()
        {
            maxStackSize = 1;
            category = ItemCategory.Tool;
        }
    }
}