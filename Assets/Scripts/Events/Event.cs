using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace Events
{
    public abstract class Event<T> : ScriptableObject
    {
        [SerializeField] private List<EventListener<T>> _listeners = new();
        [SerializeField] private bool logging;
        public UnityAction<T> callback;

        [Button("Raise Event")]
        public void Raise(T t)
        {
            if (logging) Debug.Log($"Event raised on {name} with parameter {t}");
            for (int i = _listeners.Count - 1; i >= 0; i--) _listeners[i].OnNotify(t);
            callback?.Invoke(t);
        }

        public void RegisterListener(EventListener<T> listener)
        {
            if (!_listeners.Contains(listener)) _listeners.Add(listener);
        }

        public void UnregisterListener(EventListener<T> listener)
        {
            if (_listeners.Contains(listener)) _listeners.Remove(listener);
        }
    }

    public abstract class AdvancedEvent<S, T> : ScriptableObject
    {
        [SerializeField] private List<AdvancedEventListener<S, T>> _listeners = new();
        [SerializeField] private bool logging;

        [Button("Raise Event")]
        public void Raise(S s, T t)
        {
            if (logging) Debug.Log($"Event raised on {name} with parameter {t}");
            for (int i = _listeners.Count - 1; i >= 0; i--) _listeners[i].OnNotify(s, t);
        }

        public void RegisterListener(AdvancedEventListener<S, T> listener)
        {
            if (!_listeners.Contains(listener)) _listeners.Add(listener);
        }

        public void UnregisterListener(AdvancedEventListener<S, T> listener)
        {
            if (_listeners.Contains(listener)) _listeners.Remove(listener);
        }
    }
}