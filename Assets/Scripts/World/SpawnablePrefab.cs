using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace World
{
    [Serializable]
    public class SpawnablePrefab
    {
        [SerializeField, PreviewField(100)] private GameObject _prefab;
        [Range(0, 1)] public float spawnChance;

        public GameObject Prefab => _prefab;
    }
}