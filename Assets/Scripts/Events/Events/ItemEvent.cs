using System;
using InventorySystem.Items;
using UnityEngine;

namespace Events.Events
{
    [CreateAssetMenu(menuName = "Events/Item Event"), Serializable]
    public class ItemEvent : Event<ItemObject>
    {
        
    }
}