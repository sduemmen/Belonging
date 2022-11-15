using System;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

namespace World
{
    [Serializable, CreateAssetMenu(menuName = "Collection/Spawnable Prefabs")]
    public class SpawnablePrefabCollection : ScriptableObject
    {
        [SerializeField] private List<SpawnablePrefab> _prefabs = new List<SpawnablePrefab>();
        [SerializeField] private List<float> _spawnChanceCheck = new List<float>();
        [SerializeField] private List<float> _sortedCumulativeSums = new List<float>();

        public List<SpawnablePrefab> Prefabs => _prefabs;

        private void OnValidate()
        {
            // initialize checklist if count is out of sync
            if (_spawnChanceCheck.Count != _prefabs.Count)
            {
                _spawnChanceCheck.Clear();
                foreach (SpawnablePrefab spawnablePrefab in _prefabs)
                {
                    _spawnChanceCheck.Add(spawnablePrefab.spawnChance);
                }
            }
            
            float summedProbability = 0;
            int changedIndex = -1;

            for (int i = 0; i < _prefabs.Count; i++)
            {
                float spawnChance = _prefabs[i].spawnChance;
                summedProbability += spawnChance;
                
                if (Math.Abs(spawnChance - _spawnChanceCheck[i]) > .0001f)
                {
                    changedIndex = i;
                }
            }

            if (changedIndex == -1) return;

            if (Math.Abs(_prefabs[changedIndex].spawnChance - 1) < 0.0000001f)
            {
                for (int i = 0; i < _prefabs.Count; i++)
                {
                    if (i != changedIndex)
                    {
                        _prefabs[i].spawnChance = 0;
                    }
                }
            }
            
            float adjustmentPerEntry = 0;
            if (summedProbability > 1) adjustmentPerEntry = -(summedProbability - 1) / (_prefabs.Count - 1);
            if (summedProbability < 1) adjustmentPerEntry = (1 - summedProbability) / (_prefabs.Count - 1);

            for (int i = 0; i < _prefabs.Count; i++)
            {
                if (i == changedIndex) continue;

                SpawnablePrefab spawnablePrefab = _prefabs[i];
                float valueAfterAdjustment = spawnablePrefab.spawnChance + adjustmentPerEntry;

                if (valueAfterAdjustment > 1 || valueAfterAdjustment < 0)
                {
                    valueAfterAdjustment = Mathf.Clamp01(valueAfterAdjustment);
                }

                spawnablePrefab.spawnChance = valueAfterAdjustment;
            }

            _spawnChanceCheck.Clear();
            _sortedCumulativeSums.Clear();
            float cumulativeSum = 0;
            foreach (SpawnablePrefab spawnablePrefab in _prefabs)
            {
                cumulativeSum += spawnablePrefab.spawnChance;
                _spawnChanceCheck.Add(spawnablePrefab.spawnChance);
                _sortedCumulativeSums.Add(cumulativeSum);
            }
        }

        public GameObject GetRandom(Random random)
        {
            double sample = random.NextDouble();

            for (int i = 0; i < _sortedCumulativeSums.Count; i++)
            {
                float spawnChance = _sortedCumulativeSums[i];
                if (sample < spawnChance)
                {
                    return _prefabs[i].Prefab;
                }
            }

            return null;

            // int left = 0;
            // int right = _sortedCumulativeSums.Count - 1;
            // int mid = 0;
            //
            // do
            // {
            //     mid = left + (right - left) / 2;
            //
            //     if (sample < _sortedCumulativeSums[mid])
            //     {
            //         left = mid + 1;
            //     }
            //     else
            //     {
            //         right = mid - 1;
            //     }
            //
            //     if (Math.Abs(_sortedCumulativeSums[mid] - sample) < 0.00000001f)
            //     {
            //         break;
            //     }
            // } while (left <= right);
            //
            // return _prefabs.Find(entry => Math.Abs(entry.spawnChance - _sortedCumulativeSums[mid - 1]) < 0.00000001f).Prefab;
        }
    }
}