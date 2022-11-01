using System;
using UnityEngine;

namespace InventorySystem.Items
{
    public enum ItemCategory
    {
        Material,
        Tool
    }

    [Serializable]
    public abstract class ItemObject : ScriptableObject
    {
        [SerializeField] protected string _displayName;
        [TextArea, SerializeField] protected string _description;
        [SerializeField] protected ItemCategory _category;
        [SerializeField] protected Sprite _icon;
        [SerializeField] protected int _maxStackSize;

        public string DisplayName => _displayName;
        public string Description => _description;
        public ItemCategory Category => _category;
        public Sprite Icon => _icon;
        public int MaxStackSize => _maxStackSize;
    }
}