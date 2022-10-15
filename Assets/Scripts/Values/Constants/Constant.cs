using UnityEngine;

namespace Values.Constants
{
    public abstract class Constant<T> : ScriptableObject
    {
        public T value;
    }
}