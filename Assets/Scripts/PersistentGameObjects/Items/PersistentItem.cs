using System;
using UnityEngine;

namespace PersistentGameObjects.Items 
{
    [CreateAssetMenu(menuName = "Custom/PersistentItem"), Serializable]
    public class PersistentItem : PersistentGameObject
    {
        [SerializeField] private Sprite _icon;
        [SerializeField] private string _displayName;
        [TextArea, SerializeField] private string _description;
        [SerializeField] private  int _maxStackSize;

        #region -- Getters --

        public Sprite GetIcon()
        {
            return _icon;
        }

        public string GetDisplayName()
        {
            return _displayName;
        }

        public string GetDescription()
        {
            return _description;
        }

        public int GetMaxStackSize()
        {
            return _maxStackSize;
        }

        #endregion
    }
}