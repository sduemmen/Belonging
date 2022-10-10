using System;
using Models;
using UnityEngine;

namespace SaveSystem.Data
{
    [Serializable]
    public class PersistentDestroyableData
    {
        public Vector3 worldPosition;
        public Quaternion worldRotation;
        public DestroyableType type;

        public PersistentDestroyableData(Vector3 position, Quaternion rotation, DestroyableType type)
        {
            this.worldPosition = position;
            this.worldRotation = rotation;
            this.type = type;
        }

        public static GameObject GetGameObjectFromType(DestroyableType type)
        {
            return Resources.Load<GameObject>("Prefabs/Models/World/" + type.ToString());
        }
    }
}