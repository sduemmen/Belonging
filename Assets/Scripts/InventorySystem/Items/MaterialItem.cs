using System;
using UnityEngine;

namespace InventorySystem.Items
{
    [CreateAssetMenu(menuName = "Inventory/Items/Material Item"), Serializable]
    public class MaterialItemObject : ItemObject
    {
        public GameObject prefab;
        
        private void Awake()
        {
            category = ItemCategory.Material;
        }
    }
}