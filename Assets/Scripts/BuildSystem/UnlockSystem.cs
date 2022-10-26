using System;
using System.Collections.Generic;
using System.Linq;
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

        private static UnlockSystem _instance;

        public static UnlockSystem Instance {
            get {
                if (_instance == null)
                    _instance = (UnlockSystem)FindObjectOfType(typeof(UnlockSystem));
                return _instance;
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
            for (int i = 0; i < _segmentUnlockData.Count; i++) {
                if (segmentName == _segmentUnlockData[i].segmentName) {
                    _segmentUnlockData[i].unlocked = true;
                    Debug.Log("Unlocked " + segmentName);
                }
            }
        }

        public bool SegmentUnlocked(string segmentName)
        {
            return _segmentUnlockData.Find(data => data.segmentName == segmentName).unlocked;
        }
        
        public void LoadData(GameData data)
        {
            _segmentUnlockData = data.segmentUnlockData;
        }

        public void SaveData(ref GameData data)
        {
            data.segmentUnlockData = _segmentUnlockData;
        }
    }
}