using System;
using System.Collections.Generic;
using QuestSystem.QuestBehaviours;
using QuestSystem.QuestCompletionBehaviours;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace QuestSystem
{
    [CreateAssetMenu(menuName = "Quests/Quest"), Serializable]
    public class Quest : ScriptableObject
    {
        [PropertyOrder(1), TitleGroup("General Information")]
        [SerializeField] 
        public string title;

        [PropertyOrder(2), TitleGroup("General Information")]
        [TextArea] 
        [SerializeField] 
        public string description;
        
        [PropertyOrder(3), TitleGroup("General Information")]
        [SerializeField] 
        public bool completed;

        [PropertyOrder(11), TitleGroup("Behaviour")]
        [SerializeReference] 
        public QuestBehaviour questBehaviour;

        [PropertyOrder(21), TitleGroup("Completion")]
        [SerializeReference]
        public List<QuestCompletionBehaviour> completionBehaviours;

        [PropertyOrder(22), TitleGroup("Completion")]
        [SerializeField]
        public UnityEvent OnCompleteQuestCallback;
        
        [PropertyOrder(4)]
        [Button("Complete Quest")]
        public void OnComplete()
        {
            completed = true;
            
            OnCompleteQuestCallback?.Invoke();
            
            foreach (var completionBehaviour in completionBehaviours) {
                completionBehaviour.OnComplete();
            }
        }
        
        [PropertyOrder(5)]
        [Button("Reset Quest")]
        public void ResetQuest()
        {
            completed = false;
            
            if (questBehaviour != null)
                questBehaviour.Reset();
        }

        public void Initialize()
        {
            if (questBehaviour == null) return;
            
            questBehaviour.OnComplete += OnComplete;
            
            foreach (var completionBehaviour in completionBehaviours) {
                questBehaviour.OnComplete += completionBehaviour.OnComplete;
            }

            if (questBehaviour.GetType() == typeof(GatheringBehaviour)) {
                GatheringBehaviour gatheringBehaviour = (GatheringBehaviour)questBehaviour;
                if (gatheringBehaviour.useDynamicIncrement) {
                    gatheringBehaviour.dynamicProgressEvent.callback += gatheringBehaviour.Progress;
                } else {
                    gatheringBehaviour.staticProgressEvent.callback += gatheringBehaviour.Progress;
                }
            }
            
            questBehaviour.OnUpdate();
            Debug.Log("Quest Awake: " + title);
        }
    }
}