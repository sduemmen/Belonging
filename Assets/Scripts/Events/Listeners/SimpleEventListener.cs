using System;
using Events.Events;
using UnityEngine;
using UnityEngine.Events;

namespace Events.Listeners
{
    [Serializable]
    public class SimpleEventListener : MonoBehaviour
    {
        [SerializeField] protected SimpleEvent _event;
        [SerializeField] protected UnityEvent _response;

        protected void OnEnable()
        {
            _event.RegisterListener(this);
        }

        protected void OnDisable()
        {
            _event.UnregisterListener(this);
        }

        public void OnNotify()
        {
            _response?.Invoke();
        }
    }
}