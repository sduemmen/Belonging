using UnityEngine;

namespace Player
{
    public class PlayerWorldBuilding : MonoBehaviour
    {
        [SerializeField] private float _maxInteractionDistance;
        [SerializeField] private LayerMask _buildModeLayerMask;
        [SerializeField] private LayerMask _deleteModeLayerMask;
        [SerializeField] private int _defaultLayerInt;
        [SerializeField] private Material _buildingMatPos;
        [SerializeField] private Material _buildingMatNeg;
        
        public float MaxInteractionDistance => _maxInteractionDistance;
        public LayerMask BuildModeLayerMask => _buildModeLayerMask;
        public LayerMask DeleteModeLayerMask => _deleteModeLayerMask;

        public GameObject testingObjectPrefab;
        public GameObject previewGameObject;
    }
}