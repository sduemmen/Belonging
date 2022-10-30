using System;
using QuestSystem.QuestBehaviours;
using UnityEngine;

namespace QuestSystem
{
    public class QuestDisplayController : MonoBehaviour
    {
        public GameObject questDisplay;
        public GameObject questContentHolder;
        public GameObject uiQuestPrefab;

        public void Initialize()
        {
            foreach (Quest quest in QuestManager.Instance.quests) {
                if (quest.completed) continue;
                
                GameObject questObj = Instantiate(uiQuestPrefab, questContentHolder.transform, false);
                UIQuest uiQuest = questObj.GetComponent<UIQuest>();
                uiQuest.questTitleLabel.text = quest.title;
                uiQuest.questDescriptionLabel.text = quest.description;
                
                if (quest.questBehaviour.broadcastCurrentState) {
                    if (quest.questBehaviour.GetType() == typeof(GatheringBehaviour)) {
                        GatheringBehaviour behaviour = (GatheringBehaviour)quest.questBehaviour;
                        behaviour.broadcastEventCallback.AddListener(uiQuest.SetProgress);
                        uiQuest.SetProgress(behaviour.current, behaviour.target);
                    }
                } else {
                    uiQuest.questProgressLabel.gameObject.SetActive(false);
                }

                quest.OnCompleteQuestCallback.AddListener(uiQuest.OnComplete);
            }
        }
        
        public void Show()
        {
            questDisplay.SetActive(true);
        }

        public void Hide()
        {
            questDisplay.SetActive(false);
        }
    }
}