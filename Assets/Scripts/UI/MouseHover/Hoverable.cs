using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UI.MouseHover;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI
{
    [Serializable]
    [RequireComponent(typeof(EventTrigger))]
    public abstract class Hoverable : MonoBehaviour
    {
        [InlineEditor, SerializeField, PropertyOrder(1001), TitleGroup("Hover Settings")] private List<ScriptableObject> _hoverBehaviours;

        // scale internals
        private Vector3 _originalScale;
        private int _currentAnimationID;
        
        public Vector3 OriginalScale => _originalScale;
        public int CurrentAnimationID {
            get => _currentAnimationID;
            set => _currentAnimationID = value;
        }

        // tooltip internals
        private Coroutine _activeTooltip;
        public Coroutine ActiveTooltip {
            get => _activeTooltip;
            set => _activeTooltip = value;
        }

        private List<IMouseHoverBehaviour> hoverBehaviours;

        protected virtual void Awake()
        {
            hoverBehaviours = new List<IMouseHoverBehaviour>();
            
            foreach (var behaviour in _hoverBehaviours)
            {
                hoverBehaviours.Add(behaviour as IMouseHoverBehaviour);
            }

            _originalScale = transform.localScale;
            _currentAnimationID = 0;

            EventTrigger eventTrigger = GetComponent<EventTrigger>();

            EventTrigger.Entry onHoverEnter = new EventTrigger.Entry();
            onHoverEnter.eventID = EventTriggerType.PointerEnter;
            onHoverEnter.callback.AddListener(e => OnHoverEnter());
            eventTrigger.triggers.Add(onHoverEnter);

            EventTrigger.Entry onHoverLeave = new EventTrigger.Entry();
            onHoverLeave.eventID = EventTriggerType.PointerExit;
            onHoverLeave.callback.AddListener(e => OnHoverLeave());
            eventTrigger.triggers.Add(onHoverLeave);
        }

        private void OnDisable()
        {
            foreach (ScriptableObject hoverBehaviour in _hoverBehaviours)
            {
                if (hoverBehaviour is ScaleHoverBehaviour scaleHoverBehaviour)
                {
                    this.transform.localScale = _originalScale;
                }
            }
        }

        public abstract void OnTooltipVisible(Tooltip tooltip);

        protected virtual void OnHoverEnter()
        {
            foreach (IMouseHoverBehaviour hoverBehaviour in hoverBehaviours)
            {
                hoverBehaviour.OnHoverEnter(this);
            }
        }

        private void OnHoverLeave()
        {
            foreach (IMouseHoverBehaviour hoverBehaviour in hoverBehaviours)
            {
                hoverBehaviour.OnHoverLeave(this);
            }
        }
    }
}