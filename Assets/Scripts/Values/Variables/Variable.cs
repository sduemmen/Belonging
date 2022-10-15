using UnityEngine;

namespace Values.Variables
{
    public abstract class Variable<T> : ScriptableObject
    {
        public T defaultValue;
        public T value;
        public bool resetOnRestarts = true;

        private void OnEnable()
        {
            if (resetOnRestarts) value = defaultValue;
        }
    }
}