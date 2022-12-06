using System;
using System.Collections.Generic;
using BuildSystem;
using UI.MainMenu;
using UnityEngine;

namespace SaveSystem.Data
{
    [Serializable]
    public class GameData
    {
        public string profileID;
        public long lastPlayed;

        public Vector3 playerSpawnPosition;
        public Vector3 playerPosition;
        public Quaternion playerRotation;
        public Quaternion cameraRotation;

        public int seed;
        public float treeDensityThreshold;
        public float stoneDensityThreshold;
        public List<string> worldAlterations;
        public List<PersistentItemData> persistentItems;
        public List<PersistentDestructibleData> persistentDestructibleData;
        public List<PersistentInventoryData> persistentInventoryData;
        public List<SegmentUnlockData> segmentUnlockData;
        public List<PersistentQuestData> quests;
        public List<PlayerSkills.Skill> playerSkills;

        public string name;
        public float playtime;
        public int placedSegments;
        public int unlockedSegments;

        public bool unlockAll;
        public bool noBuildCost;
        public bool firstLoad;

        public GameData()
        {
            profileID = Guid.NewGuid().ToString();
            lastPlayed = DateTime.Now.ToFileTime();

            playerSpawnPosition = Vector3.zero;
            playerPosition = Vector3.zero;
            playerRotation = Quaternion.identity;
            cameraRotation = Quaternion.Euler(40, 0, 0);

            seed = 0;
            treeDensityThreshold = 0;
            stoneDensityThreshold = 0;
            worldAlterations = new List<string>();
            persistentItems = new List<PersistentItemData>();
            persistentDestructibleData = new List<PersistentDestructibleData>();
            persistentInventoryData = new List<PersistentInventoryData>();
            segmentUnlockData = new List<SegmentUnlockData>();
            quests = new List<PersistentQuestData>();
            playerSkills = new List<PlayerSkills.Skill>();
            
            name = "New World";
            playtime = 0f;
            placedSegments = 0;
            unlockedSegments = 0;

            unlockAll = false;
            noBuildCost = false;
            firstLoad = true;
        }

        public GameData(NewGameData newGameData)
        {
            profileID = Guid.NewGuid().ToString();
            lastPlayed = DateTime.Now.ToFileTime();

            playerSpawnPosition = newGameData.playerSpawnPosition;
            playerPosition = Vector3.zero;
            playerRotation = Quaternion.identity;
            cameraRotation = Quaternion.Euler(40, 0, 0);

            seed = newGameData.seed;
            treeDensityThreshold = newGameData.treeDensityThreshold;
            stoneDensityThreshold = newGameData.stoneDensityThreshold;
            worldAlterations = new List<string>();
            persistentItems = new List<PersistentItemData>();
            persistentDestructibleData = new List<PersistentDestructibleData>();
            persistentInventoryData = new List<PersistentInventoryData>();
            segmentUnlockData = new List<SegmentUnlockData>();
            quests = new List<PersistentQuestData>();
            playerSkills = new List<PlayerSkills.Skill>();

            name = newGameData.gameName;
            playtime = 0f;
            placedSegments = 0;
            unlockedSegments = 0;

            unlockAll = newGameData.unlockAll;
            noBuildCost = newGameData.noBuildCost;
            firstLoad = true;
        }
    }
}