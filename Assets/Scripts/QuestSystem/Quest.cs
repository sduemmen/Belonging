using System;
using Collections;
using Events.Events;
using Sirenix.OdinInspector;
using UnityEngine;

namespace QuestSystem
{
    [Serializable]
    public enum QuestType
    {
        Collection,
        Building,
    }

    [Serializable]
    public enum CollectionQuestType
    {
        Any,
        Wood,
        Stone,
    }
    
    [Serializable]
    public class Quest : CollectionEntry
    {
        public string segmentReward;
        public QuestType questType;

        [ShowIf("@questType == QuestType.Collection")] 
        [SerializeField]
        public CollectionQuestType collectionQuestType; 
        
        public string description;
        public bool fulfilled;
        public int target;
        public int progress;

        [SerializeField] private StringEvent questFulfilledEvent;

        public void Fulfill()
        {
            fulfilled = true;
            questFulfilledEvent.Raise(segmentReward);
        }

        public void Increment()
        {
            progress += 1;
            if (progress >= target) {
                Fulfill();
            }
        }
    }
}