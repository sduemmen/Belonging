using System;
using UnityEngine;

namespace InventorySystem.Items
{
    public enum ItemCategory
    {
        Material,
        Tool,
    }
    
    [Serializable]
    public abstract class ItemObject : ScriptableObject
    {
        public Sprite icon;
        public string displayName;
        [TextArea] public string description;
        public int maxStackSize;
        public ItemCategory category;
    }
}
