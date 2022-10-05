using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SaveSystem.Data
{
    [Serializable]
    public class GameData
    {
        public string profileID;
        public Vector3 playerPosition;
        public Quaternion playerRotation;
        public Quaternion cameraRotation;
        public List<PersistentGameObjectData> persistentGameObjects;
        public List<PersistentInventoryData> persistentInventoryData;

        public string name;
        public float playtime;
        public int score;
        public int unlocked;

        public int seed;

        public GameData()
        {
            profileID = Guid.NewGuid().ToString();
            
            playerPosition = Vector3.zero;
            playerRotation = Quaternion.Euler(0, 0, 0);
            cameraRotation = Quaternion.Euler(40, 0, 0);
            persistentGameObjects = new List<PersistentGameObjectData>(); // TODO - initialize with world gen
            persistentInventoryData = new List<PersistentInventoryData>();

            name = "New World";
            playtime = 0f;
            score = 0;
            unlocked = 0;

            seed = Random.Range(100000, 100000000);
        }
    }
}