using System;
using System.Collections.Generic;
using Environment;
using UnityEngine;

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

        private float rotationDampen = .3f;
        public Quaternion targetRotation = Quaternion.identity;

        private void Awake()
        {
            foreach (MeshRenderer meshRenderer in _renderers) {
                _defaultMaterials.Add(meshRenderer.material);
                meshRenderer.material = _placementOkMaterial;
            }

            _collider.enabled = false;
            transform.GetComponent<Destroyable>().colliders.SetActive(false);
        }

        private void Update()
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationDampen);
        }

        private void OnDestroy()
        {
            transform.GetComponent<Destroyable>().colliders.SetActive(true);
        }

        public void RotatePreview(int deg)
        {
            targetRotation *= Quaternion.Euler(0, deg, 0);
        }

        public void SetMaterial(Material material)
        {
            foreach (MeshRenderer meshRenderer in _renderers) {
                meshRenderer.material = material;
            }
        }

        public void ResetMaterial()
        {
            for (int i = 0; i < _renderers.Count; i++) {
                _renderers[i].material = _defaultMaterials[i];
            }
            _collider.enabled = true;
        }

        private void OnTriggerStay(Collider other)
        { 
            if (!other.CompareTag("Environment")) return;
            
            foreach (MeshRenderer meshRenderer in _renderers) {
                meshRenderer.material = _placementBlockedMaterial;
            }
            
            canBePlaced = false;
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.CompareTag("Environment")) return;
            
            foreach (MeshRenderer meshRenderer in _renderers) {
                meshRenderer.material = _placementOkMaterial;
            }
            
            canBePlaced = true;
        }
    }
}