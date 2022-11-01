using System;
using UnityEngine;

namespace Events.Events
{
    [CreateAssetMenu(menuName = "Events/Tooltip")]
    [Serializable]
    public class TooltipEvent : Event<GameObject>
    { }
}