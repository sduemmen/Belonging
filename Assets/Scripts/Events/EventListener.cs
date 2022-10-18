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
}