using System;
using UnityEngine;

namespace Events.Events
{
    [CreateAssetMenu(menuName = "Events/String Event")]
    [Serializable]
    public class StringEvent : Event<string>
    { }
}