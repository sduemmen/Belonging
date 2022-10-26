using System;
using System.Collections.Generic;
using QuestSystem.QuestBehaviours;
using SaveSystem;
using SaveSystem.Data;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace QuestSystem
{
    [Serializable]
    public class QuestManager : MonoBehaviour, IDataPersistence
    {
        [SerializeField, InlineEditor] private List<Quest> quests = new List<Quest>();
        [SerializeField] private bool _firstLoad = true;

        [Button("Load Quests from Assets")]
        private void LoadQuests()
        {
            string[] questAssets = AssetDatabase.FindAssets("t:Quest");
            
            foreach (var s in questAssets) {
                string path = AssetDatabase.GUIDToAssetPath(s);
                Quest q = AssetDatabase.LoadAssetAtPath<Quest>(path);
                Quest quest = (Quest)ScriptableObject.CreateInstance(typeof(Quest));
                quest.title = q.title;
                quest.description = q.description;
                quest.completed = q.completed;
                quest.questBehaviour = QuestBehaviour.Create(q.questBehaviour.GetType());
                quest.questBehaviour.Initialize(q.questBehaviour);
                quest.completionBehaviours = q.completionBehaviours;
                quest.OnCompleteQuestCallback = q.OnCompleteQuestCallback;
                quests.Add(quest);
            }
        }

        public void LoadData(GameData data)
        {
            quests.Clear();
            
            if (data.firstLoad) {
                LoadQuests();
            } else {
                foreach (var questData in data.quests) {
                    Quest quest = (Quest)ScriptableObject.CreateInstance(typeof(Quest));
                    quest.title = questData.title;
                    quest.description = questData.description;
                    quest.completed = questData.completed;
                    quest.questBehaviour = questData.questBehaviour;
                    quest.completionBehaviours = questData.questCompletionBehaviours;
                    quest.OnCompleteQuestCallback = questData.OnCompleteQuestCallback;
                    quests.Add(quest);
                }
            }

            foreach (Quest quest in quests) {
                quest.Initialize();
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