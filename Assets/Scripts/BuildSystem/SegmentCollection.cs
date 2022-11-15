using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using World;

namespace BuildSystem
{
    [Serializable]
    [CreateAssetMenu(menuName = "Segment Collection")]
    public class SegmentCollection : ScriptableObject
    {
        [SerializeField] private string _collectionTitle;
        [SerializeField] private List<Segment> _segments;

        public string CollectionTitle => _collectionTitle;
        public List<Segment> Segments => _segments;
        public int Count => _segments.Count;

        private void OnValidate()
        {
            foreach (Segment segment in _segments) 
            {
                segment.ApplyPrefabName();
            }
        }
        
#if UNITY_EDITOR
        [Button("Set up item drops in prefabs")]
        public void SetupItemDrops()
        {
            foreach (Segment segment in _segments)
            {
                if (segment.Prefab.GetComponent<Destroyable>().ItemDrops != segment.BuildCosts) 
                {
                    segment.Prefab.GetComponent<Destroyable>().ItemDrops = segment.BuildCosts;
                    EditorUtility.SetDirty(segment.Prefab);
                    Debug.Log($"set up item drops on {segment.Name}");
                }
            }
        }
#endif
    }
}