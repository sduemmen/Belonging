using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using World;

namespace BuildSystem
{
    [Serializable]
    public class SegmentPreview : MonoBehaviour
    {
        [SerializeField, TitleGroup("Settings")] private MeshCollider _collider;
        [SerializeField, TitleGroup("Settings")] private List<MeshRenderer> _renderers;
        [SerializeField, TitleGroup("Settings")] private List<Material> _defaultMaterials;
        [SerializeField, TitleGroup("Settings")] private Material _placementBlockedMaterial;
        [SerializeField, TitleGroup("Settings")] private Material _placementOkMaterial;
        
        [TitleGroup("Internals")] public bool canBePlaced = true;
        [TitleGroup("Internals")] public bool canBeDestroyed = false;
        [TitleGroup("Internals")] public bool isPlaced = false;
        private Quaternion _targetRotation = Quaternion.identity;
        private float rotationDampen = .3f;

        [Button("Initialize References"), PropertyOrder(-1)]
        private void InitializeReferences()
        {
            _collider = GetComponentInChildren<MeshCollider>();
            _renderers = GetComponentsInChildren<MeshRenderer>().ToList();

            _defaultMaterials.Clear();
            foreach (MeshRenderer meshRenderer in _renderers)
            {
                _defaultMaterials.Add(meshRenderer.material);
            }
            
            _placementOkMaterial = Resources.Load<Material>("Materials/Shaders/GreenFresnel.mat");
            _placementBlockedMaterial = Resources.Load<Material>("Materials/Shaders/RedFresnel.mat");
        }

        private void Awake()
        {
            foreach (MeshRenderer meshRenderer in _renderers)
            {
                meshRenderer.material = _placementOkMaterial;
            }

            _collider.enabled = false;
            transform.GetComponent<Destroyable>().SegmentColliders.SetActive(false);
        }

        private void Update()
        {
            if (isPlaced) return;
            transform.rotation = Quaternion.Lerp(transform.rotation, _targetRotation, rotationDampen);
        }

        private void OnTriggerExit(Collider other)
        {
            if (isPlaced || !other.CompareTag("Environment")) return;

            canBePlaced = true;
            UpdateMaterial();
        }

        private void OnTriggerStay(Collider other)
        {
            if (isPlaced || !other.CompareTag("Environment")) return;

            canBePlaced = false;
            UpdateMaterial();
        }

        public void RotatePreview(int deg)
        {
            if (isPlaced) return;
            _targetRotation *= Quaternion.Euler(0, deg, 0);
        }

        public void UpdateMaterial()
        {
            Material previewMaterial = !canBePlaced || canBeDestroyed ? _placementBlockedMaterial : _placementOkMaterial;
            
            foreach (MeshRenderer meshRenderer in _renderers)
            {
                meshRenderer.material = previewMaterial;
            }
        }

        public void ResetMaterial()
        {
            for (int i = 0; i < _renderers.Count; i++)
            {
                _renderers[i].material = _defaultMaterials[i];
            }
            _collider.enabled = true;
            transform.GetComponent<Destroyable>().SegmentColliders.SetActive(true);
        }
    }
}