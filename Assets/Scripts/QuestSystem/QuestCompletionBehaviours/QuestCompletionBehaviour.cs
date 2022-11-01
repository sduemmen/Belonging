using System;
using System.Collections.Generic;
using Events.Events;
using InventorySystem;

namespace QuestSystem.QuestCompletionBehaviours
{
    public enum CompletionBehaviour
    {
        Simple,
        Item,
        Int,
        Float,
        String
    }

    [Serializable]
    public abstract class QuestCompletionBehaviour
    {
        public abstract void OnComplete();
    }

    [Serializable]
    public class SimpleCompletionBehaviour : QuestCompletionBehaviour
    {
        public SimpleEvent completionEvent;

        public override void OnComplete()
        {
            completionEvent.Raise();
        }
    }

    [Serializable]
    public class ItemCompletionBehaviour : QuestCompletionBehaviour
    {
        public ItemEvent completionEvent;
        public List<InventorySlot> callEventWithValues;

        public override void OnComplete()
        {
            foreach (InventorySlot reward in callEventWithValues)
                for (int i = 0; i < reward.StackSize; i++)
                    completionEvent.Raise(reward.Item);
        }
    }

    [Serializable]
    public class IntCompletionBehaviour : QuestCompletionBehaviour
    {
        public FloatEvent completionEvent;
        public List<int> callEventWithValues;

        public override void OnComplete()
        {
            foreach (int i in callEventWithValues) completionEvent.Raise(i);
        }
    }

    [Serializable]
    public class FloatCompletionBehaviour : QuestCompletionBehaviour
    {
        public FloatEvent completionEvent;
        public List<float> callEventWithValues;

        public override void OnComplete()
        {
            foreach (float f in callEventWithValues) completionEvent.Raise(f);
        }
    }

    [Serializable]
    public class StringCompletionBehaviour : QuestCompletionBehaviour
    {
        public StringEvent completionEvent;
        public List<string> callEventWithValues;

        public override void OnComplete()
        {
            foreach (string s in callEventWithValues) completionEvent.Raise(s);
        }
    }
}