using System;
using System.Collections.Generic;
using UnityEngine;
using World;

namespace BuildSystem
{
    [Serializable]
    public class SegmentPreview : MonoBehaviour
    {
        [SerializeField] private MeshCollider _collider;
        [SerializeField] private List<MeshRenderer> _renderers;
        [SerializeField] private List<Material> _defaultMaterials;
        [SerializeField] private Material _placementBlockedMaterial;
        [SerializeField] private Material _placementOkMaterial;
        public bool canBePlaced = true;
        public bool canBeDestroyed = false;
        public bool isPlaced = false;
        public Quaternion targetRotation = Quaternion.identity;

        private float rotationDampen = .3f;

        private void Awake()
        {
            foreach (MeshRenderer meshRenderer in _renderers)
            {
                _defaultMaterials.Add(meshRenderer.material);
                meshRenderer.material = _placementOkMaterial;
            }

            _collider.enabled = false;
            transform.GetComponent<Destroyable>().colliders.SetActive(false);
        }

        private void Update()
        {
            if (isPlaced) return;
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationDampen);
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
            targetRotation *= Quaternion.Euler(0, deg, 0);
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
            transform.GetComponent<Destroyable>().colliders.SetActive(true);
        }
    }
}