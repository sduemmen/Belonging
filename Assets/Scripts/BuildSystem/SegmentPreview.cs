using System;
using UnityEngine;

namespace BuildSystem
{
    [Serializable]
    public class SegmentPreview : MonoBehaviour
    {
        [SerializeField] private MeshCollider _collider;
        [SerializeField] private MeshRenderer _renderer;
        [SerializeField] private Material _defaultMaterial;
        [SerializeField] private Material _placementBlockedMaterial;
        [SerializeField] private Material _placementOkMaterial;
        public bool canBePlaced = true;

        private float rotationDampen = .3f;
        public Quaternion targetRotation = Quaternion.identity;

        private void Awake()
        {
            _defaultMaterial = _renderer.material;
            _renderer.material = _placementOkMaterial;
            _collider.enabled = false;
        }

        private void Update()
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationDampen);
        }

        public void RotatePreview(int deg)
        {
            targetRotation *= Quaternion.Euler(0, deg, 0);
        }

        public void SetMaterial(Material material)
        {
            _renderer.material = material;
        }

        public void ResetMaterial()
        {
            _renderer.material = _defaultMaterial;
            _collider.enabled = true;
        }

        private void OnTriggerStay(Collider other)
        { 
            if (!other.CompareTag("Environment")) return;
            
            _renderer.material = _placementBlockedMaterial;
            canBePlaced = false;
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.CompareTag("Environment")) return;
            
            _renderer.material = _placementOkMaterial;
            canBePlaced = true;
        }
    }
}