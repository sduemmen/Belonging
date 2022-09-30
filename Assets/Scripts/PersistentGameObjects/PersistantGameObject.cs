using System;
using SaveSystem.Data;
using UnityEngine;

namespace PersistentGameObjects
{
    [CreateAssetMenu(menuName = "Custom/PersistentGameObject"), Serializable]
    public class PersistentGameObject : ScriptableObject
    {
        [SerializeField] protected PersistentGameObjectType _type;
        [SerializeField] protected GameObject _prefab;

        #region -- Getters --

        public PersistentGameObjectType GetPersistantGameObjectType()
        {
            return _type;
        }
        
        public GameObject GetPrefab()
        {
            return _prefab;
        }

        #endregion
    }
}