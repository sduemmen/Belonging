using System;
using System.Collections.Generic;
using Events.Listeners;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace Events.Events
{
    [CreateAssetMenu(menuName = "Events/Simple Event"), Serializable]
    public class SimpleEvent : ScriptableObject
    {
        [SerializeField] private List<SimpleEventListener> _listeners = new List<SimpleEventListener>();
        [SerializeField] private bool logging;
        public UnityAction callback;

        [Button("Raise Event")]
        public void Raise()
        {
            if (logging) Debug.Log($"Event raised on {this.name}");
            for (int i = _listeners.Count - 1; i >= 0; i--) {
                _listeners[i].OnNotify();
            }
            callback?.Invoke();
        }
        
        public void RegisterListener(SimpleEventListener listener) { if (!_listeners.Contains(listener)) _listeners.Add(listener); }
        
        public void UnregisterListener(SimpleEventListener listener) { if (_listeners.Contains(listener)) _listeners.Remove(listener); }
    }
}