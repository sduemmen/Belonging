using System;
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
        [SerializeField] private World _world;
        
        public float MaxBuildingDistance => _maxBuildingDistance;
        public LayerMask BuildModeLayerMask => _buildModeLayerMask;

        public GameObject selectedSegment;
        public GameObject previewGameObject;
        private Vector3 currentSnappingPoint;

        private Camera _camera;

        private void Awake()
        {
            _camera = Camera.main;
            selectedSegment = null;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(currentSnappingPoint, 1f);
        }

        private void Update()
        {
            if (!GameFlags.HAMMER_EQUIPPED || previewGameObject == null) return;

            Destroyable previewSegment = previewGameObject.GetComponent<Destroyable>();
            
            int previewSegmentLayer = LayerMask.NameToLayer(previewGameObject.tag);
            int layerMask = 0;
            layerMask |= _buildModeLayerMask;
            layerMask |= 1 << previewSegmentLayer;  // we can neglect all layers except the current segment layer/type
            
            if (GetMouseRayHit(layerMask, out RaycastHit raycastHit, 40)) {
                bool mouseHitOutOfRange = (transform.position - raycastHit.point).magnitude > _maxBuildingDistance;
                
                if (mouseHitOutOfRange) return;
                
                bool cancelSnapping = (currentSnappingPoint - raycastHit.point).magnitude > _cancelSnappingDistance;
                bool colliderLayerEqualToPreviewSegment = raycastHit.transform.gameObject.layer.Equals(previewSegmentLayer);
                bool snappingPointChanged = previewSegment.isSnapped && currentSnappingPoint != raycastHit.collider.bounds.center && colliderLayerEqualToPreviewSegment;
                
                if ((!previewSegment.isSnapped || snappingPointChanged) && colliderLayerEqualToPreviewSegment) {
                    previewSegment.isSnapped = true;
                    currentSnappingPoint = raycastHit.collider.bounds.center;
                    previewGameObject.transform.position = raycastHit.transform.position;
                } else if (cancelSnapping || !colliderLayerEqualToPreviewSegment) {
                    previewSegment.isSnapped = false;
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