using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Collections
{
    [Serializable]
    public abstract class Collection<T> : ScriptableObject where T : CollectionEntry
    {
        [SerializeField] public string collectionTitle;
        [SerializeField] public List<T> entries;

        [Button("Verify Entries")]
        protected virtual void VerifyEntries()
        {
            for (int i = 0; i < entries.Count; i++) {
                if (entries.FindAll(entry => entry.Equals(entries[i])).Count > 1) {
                    Debug.LogWarning($"found duplicate entry at index {i}");
                }
            }
        }
    }
}