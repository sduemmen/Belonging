using System;
using UnityEngine;

namespace Player
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

        private void Awake()
        {
            _collider = GetComponent<MeshCollider>();
            _renderer = GetComponentInChildren<MeshRenderer>();
            _defaultMaterial = _renderer.material;
            _renderer.material = _placementOkMaterial;
        }

        public void SetMaterial(Material material)
        {
            _renderer.material = material;
        }

        public void ResetMaterial()
        {
            _renderer.material = _defaultMaterial;
        }

        private void OnTriggerStay(Collider other)
        {
            _renderer.material = _placementBlockedMaterial;
            canBePlaced = false;
        }

        private void OnTriggerExit(Collider other)
        {
            _renderer.material = _placementOkMaterial;
            canBePlaced = true;
        }
    }
}