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
        public bool droppedByPlayer;
        public float pickUpDelay;

        public PersistentItemData(Vector3 position, Quaternion rotation, string prefabName, bool droppedByPlayer, float pickUpDelay)
        {
            worldPosition = position;
            worldRotation = rotation;
            this.prefabName = prefabName;
            this.droppedByPlayer = droppedByPlayer;
            this.pickUpDelay = pickUpDelay;
        }

        public static GameObject GetGameObjectFromType(string prefabName)
        {
            return Resources.Load<GameObject>($"Prefabs/Models/Items/{prefabName}");
        }
    }
}