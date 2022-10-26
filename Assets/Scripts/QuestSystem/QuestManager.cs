using System;
using System.Collections.Generic;
using SaveSystem;
using SaveSystem.Data;
using Sirenix.OdinInspector;
using UnityEngine;

namespace QuestSystem
{
    [Serializable]
    public class QuestManager : MonoBehaviour, IDataPersistence
    {
        [SerializeField, InlineEditor] public List<Quest> questCollection;
        [SerializeField, InlineEditor] private List<Quest> quests = new List<Quest>();
        [SerializeField] private bool _firstLoad = true;

        public void LoadData(GameData data)
        {
            quests.Clear();
            
            if (data.firstLoad) {
                foreach (Quest q in questCollection) {
                    Quest quest = (Quest)ScriptableObject.CreateInstance(typeof(Quest));
                    quest.title = q.title;
                    quest.description = q.description;
                    quest.completed = q.completed;
                    quest.questBehaviour = q.questBehaviour;
                    quest.completionBehaviours = q.completionBehaviours;
                    quest.OnCompleteQuestCallback = q.OnCompleteQuestCallback;
                    quest.Initialize();
                    quests.Add(quest);
                }
            } else {
                foreach (var questData in data.quests) {
                    Quest quest = (Quest)ScriptableObject.CreateInstance(typeof(Quest));
                    quest.title = questData.title;
                    quest.description = questData.description;
                    quest.completed = questData.completed;
                    quest.questBehaviour = questData.questBehaviour;
                    quest.completionBehaviours = questData.questCompletionBehaviours;
                    quest.OnCompleteQuestCallback = questData.OnCompleteQuestCallback;
                    quest.Initialize();
                    quests.Add(quest);
                }
            }
            
            _firstLoad = false;
        }

        public void SaveData(ref GameData data)
        {
            data.quests.Clear();
            foreach (Quest quest in quests) {
                data.quests.Add(new PersistentQuestData(quest));
            }
            data.firstLoad = _firstLoad;
        }
    }
}