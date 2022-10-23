using Environment;
using Flags;
using UnityEngine;
using Utility;

namespace BuildSystem
{
    public class PlayerWorldBuilding : MonoBehaviour
    {
        [SerializeField] private float _maxBuildingDistance;
        [SerializeField] private float _cancelSnappingDistance;
        [SerializeField] private LayerMask _buildModeLayerMask;
        [SerializeField] private World _world;
        
        public float MaxBuildingDistance => _maxBuildingDistance;
        public LayerMask BuildModeLayerMask => _buildModeLayerMask;

        public GameObject selectedSegment;
        public GameObject previewGameObject;
        private Vector3 currentSnappingPoint;
        private bool previewOutOfRange;

        private Camera _camera;

        private void Awake()
        {
            _camera = Camera.main;
            selectedSegment = null;
        }

        private void OnDrawGizmos()
        {
            if (DebugInformation.Instance == null || !DebugInformation.Instance.drawCurrentSnapPoint) return;
            
            DebugExtension.DebugWireSphere(currentSnappingPoint, Color.red, .1f, depthTest:false);
        }

        private void Update()
        {
            if (!GameFlags.HAMMER_EQUIPPED || previewGameObject == null) return;
            
            Destroyable segment = previewGameObject.GetComponent<Destroyable>();
            SegmentPreview preview = previewGameObject.GetComponent<SegmentPreview>();
            
            bool mouseOutOfRange = (transform.position - segment.transform.position).magnitude > _maxBuildingDistance;

            if (preview.canBePlaced && mouseOutOfRange) {
                preview.canBePlaced = false;
                preview.UpdateMaterial();
                previewOutOfRange = true;
                return;
            } 
            if (!preview.canBePlaced && !mouseOutOfRange && previewOutOfRange) {
                preview.canBePlaced = true;
                preview.UpdateMaterial();
                previewOutOfRange = false;
                return;
            }
            
            int previewSegmentLayer = LayerMask.NameToLayer(previewGameObject.tag);
            int layerMask = 0;
            layerMask |= _buildModeLayerMask;
            layerMask |= 1 << previewSegmentLayer;  // we can neglect all layers except the current segment layer/type
            
            if (GetMouseRayHit(layerMask, out RaycastHit raycastHit, 60)) {
                bool colliderLayerEqualToPreviewSegment = raycastHit.transform.gameObject.layer.Equals(previewSegmentLayer);
                bool snappingPointChanged = segment.isSnapped && currentSnappingPoint != raycastHit.collider.bounds.center && colliderLayerEqualToPreviewSegment;
                bool cancelSnapping = (currentSnappingPoint - raycastHit.point).magnitude > _cancelSnappingDistance;

                if ((!segment.isSnapped || snappingPointChanged) && colliderLayerEqualToPreviewSegment) {
                    segment.isSnapped = true;
                    currentSnappingPoint = raycastHit.collider.bounds.center;
                    previewGameObject.transform.position = raycastHit.transform.position;
                } else if (cancelSnapping || !colliderLayerEqualToPreviewSegment) {
                    segment.isSnapped = false;
                    previewGameObject.transform.position = raycastHit.point;
                }
            }
        }

        public void TryPlaceSegment(Vector3 position)
        {
            SegmentPreview previewSegment = previewGameObject.GetComponent<SegmentPreview>();
            if (previewSegment.canBePlaced) {
                Vector3 actualPosition = previewGameObject.GetComponent<Destroyable>().isSnapped ? previewGameObject.transform.position : position;
                GameObject segmentObj = Instantiate(selectedSegment, actualPosition, previewGameObject.transform.rotation);
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