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
        [SerializeField, InlineEditor] private List<Quest> questDatabase = new List<Quest>();
        [SerializeField, InlineEditor] public List<Quest> quests = new List<Quest>(); 
        [SerializeField] private bool _firstLoad = true;
        [SerializeField] private QuestDisplayController _questDisplayController;

        private static QuestManager _instance;

        public static QuestManager Instance {
            get {
                if (_instance == null) {
                    _instance = (QuestManager)FindObjectOfType(typeof(QuestManager));
                }

                return _instance;
            }
        }

#if UNITY_EDITOR
        [Button("Load Quests from Assets")]
        private void LoadQuestsFromAssets()
        {
            questDatabase.Clear();
            quests.Clear();
            
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
                questDatabase.Add(quest);
            }
            
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
#endif
        
        [Button("Initialize Quest Events and Listeners")]
        private void InitializeQuests()
        {
            foreach (var quest in quests) {
                quest.Initialize();
            }
        }
        
        public void LoadData(GameData data)
        {
            quests.Clear();
            
            if (data.firstLoad) {
                foreach (var questData in questDatabase) {
                    Quest quest = (Quest)ScriptableObject.CreateInstance(typeof(Quest));
                    quest.title = questData.title;
                    quest.description = questData.description;
                    quest.completed = questData.completed;
                    quest.questBehaviour = QuestBehaviour.Create(questData.questBehaviour.GetType());
                    quest.questBehaviour.Initialize(questData.questBehaviour);
                    quest.completionBehaviours = questData.completionBehaviours;
                    quest.OnCompleteQuestCallback = questData.OnCompleteQuestCallback;
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
                    quests.Add(quest);
                }
            }

            InitializeQuests();
            
            _firstLoad = false;
            _questDisplayController.Initialize();
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