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
        [SerializeField] private string _name;
        [TextArea, SerializeField] private string _description;
        [SerializeField] private  int _maxStackSize;
        [SerializeField] private GameObject _prefab;
        [SerializeField] private ItemType _type;

        public ItemType Type {
            get => _type;
            private set => _type = value;
        }
        
        public Sprite Icon {
            get => _icon;
            private set => _icon = value;
        }

        public string Name {
            get => _name;
            private set => _name = value;
        }
        
        public string Description {
            get => _description;
            private set => _description = value;
        }

        public int MaxStackSize {
            get => _maxStackSize;
            private set => _maxStackSize = value;
        }
        
        public GameObject Prefab {
            get => _prefab;
            private set => _prefab = value;
        }
    }
}