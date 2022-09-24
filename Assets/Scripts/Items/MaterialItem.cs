using UnityEngine;

namespace Items
{
    public enum MaterialType
    {
        Wood,
        Stone,
    }
    
    public class MaterialItem : Item
    {
        private MaterialType _material;
        public string Name => _material.ToString();

        public MaterialType GetMaterialType()
        {
            return _material;
        }
    }
}
