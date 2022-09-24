using UnityEngine;

namespace Items
{
    public enum ToolType
    {
        Axe,
        Pickaxe,
    }
    
    public class ToolItem : Item
    {
        private ToolType _toolType;
        public string Name => _toolType.ToString();

        public ToolType GetToolType()
        {
            return _toolType;
        }
    }
}
