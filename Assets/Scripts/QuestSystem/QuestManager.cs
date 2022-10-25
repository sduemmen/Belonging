using System;
using System.Collections.Generic;
using System.Linq;
using SaveSystem;
using SaveSystem.Data;
using Sirenix.OdinInspector;
using UnityEngine;

namespace QuestSystem
{
    [Serializable]
    public class QuestManager : MonoBehaviour, IDataPersistence
    {
        public QuestCollection questCollection;
        [InlineEditor] private List<Quest> quests = new List<Quest>();
        private bool _firstLoad = true;
        
        public void IncrementWoodCollectionQuests()
        {
            List<Quest> woodCollectionQuests = quests.Where(quest => quest.questType == QuestType.Collection && quest.collectionQuestType == CollectionQuestType.Wood).ToList();
            foreach (Quest quest in woodCollectionQuests) {
                quest.Increment();
            }
        }
        
        public void IncrementStoneCollectionQuests()
        {
            List<Quest> stoneCollectionQuests = quests.Where(quest => quest.questType == QuestType.Collection && quest.collectionQuestType == CollectionQuestType.Stone).ToList();
            foreach (Quest quest in stoneCollectionQuests) {
                quest.Increment();
            }
        }
        
        public void IncrementAnyCollectionQuests()
        {
            List<Quest> anyCollectionQuests = quests.Where(quest => quest.questType == QuestType.Collection && quest.collectionQuestType == CollectionQuestType.Any).ToList();
            foreach (Quest quest in anyCollectionQuests) {
                quest.Increment();
            }
        }
        
        public void IncrementBuildingQuests()
        {
            List<Quest> buildingQuests = quests.Where(quest => quest.questType == QuestType.Building).ToList();
            foreach (Quest quest in buildingQuests) {
                quest.Increment();
            }
        }
        
        public void LoadData(GameData data)
        {
            if (data.firstLoad) {
                quests = questCollection.entries;
            } else {
                quests = data.quests;
            }
            _firstLoad = false;
        }

        public void SaveData(ref GameData data)
        {
            data.quests = quests;
            data.firstLoad = _firstLoad;
        }
    }
}