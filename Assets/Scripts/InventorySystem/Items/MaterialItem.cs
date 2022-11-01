using System;
using UnityEngine;

namespace InventorySystem.Items
{
    [CreateAssetMenu(menuName = "Inventory/Items/Material Item")]
    [Serializable]
    public class MaterialItemObject : ItemObject
    {
        [SerializeField] private GameObject _prefab;

        public GameObject Prefab => _prefab;

        private void Awake()
        {
            _category = ItemCategory.Material;
        }
    }
}