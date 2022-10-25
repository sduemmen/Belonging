using Environment;
using Flags;
using InventorySystem;
using UnityEngine;
using Utility;

namespace BuildSystem
{
    public class PlayerWorldBuilding : MonoBehaviour
    {
        [SerializeField] private float _maxBuildingDistance;
        [SerializeField] private LayerMask _buildModeLayerMask;
        [SerializeField] private World _world;
        [SerializeField] private Inventory playerInventory;

        public GameObject selectedSegment;
        public GameObject previewGameObject;
        private Vector3 currentSnappingPoint = Vector3.positiveInfinity;
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
            bool costIsAffordable = true;

            foreach (BuildCost buildCost in segment.itemDrops) {
                if (!playerInventory.Contains(buildCost.item, buildCost.amount)) {
                    costIsAffordable = false;
                    break;
                }
            }
            
            if ((preview.canBePlaced && mouseOutOfRange) || !costIsAffordable) {
                preview.canBePlaced = false;
                preview.UpdateMaterial();
                previewOutOfRange = true;
                if (preview.canBePlaced && mouseOutOfRange) return;
            } 
            if (!preview.canBePlaced && !mouseOutOfRange && previewOutOfRange && costIsAffordable) {
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
                bool snapTypeEqualToSegmentType = raycastHit.transform.gameObject.layer.Equals(previewSegmentLayer);
                bool snappingPointChanged = segment.isSnapped && currentSnappingPoint != raycastHit.collider.bounds.center && snapTypeEqualToSegmentType;
                
                if ((!segment.isSnapped || snappingPointChanged) && snapTypeEqualToSegmentType) {
                    segment.isSnapped = true;
                    currentSnappingPoint = raycastHit.collider.bounds.center;
                    previewGameObject.transform.position = raycastHit.transform.position;
                } else if (!snapTypeEqualToSegmentType) {
                    segment.isSnapped = false;
                    previewGameObject.transform.position = raycastHit.point;
                }
            }
        }

        public void TryPlaceSegment()
        {
            SegmentPreview previewSegment = previewGameObject.GetComponent<SegmentPreview>();
            
            if (previewSegment.canBePlaced) {
                Vector3 position = previewGameObject.transform.position;
                Quaternion rotation = previewGameObject.transform.rotation;
                GameObject segmentObj = Instantiate(selectedSegment, position, rotation);
                segmentObj.GetComponent<SegmentPreview>().ResetMaterial();
                Destroy(segmentObj.GetComponent<SegmentPreview>());
                _world.placedSegments += 1;
                Destroyable segment = segmentObj.GetComponent<Destroyable>();
                segment.world = _world;
                
                foreach (BuildCost buildCost in segment.itemDrops) {
                    if (playerInventory.Contains(buildCost.item, buildCost.amount)) {
                        playerInventory.RemoveItem(buildCost.item, buildCost.amount);
                    }
                }
            }
        }
        
        private bool GetMouseRayHit(LayerMask layerMask, out RaycastHit raycastHit, float distance)
        {
            Ray ray = _camera.ViewportPointToRay(new Vector3(UnityEngine.Input.mousePosition.x / Screen.width, UnityEngine.Input.mousePosition.y / Screen.height, 0));
            return Physics.Raycast(ray, out raycastHit, distance, layerMask);
        }
    }
}