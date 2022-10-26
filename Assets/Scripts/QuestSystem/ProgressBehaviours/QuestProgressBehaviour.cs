using UnityEngine;

namespace QuestSystem.ProgressBehaviours
{
    public abstract class QuestProgressBehaviour : ScriptableObject
    {
        public abstract int GetNewProgress(int current, int progress);
        public abstract float GetNewProgress(float current, float progress);
        public abstract double GetNewProgress(double current, double progress);
    }
}