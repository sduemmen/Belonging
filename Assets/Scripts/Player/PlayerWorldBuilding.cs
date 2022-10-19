using Flags;
using UnityEngine;

namespace Player
{
    public class PlayerWorldBuilding : MonoBehaviour
    {
        [SerializeField] private float _maxInteractionDistance;
        [SerializeField] private LayerMask _buildModeLayerMask;
        [SerializeField] private LayerMask _deleteModeLayerMask;
        [SerializeField] private int _defaultLayerInt;
        
        public float MaxInteractionDistance => _maxInteractionDistance;
        public LayerMask BuildModeLayerMask => _buildModeLayerMask;
        public LayerMask DeleteModeLayerMask => _deleteModeLayerMask;

        public GameObject selectedSegment;
        public GameObject previewGameObject;

        private Camera _camera;

        private void Awake()
        {
            _camera = Camera.main;
            selectedSegment = null;
        }

        private void Update()
        {
            if (GameFlags.HAMMER_EQUIPPED && GetMouseRayHit(_buildModeLayerMask, out RaycastHit raycastHit, _maxInteractionDistance) && previewGameObject != null) {
                previewGameObject.transform.position = raycastHit.point;
            }
        }

        public void TryPlaceSegment(Vector3 position)
        {
            SegmentPreview previewSegment = previewGameObject.GetComponent<SegmentPreview>();
            if (previewSegment.canBePlaced) {
                GameObject segmentObj = Instantiate(selectedSegment, position, Quaternion.identity);
                segmentObj.GetComponent<SegmentPreview>().ResetMaterial();
                Destroy(segmentObj.GetComponent<SegmentPreview>());
            }
        }
        
        private bool GetMouseRayHit(LayerMask layerMask, out RaycastHit raycastHit, float distance)
        {
            Ray ray = _camera.ViewportPointToRay(new Vector3(UnityEngine.Input.mousePosition.x / Screen.width, UnityEngine.Input.mousePosition.y / Screen.height, 0));
            return Physics.Raycast(ray, out raycastHit, distance, layerMask);
        }
    }
}