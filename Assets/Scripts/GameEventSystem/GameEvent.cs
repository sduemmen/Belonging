using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GameEventSystem
{
    [CreateAssetMenu(menuName = "Custom/Game Event")]
    public class GameEvent : ScriptableObject
    {
        private List<GameEventListener> _gameEventListeners = new List<GameEventListener>();

        [Button("Raise Event")]
        public void Raise()
        {
            // Debug.Log("Event raised on " + this.name);
            for (int i = _gameEventListeners.Count - 1; i >= 0; i--) {
                _gameEventListeners[i].OnNotify();
            }
        }
        
        public void RegisterListener(GameEventListener listener) { if (!_gameEventListeners.Contains(listener)) _gameEventListeners.Add(listener); }
        
        public void UnregisterListener(GameEventListener listener) { if (_gameEventListeners.Contains(listener)) _gameEventListeners.Remove(listener); }
    }
}