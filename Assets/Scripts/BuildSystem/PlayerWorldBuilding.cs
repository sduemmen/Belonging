using Environment;
using Flags;
using UnityEngine;

namespace BuildSystem
{
    public class PlayerWorldBuilding : MonoBehaviour
    {
        [SerializeField] private float _maxBuildingDistance;
        [SerializeField] private float _cancelSnappingDistance;
        [SerializeField] private LayerMask _buildModeLayerMask;
        [SerializeField] private LayerMask _deleteModeLayerMask;
        [SerializeField] private World _world;
        [SerializeField] private int _defaultLayerInt;
        
        public float MaxBuildingDistance => _maxBuildingDistance;
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
            if (GameFlags.HAMMER_EQUIPPED && GetMouseRayHit(_buildModeLayerMask, out RaycastHit raycastHit, 40) && previewGameObject != null) {
                if ((transform.position - raycastHit.point).magnitude > _maxBuildingDistance) return;
                
                if (!previewGameObject.GetComponent<Destroyable>().isSnapped) {
                    previewGameObject.transform.position = raycastHit.point;
                } else {
                    if ((previewGameObject.transform.position - raycastHit.point).magnitude > _cancelSnappingDistance) {
                        previewGameObject.GetComponent<Destroyable>().isSnapped = false;
                        previewGameObject.transform.position = raycastHit.point;
                    }
                }
            }
        }

        public void TryPlaceSegment(Vector3 position)
        {
            SegmentPreview previewSegment = previewGameObject.GetComponent<SegmentPreview>();
            if (previewSegment.canBePlaced) {
                Vector3 actualPosition = previewGameObject.GetComponent<Destroyable>().isSnapped ? previewGameObject.transform.position : position;
                GameObject segmentObj = Instantiate(selectedSegment, actualPosition, Quaternion.identity);
                segmentObj.GetComponent<SegmentPreview>().ResetMaterial();
                Destroy(segmentObj.GetComponent<SegmentPreview>());
                _world.placedSegments += 1;
                segmentObj.GetComponent<Destroyable>().world = _world;
            }
        }
        
        private bool GetMouseRayHit(LayerMask layerMask, out RaycastHit raycastHit, float distance)
        {
            Ray ray = _camera.ViewportPointToRay(new Vector3(UnityEngine.Input.mousePosition.x / Screen.width, UnityEngine.Input.mousePosition.y / Screen.height, 0));
            return Physics.Raycast(ray, out raycastHit, distance, layerMask);
        }
    }
}