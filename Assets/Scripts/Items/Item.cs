using UnityEngine;

namespace Items {
    public enum ItemType
    {
        Material,
        Tool,
        Empty,
    }

    public abstract class Item : ScriptableObject
    {
        [SerializeField] protected ItemType itemType;
        [SerializeField, TextArea] protected string description;
        [SerializeField] protected GameObject prefab;

        public string GetItemTypeAsString()
        {
            return itemType.ToString();
        }

        public ItemType GetItemType()
        {
            return itemType;
        }

        public string GetDescription()
        {
            return description;
        }

        public GameObject GetPrefab()
        {
            return prefab;
        }
    }

    public class EmptyItem : Item
    {
        
    }
}