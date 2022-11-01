using System;
using UnityEngine;

namespace QuestSystem.ProgressBehaviours
{
    [CreateAssetMenu(menuName = "Quests/Behaviours/Progress/Increment")]
    [Serializable]
    public class QuestIncrementBehaviour : QuestProgressBehaviour
    {
        public override int GetNewProgress(int current, int progress)
        {
            return current + progress;
        }

        public override float GetNewProgress(float current, float progress)
        {
            return current + progress;
        }

        public override double GetNewProgress(double current, double progress)
        {
            return current + progress;
        }
    }
}