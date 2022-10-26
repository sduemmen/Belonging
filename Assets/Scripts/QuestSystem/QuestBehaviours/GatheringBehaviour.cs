using System;
using Events.Events;
using QuestSystem.ProgressBehaviours;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using Utility;
using Comparison = Utility.Comparison;

namespace QuestSystem.QuestBehaviours
{
    [Serializable]
    public class GatheringBehaviour : QuestBehaviour
    {
        [Title("Values")]
        
        [PropertyOrder(1)]
        [HorizontalGroup("values"), VerticalGroup("values/left")]
        [SerializeField] 
        public float target;

        [PropertyOrder(2)]
        [HorizontalGroup("values"), VerticalGroup("values/left")]
        [SerializeField] 
        private float current;
        
        [Title("Default Values")]
        
        [PropertyOrder(3)]
        [HorizontalGroup("values"), VerticalGroup("values/right")]
        [SerializeField] 
        public float defaultTarget;

        [PropertyOrder(4)]
        [HorizontalGroup("values"), VerticalGroup("values/right")]
        [SerializeField] 
        private float defaultCurrent;

        [PropertyOrder(5)]
        [Button("Apply values to default values")]
        private void ApplyDefaultValue()
        {
            defaultCurrent = current;
            defaultTarget = target;
        }
        
        [PropertyOrder(11)]
        [TitleGroup("Progress")]
        [ValueDropdown("incrementOption")]
        [HideLabel]
        [SerializeField] 
        public bool useDynamicIncrement = false;
        
        [PropertyOrder(12)]
        [TitleGroup("Progress")]
        [Tooltip("The event to subscribe to")]
        [ShowIf("useDynamicIncrement")] 
        [SerializeField]
        public FloatEvent dynamicProgressEvent;

        [PropertyOrder(13)]
        [TitleGroup("Progress")]
        [Tooltip("The event to subscribe to")]
        [ShowIf("@useDynamicIncrement == false")] 
        [SerializeField]
        public SimpleEvent staticProgressEvent;
        
        [PropertyOrder(14)]
        [TitleGroup("Progress")]
        [ShowIf("@useDynamicIncrement == false")] 
        [SerializeField]
        public float progressAmount;
        
        [PropertyOrder(15)]
        [TitleGroup("Progress")]
        [SerializeField] 
        public Comparison completeCondition;

        [PropertyOrder(16)] 
        [TitleGroup("Progress")] 
        [SerializeReference]
        public QuestProgressBehaviour progressBehaviour;

        private static ValueDropdownList<bool> incrementOption = new ValueDropdownList<bool>() {
            { "Dynamic Progress", true },
            { "Static Progress", false },
        };

        [PropertyOrder(21)] 
        [TitleGroup("Broadcasting")] 
        [ShowIf("broadcastCurrentState")]
        public FloatFloatEvent broadcastEvent;
        
        [PropertyOrder(21)] 
        [TitleGroup("Broadcasting")] 
        [ShowIf("broadcastCurrentState")]
        [SerializeField]
        public UnityEvent<float, float> broadcastEventCallback;

        public void Progress(float f)
        {
            Debug.Log("dynamic");
            current = progressBehaviour.GetNewProgress(current, f);
            if (MathUtilities.Evaluate(completeCondition, current, target)) {
                OnComplete?.Invoke();
            }
        }
        
        public void Progress()
        {
            Debug.Log("static");
            current = progressBehaviour.GetNewProgress(current, progressAmount);
            OnUpdate();
            if (MathUtilities.Evaluate(completeCondition, current, target)) {
                OnComplete?.Invoke();
            }
        }

        public override void Initialize(QuestBehaviour questBehaviour)
        {
            if (questBehaviour.GetType() != typeof(GatheringBehaviour)) {
                throw new ArgumentException("Parameter questbehaviour is of different type");
            }

            GatheringBehaviour other = (GatheringBehaviour)questBehaviour;
            
            this.current = other.current;
            this.target = other.target;
            this.defaultCurrent = other.defaultCurrent;
            this.defaultTarget = other.defaultTarget;
            this.broadcastEvent = other.broadcastEvent;
            this.completeCondition = other.completeCondition;
            this.progressAmount = other.progressAmount;
            this.progressBehaviour = other.progressBehaviour;
            this.broadcastEventCallback = other.broadcastEventCallback;
            this.dynamicProgressEvent = other.dynamicProgressEvent;
            this.staticProgressEvent = other.staticProgressEvent;
            this.useDynamicIncrement = other.useDynamicIncrement;
        }

        public override void OnUpdate()
        {
            if (!broadcastCurrentState) return;
            
            if (broadcastEvent != null) broadcastEvent.Raise(current, target);
            broadcastEventCallback?.Invoke(current, target);
        }

        public override void Reset()
        {
            current = defaultCurrent;
            target = defaultTarget;
        }
    }
}