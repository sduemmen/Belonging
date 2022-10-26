using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace QuestSystem.QuestBehaviours
{
    [Serializable]
    public abstract class QuestBehaviour
    {
        [TitleGroup("Broadcasting")] 
        [PropertyOrder(20)]
        [SerializeField]
        public bool broadcastCurrentState;

        public UnityAction OnComplete;

        public abstract void OnUpdate();
        public abstract void Reset();
    }
}