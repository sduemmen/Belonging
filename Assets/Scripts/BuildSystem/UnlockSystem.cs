using System;
using System.Collections.Generic;
using Collections;
using SaveSystem;
using SaveSystem.Data;
using Sirenix.OdinInspector;
using UnityEngine;

namespace BuildSystem
{
    [Serializable]
    public class UnlockSystem : MonoBehaviour, IDataPersistence
    {
        [SerializeField] private List<SegmentUnlockData> _segmentUnlockData;
        [SerializeField] private List<SegmentCollection> _segmentCollections;
        private Dictionary<string, bool> _segmentUnlockDict;

        private static UnlockSystem _instance;

        public static UnlockSystem Instance {
            get {
                if (_instance == null)
                    _instance = (UnlockSystem)FindObjectOfType(typeof(UnlockSystem));
                return _instance;
            }
        }

        private void Awake()
        {
            _segmentUnlockDict = new Dictionary<string, bool>();
            foreach (SegmentUnlockData unlockData in _segmentUnlockData) {
                _segmentUnlockDict.Add(unlockData.segmentName, unlockData.unlocked);
            }
        }

        [Button("Initialize")]
        public void Initialize()
        {
            foreach (SegmentCollection collection in _segmentCollections) {
                foreach (SegmentCollectionEntry segment in collection.entries) {
                    if (!_segmentUnlockData.Exists(data => data.segmentName == segment.displayName))
                        _segmentUnlockData.Add(new SegmentUnlockData(segment.displayName));
                }
            }
        }

        public void OnUnlockSegment(string segmentName)
        {
            _segmentUnlockDict[segmentName] = true;
        }

        public bool SegmentUnlocked(string segmentName)
        {
            return _segmentUnlockDict[segmentName];
        }
        
        public void LoadData(GameData data)
        {
            _segmentUnlockData = data.segmentUnlockData;
            foreach (SegmentUnlockData unlockData in _segmentUnlockData) {
                _segmentUnlockDict.TryAdd(unlockData.segmentName, unlockData.unlocked);
            }
        }

        public void SaveData(ref GameData data)
        {
            _segmentUnlockData.Clear();

            foreach (var entry in _segmentUnlockDict) {
                _segmentUnlockData.Add(new SegmentUnlockData(entry.Key, entry.Value));
            }
            
            data.segmentUnlockData = _segmentUnlockData;
        }
    }
}