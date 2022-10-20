using System;
using System.Collections.Generic;
using Flags;
using UI.MainMenu;
using UnityEngine;
using Random = System.Random;

namespace SaveSystem.Data
{
    [Serializable]
    public class GameData
    {
        public string profileID;
        public long lastPlayed;
        public Vector3 playerPosition;
        public Quaternion playerRotation;
        public Quaternion cameraRotation;
        
        public int seed;
        public float treeThreshold;
        public float stoneThreshold;
        public List<string> worldAlterations;
        public List<PersistentItemData> persistentItems;
        public List<PersistentDestroyableData> persistentDestroyables;
        public List<PersistentInventoryData> persistentInventoryData;

        public string name;
        public float playtime;
        public int placedSegments;
        public int unlocked;
        
        public bool achievementsEnabled;

        public GameData()
        {
            profileID = Guid.NewGuid().ToString();
            lastPlayed = DateTime.Now.ToFileTime();
            
            playerPosition = Vector3.zero;
            playerRotation = Quaternion.identity;
            cameraRotation = Quaternion.Euler(40, 0, 0);
            
            Random random = new Random();
            seed = random.Next(100000, 100000000);
            treeThreshold = 0.8f;
            stoneThreshold = 0.2f;
            worldAlterations = new List<string>();
            persistentItems = new List<PersistentItemData>();
            persistentDestroyables = new List<PersistentDestroyableData>();
            persistentInventoryData = new List<PersistentInventoryData>();

            name = "New World";
            playtime = 0f;
            placedSegments = 0;
            unlocked = 0;
            achievementsEnabled = true;
        }

        public GameData(NewGameData newGameData)
        {
            profileID = Guid.NewGuid().ToString();
            lastPlayed = DateTime.Now.ToFileTime();
            
            playerPosition = Vector3.zero;
            playerRotation = Quaternion.identity;
            cameraRotation = Quaternion.Euler(40, 0, 0);
            
            seed = newGameData.seed;
            treeThreshold = 1 - newGameData.treeThreshold;
            stoneThreshold = newGameData.stoneThreshold;
            worldAlterations = new List<string>();
            persistentItems = new List<PersistentItemData>();
            persistentDestroyables = new List<PersistentDestroyableData>();
            persistentInventoryData = new List<PersistentInventoryData>();

            name = newGameData.gameName;
            playtime = 0f;
            placedSegments = 0;
            unlocked = newGameData.unlockAll ? GameConstants.MAX_UNLOCKABLE_SEGMENTS : 0;
            achievementsEnabled = !newGameData.unlockAll;
        }
    }
}