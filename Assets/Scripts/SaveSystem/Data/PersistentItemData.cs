using System;
using ScriptableObjects;
using UnityEngine;

namespace SaveSystem.Data
{
    [Serializable]
    public class PersistentItemData
    {
        public Vector3 worldPosition;
        public Quaternion worldRotation;
        public ItemType type;

        public PersistentItemData(Vector3 position, Quaternion rotation, ItemType type)
        {
            this.worldPosition = position;
            this.worldRotation = rotation;
            this.type = type;
        }

        public static GameObject GetGameObjectFromType(ItemType type)
        {
            return Resources.Load<GameObject>("Prefabs/Models/Items/" + type.ToString());
        }
    }
}