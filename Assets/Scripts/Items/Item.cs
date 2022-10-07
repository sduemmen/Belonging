using System;
using UnityEngine;

namespace Items
{
    [Serializable]
    public enum ItemType
    {
        Axe,
        Pickaxe,
        Wood,
        Stone,
    }
    
    [CreateAssetMenu(menuName = "Custom/Item"), Serializable]
    public class Item : ScriptableObject
    {
        [SerializeField] private Sprite _icon;
        [SerializeField] private string _displayName;
        [TextArea, SerializeField] private string _description;
        [SerializeField] private  int _maxStackSize;
        [SerializeField] private GameObject _prefab;
        [SerializeField] private ItemType _type;
        public static ItemType WOOD = ItemType.Wood;
        public static ItemType STONE = ItemType.Stone;
        public static ItemType AXE = ItemType.Axe;
        public static ItemType PICKAXE = ItemType.Pickaxe;

        public ItemType GetItemType()
        {
            return _type;
        }
        
        public Sprite GetIcon()
        {
            return _icon;
        }

        public string GetDisplayName()
        {
            return _displayName;
        }
        
        public string GetDescription()
        {
            return _description;
        }

        public int GetMaxStackSize()
        {
            return _maxStackSize;
        }
        
        public GameObject GetPrefab()
        {
            return _prefab;
        }
    }
}