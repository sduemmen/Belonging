using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Events
{
    public abstract class Event<T> : ScriptableObject
    {
        [SerializeField] private List<EventListener<T>> _listeners = new List<EventListener<T>>();
        [SerializeField] private bool logging;

        [Button("Raise Event")]
        public void Raise(T t)
        {
            if (logging) Debug.Log($"Event raised on {this.name} with parameter {t}");
            for (int i = _listeners.Count - 1; i >= 0; i--) {
                _listeners[i].OnNotify(t);
            }
        }
        
        public void RegisterListener(EventListener<T> listener) { if (!_listeners.Contains(listener)) _listeners.Add(listener); }
        
        public void UnregisterListener(EventListener<T> listener) { if (_listeners.Contains(listener)) _listeners.Remove(listener); }
    }
}