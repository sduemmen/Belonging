using System;
using System.Collections.Generic;
using UnityEngine;

namespace BuildSystem
{
    [Serializable]
    public class Segment
    {
        [SerializeField] private string _name;
        [SerializeField] private GameObject _prefab;
        [SerializeField] private Sprite _previewImage;
        [SerializeField] private List<ItemStack> _buildCosts;

        public string Name => _name;
        public GameObject Prefab => _prefab;
        public Sprite PreviewImage => _previewImage;
        public List<ItemStack> BuildCosts => _buildCosts;

        public void ApplyPrefabName()
        {
            if (_prefab == null) return;
            
            _name = _prefab.name;
        }
    }
}