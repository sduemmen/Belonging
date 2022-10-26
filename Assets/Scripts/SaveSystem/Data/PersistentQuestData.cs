using System;
using System.Collections.Generic;
using QuestSystem;
using QuestSystem.QuestBehaviours;
using QuestSystem.QuestCompletionBehaviours;
using UnityEngine;
using UnityEngine.Events;

namespace SaveSystem.Data
{
    [Serializable]
    public class PersistentQuestData
    {
        public string title;
        public string description;
        public bool completed;
        [SerializeReference] public QuestBehaviour questBehaviour;
        [SerializeReference] public List<QuestCompletionBehaviour> questCompletionBehaviours;
        public UnityEvent OnCompleteQuestCallback;

        public PersistentQuestData(Quest quest)
        {
            title = quest.title;
            description = quest.description;
            completed = quest.completed;
            questBehaviour = quest.questBehaviour;
            questCompletionBehaviours = quest.completionBehaviours;
            OnCompleteQuestCallback = quest.OnCompleteQuestCallback;
        }
    }
}