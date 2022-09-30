using System;
using System.Collections.Generic;
using UnityEngine;

namespace SaveSystem.Data
{
    [Serializable]
    public class GameData
    {
        public Vector3 playerPosition;
        public Quaternion playerRotation;
        public Quaternion cameraRotation;
        public List<PersistentGameObjectData> persistentGameObjects;
        public List<PersistentInventoryData> persistentInventoryData;

        public GameData()
        {
            playerPosition = Vector3.zero;
            playerRotation = Quaternion.Euler(0, 0, 0);
            cameraRotation = Quaternion.Euler(40, 0, 0);
            persistentGameObjects = new List<PersistentGameObjectData>(); // TODO - initialize with world gen
            persistentInventoryData = new List<PersistentInventoryData>();
        }
    }
}