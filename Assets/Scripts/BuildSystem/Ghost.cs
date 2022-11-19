using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace BuildSystem
{
    [Serializable]
    public class Ghost : MonoBehaviour
    {
        [SerializeField, TitleGroup("Settings")] private MeshCollider _collider;
        [SerializeField, TitleGroup("Settings")] private List<MeshRenderer> _renderers;
        [SerializeField, TitleGroup("Settings")] private List<Material> _defaultMaterials;

        [Button("Initialize References"), PropertyOrder(-1)]
        private void InitializeReferences()
        {
            _collider = GetComponentInChildren<MeshCollider>();
            _renderers = GetComponentsInChildren<MeshRenderer>().ToList();

            _defaultMaterials.Clear();
            foreach (MeshRenderer meshRenderer in _renderers)
            {
                _defaultMaterials.Add(meshRenderer.sharedMaterial);
            }
        }

        private void Awake()
        {
            _collider.enabled = false;
        }

        public void SetMaterial(Material material)
        {
            foreach (MeshRenderer meshRenderer in _renderers)
            {
                meshRenderer.material = material;
            }
        }

        public void ResetMaterial()
        {
            for (int i = 0; i < _renderers.Count; i++)
            {
                _renderers[i].material = _defaultMaterials[i];
            }
            _collider.enabled = true;
        }
    }
}