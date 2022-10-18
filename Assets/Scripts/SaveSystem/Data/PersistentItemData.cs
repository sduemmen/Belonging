using System;
using UnityEngine;

namespace SaveSystem.Data
{
    [Serializable]
    public class PersistentItemData
    {
        public Vector3 worldPosition;
        public Quaternion worldRotation;
        public string prefabName;

        public PersistentItemData(Vector3 position, Quaternion rotation, string prefabName)
        {
            worldPosition = position;
            worldRotation = rotation;
            this.prefabName = prefabName;
        }

        public static GameObject GetGameObjectFromType(string prefabName)
        {
            return Resources.Load<GameObject>($"Prefabs/Models/Items/{prefabName}");
        }
    }
}