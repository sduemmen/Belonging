using System;
using UnityEngine;

namespace Events.Events
{
    [CreateAssetMenu(menuName = "Events/Integer Event"), Serializable]
    public class IntEvent : Event<int>
    {
        
    }
}