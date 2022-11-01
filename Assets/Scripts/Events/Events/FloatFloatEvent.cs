using System;
using UnityEngine;

namespace Events.Events
{
    [CreateAssetMenu(menuName = "Events/Advanced/Float Float Event")]
    [Serializable]
    public class FloatFloatEvent : AdvancedEvent<float, float>
    { }
}