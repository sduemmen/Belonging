using System;
using UnityEngine;

namespace Utility
{
    [CreateAssetMenu(menuName = "Debug/Collider")]
    [Serializable]
    public class ColliderDebugInformation : ScriptableObject
    {
        public ColliderUsage usage;
        public ColliderType type;
        public Color color;

        public ColliderDebugInformation(ColliderUsage usage, ColliderType type, Color color)
        {
            this.usage = usage;
            this.type = type;
            this.color = color;
        }
    }
}