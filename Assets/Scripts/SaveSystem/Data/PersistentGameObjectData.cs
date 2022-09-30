using System;
using UnityEngine;

namespace SaveSystem.Data
{
    public enum PersistentGameObjectType {
        WoodItem,
        StoneItem,
        Tree,
        StoneModel,
    }
    
    [Serializable]
    public class PersistentGameObjectData
    {
        public Vector3 worldPosition;
        public Quaternion worldRotation;
        public PersistentGameObjectType type;

        public PersistentGameObjectData(Vector3 position, Quaternion rotation, PersistentGameObjectType type)
        {
            this.worldPosition = position;
            this.worldRotation = rotation;
            this.type = type;
        }

        public static GameObject GetGameObjectFromType(PersistentGameObjectType type)
        {
            return Resources.Load<GameObject>("Prefabs/" + type.ToString());
        }
    }
}