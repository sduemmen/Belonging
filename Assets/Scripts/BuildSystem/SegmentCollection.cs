using System;
using System.Collections.Generic;
using UnityEngine;

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
    }
}