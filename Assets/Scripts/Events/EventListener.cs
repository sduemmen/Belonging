using System;
using UnityEngine;
using UnityEngine.Events;

namespace Events
{
    public abstract class EventListener<T> : MonoBehaviour
    {
        [SerializeField] protected Event<T> _event;
        [SerializeField] protected UnityEvent<T> _response;

        protected void OnEnable()
        {
            _event.RegisterListener(this);
        }

        protected void OnDisable()
        {
            _event.UnregisterListener(this);
        }

        public void OnNotify(T t)
        {
            _response?.Invoke(t);
        }
    }
    
    [Serializable]
    public abstract class AdvancedEventListener<S, T> : MonoBehaviour
    {
        [SerializeField] protected AdvancedEvent<S, T> _event;
        [SerializeField] protected UnityEvent<S, T> _response;

        protected void OnEnable()
        {
            _event.RegisterListener(this);
        }

        protected void OnDisable()
        {
            _event.UnregisterListener(this);
        }

        public void OnNotify(S s, T t)
        {
            _response?.Invoke(s, t);
        }
    }
}