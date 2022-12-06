using System;
using System.Collections.Generic;
using QuestSystem.QuestBehaviours;
using SaveSystem.Data;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace QuestSystem
{
    [Serializable]
    public class QuestController : Controller, IDisplayContext
    {
        private static QuestController _instance;
        public static QuestController Instance {
            get {
                if (_instance == null) _instance = (QuestController)FindObjectOfType(typeof(QuestController));
                return _instance;
            }
        }
        
        [SerializeField] [InlineEditor] private List<Quest> questDatabase = new();
        [SerializeField] [InlineEditor] public List<Quest> quests = new();
        [SerializeField] private bool _firstLoad = true;
        [SerializeField] private GameObject _uiQuestDisplayContext;
        [SerializeField] private GameObject _uiQuestTarget;
        [SerializeField] private GameObject _uiQuestPrefab;

        private bool _displayContextActive;
        public bool DisplayContextActive => _displayContextActive;

#if UNITY_EDITOR
        [Button("Load Quests from Assets")]
        private void LoadQuestsFromAssets()
        {
            questDatabase.Clear();
            quests.Clear();

            string[] questAssets = AssetDatabase.FindAssets("t:Quest");

            foreach (string s in questAssets)
            {
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

            foreach (string s in questAssets)
            {
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
        protected override void OnLoadCompleted()
        {
            foreach (Quest quest in quests)
            {
                quest.Initialize();
            }
            
            foreach (Quest quest in quests)
            {
                if (quest.completed) continue;

                GameObject questObj = Instantiate(_uiQuestPrefab, _uiQuestTarget.transform, false);
                UIQuest uiQuest = questObj.GetComponent<UIQuest>();
                uiQuest.questTitleLabel.text = quest.title.Substring("Unlock ".Length);
                uiQuest.questDescriptionLabel.text = quest.description;

                if (quest.questBehaviour.broadcastCurrentState)
                {
                    if (quest.questBehaviour.GetType() == typeof(GatheringBehaviour))
                    {
                        GatheringBehaviour behaviour = (GatheringBehaviour)quest.questBehaviour;
                        behaviour.broadcastEventCallback.AddListener(uiQuest.SetProgress);
                        uiQuest.SetProgress(behaviour.current, behaviour.target);
                    }
                }
                else
                {
                    uiQuest.questProgressLabel.gameObject.SetActive(false);
                }

                quest.OnCompleteQuestCallback.AddListener(uiQuest.OnComplete);
            }
        }

        private void Update()
        {
            CheckInput();
        }

        private void CheckInput()
        {
            if (Flags.GAME_PAUSED)
            {
                return;
            }
            
            if (InputSystem.GetKeyDown(InputSystem.KeyBinds.Toggle_Quest_Display))
            {
                if (_displayContextActive)
                {
                    HideDisplayContext();
                }
                else
                {
                    ShowDisplayContext();
                }
            }
            else if (InputSystem.GetKeysDown(InputSystem.KeyBinds.Toggle_Inventory, InputSystem.KeyBinds.Open_Build_Menu, InputSystem.KeyBinds.EquipUnequip_Axe, InputSystem.KeyBinds.EquipUnequip_Pickaxe))
            {
                HideDisplayContext();
            }
            else if (InputSystem.GetKeyDown(InputSystem.KeyBinds.Pause_Game) && _displayContextActive)
            {
                HideDisplayContext();
            }
        }

        public void ShowDisplayContext()
        {
            _displayContextActive = true;
            
            _uiQuestDisplayContext.SetActive(true);
        }

        public void HideDisplayContext()
        {
            _displayContextActive = false;
            
            _uiQuestDisplayContext.SetActive(false);
        }

        public override void LoadData(GameData data)
        {
            quests.Clear();

            if (data.firstLoad)
            {
                foreach (Quest questData in questDatabase)
                {
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
            }
            else
            {
                foreach (PersistentQuestData questData in data.quests)
                {
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
            
            _firstLoad = false;
            base.LoadData(data);
        }
        
        public override void SaveData(ref GameData data)
        {
            data.quests.Clear();
            foreach (Quest quest in quests) data.quests.Add(new PersistentQuestData(quest));
            data.firstLoad = _firstLoad;
        }
    }
}