using System;
using System.Collections.Generic;
using BuildSystem;
using Environment;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Collections
{
    [Serializable]
    public class SegmentCollectionEntry : CollectionEntry
    {
        [SerializeField] public string displayName;
        [SerializeField] public string prefabName;
        [SerializeField] public Destroyable prefab;
        [SerializeField] public Sprite previewImage;
        [SerializeField] public List<BuildCost> buildCosts;

        [Button("Set Prefab Name")]
        public void SetPrefabName()
        {
            if (prefab != null) prefabName = prefab.name;
        }

        public override bool Equals(object obj)
        {
            if (obj is not SegmentCollectionEntry) return false;
            SegmentCollectionEntry other = (SegmentCollectionEntry)obj;
            return prefab == other.prefab && prefabName == other.prefabName && previewImage == other.previewImage;
        }

        protected bool Equals(SegmentCollectionEntry other)
        {
            return prefabName == other.prefabName && Equals(prefab, other.prefab) && Equals(previewImage, other.previewImage);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(prefabName, prefab, previewImage);
        }
    }
}