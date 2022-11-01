using System.Collections.Generic;
using BuildSystem.UI;
using Collections;
using Flags;
using InventorySystem;
using SaveSystem.Data;
using Sirenix.OdinInspector;
using UI;
using UnityEngine;
using UnityEngine.Events;
using Utility;
using World;

namespace BuildSystem
{
    public class BuildingController : Controller, IDisplayContext
    {
        private static BuildingController _instance;
        public static BuildingController Instance {
            get {
                if (_instance == null)
                {
                    _instance = (BuildingController)FindObjectOfType(typeof(BuildingController));
                }
                return _instance;
            }
        }

        [SerializeField, TitleGroup("General")] private List<SegmentCollection> _segmentCollections;
        [SerializeField, TitleGroup("General")] private List<SegmentUnlockData> _segmentUnlockData;
        [Button("Setup Unlock Data"), TitleGroup("General")]
        private void SetupUnlockData()
        {
            _segmentUnlockData = new List<SegmentUnlockData>();
            
            foreach (SegmentCollection segmentCollection in _segmentCollections)
            {
                foreach (Segment segment in segmentCollection.Segments)
                {
                    _segmentUnlockData.Add(new SegmentUnlockData(segment.Name, false));
                }
            }
        }
        [Button("Setup item drops in prefabs"), TitleGroup("General")]
        private void SetupItemDrops()
        {
            foreach (SegmentCollection segmentCollection in _segmentCollections)
            {
                segmentCollection.SetupItemDrops();
            }
        }
        
        [SerializeField, TitleGroup("UI")] private GameObject _uiBuildMenuDisplayContext;
        [SerializeField, TitleGroup("UI")] private Transform _uiBuildMenuTarget;
        [SerializeField, TitleGroup("UI")] private GameObject _uiCollectionRowPrefab;
        [SerializeField, TitleGroup("UI")] private UISegmentSlot _uiSegmentSlotPrefab;
        private List<UISegmentSlot> _uiSegmentSlots;
        private bool _displayContextActive;

        [SerializeField, TitleGroup("Internal")] private float _maxBuildingDistance;
        [SerializeField, TitleGroup("Internal")] private LayerMask _buildModeLayerMask;
        private GameObject _buildingPreviewObject;
        private Camera _camera;
        private Vector3 _currentSnappingPoint = Vector3.positiveInfinity;
        private bool _previewOutOfRange;
        private GameObject _selectedSegment;
        
        public static UnityAction<UISegmentSlot> OnSegmentSlotClickedDelegate;
        public static UnityAction<string> OnSegmentUnlockedDelegate;

        public bool BuildingPreviewEnabled => _buildingPreviewObject != null;
        public bool DisplayContextActive => _displayContextActive;

        [Button("Load Manually"), TitleGroup("Debugging")]
        protected override void OnLoadCompleted()
        {
            _uiSegmentSlots = new List<UISegmentSlot>();
            
            foreach (SegmentCollection segmentCollection in _segmentCollections)
            {
                GameObject row = Instantiate(_uiCollectionRowPrefab, _uiBuildMenuTarget, false);
                UICollectionRow uiCollectionRow = row.GetComponent<UICollectionRow>();
                uiCollectionRow.collectionTitleLabel.text = " " + segmentCollection.CollectionTitle;

                for (int i = 0; i < segmentCollection.Count; i++)
                {
                    UISegmentSlot slot = Instantiate(_uiSegmentSlotPrefab, uiCollectionRow.content.transform, false);
                    slot.Initialize(segmentCollection.Segments[i]);
                    _uiSegmentSlots.Add(slot);
                }
            }
            
            OnSegmentSlotClickedDelegate += OnSegmentSlotClicked;
            OnSegmentUnlockedDelegate += OnSegmentUnlocked;

            _camera = Camera.main;
            _selectedSegment = null;
        }

        private void Update()
        {
            if (!GameFlags.HAMMER_EQUIPPED || _buildingPreviewObject == null) return;

            Destroyable segment = _buildingPreviewObject.GetComponent<Destroyable>();
            SegmentPreview preview = _buildingPreviewObject.GetComponent<SegmentPreview>();

            bool mouseOutOfRange = (transform.position - segment.transform.position).magnitude > _maxBuildingDistance;
            bool costIsAffordable = true;

            foreach (ItemStack buildCost in segment.itemDrops)
                if (!InventoryController.Instance.PlayerInventory.Contains(buildCost.Item, buildCost.Amount))
                {
                    costIsAffordable = false;
                    break;
                }

            if ((preview.canBePlaced && mouseOutOfRange) || !costIsAffordable)
            {
                preview.canBePlaced = false;
                preview.UpdateMaterial();
                _previewOutOfRange = true;
                if (preview.canBePlaced && mouseOutOfRange) return;
            }

            if (!preview.canBePlaced && !mouseOutOfRange && _previewOutOfRange && costIsAffordable)
            {
                preview.canBePlaced = true;
                preview.UpdateMaterial();
                _previewOutOfRange = false;
                return;
            }

            int previewSegmentLayer = LayerMask.NameToLayer(_buildingPreviewObject.tag);
            int layerMask = 0;
            layerMask |= _buildModeLayerMask;
            layerMask |= 1 << previewSegmentLayer; // we can neglect all layers except the current segment layer/type

            if (Raycast.GetMouseRayHit(_camera, layerMask, out RaycastHit raycastHit, 60))
            {
                bool snapTypeEqualToSegmentType = raycastHit.transform.gameObject.layer.Equals(previewSegmentLayer);
                bool snappingPointChanged = segment.isSnapped && _currentSnappingPoint != raycastHit.collider.bounds.center && snapTypeEqualToSegmentType;

                if ((!segment.isSnapped || snappingPointChanged) && snapTypeEqualToSegmentType)
                {
                    segment.isSnapped = true;
                    _currentSnappingPoint = raycastHit.collider.bounds.center;
                    _buildingPreviewObject.transform.position = raycastHit.transform.position;
                }
                else if (!snapTypeEqualToSegmentType)
                {
                    segment.isSnapped = false;
                    _buildingPreviewObject.transform.position = raycastHit.point;
                }
            }
        }

        private void OnDrawGizmos()
        {
            if (DebugInformation.Instance == null || !DebugInformation.Instance.drawCurrentSnapPoint) return;

            DebugExtension.DebugWireSphere(_currentSnappingPoint, Color.red, .1f, depthTest: false);
        }

        public void TryPlaceSegment()
        {
            SegmentPreview previewSegment = _buildingPreviewObject.GetComponent<SegmentPreview>();

            if (previewSegment.canBePlaced)
            {
                Vector3 position = _buildingPreviewObject.transform.position;
                Quaternion rotation = _buildingPreviewObject.transform.rotation;
                GameObject segmentObj = Instantiate(_selectedSegment, position, rotation);
                segmentObj.GetComponent<SegmentPreview>().ResetMaterial();
                Destroy(segmentObj.GetComponent<SegmentPreview>());
                World.World.Instance.placedSegments += 1;
                Destroyable segment = segmentObj.GetComponent<Destroyable>();

                foreach (ItemStack buildCost in segment.itemDrops)
                {
                    if (InventoryController.Instance.PlayerInventory.Contains(buildCost.Item, buildCost.Amount))
                    {
                        InventoryController.Instance.PlayerInventory.RemoveItem(buildCost.Item, buildCost.Amount);
                    }
                }
            }
        }

        private void OnSegmentSlotClicked(UISegmentSlot clickedSlot)
        {
            if (_buildingPreviewObject != null)
            {
                Destroy(_buildingPreviewObject);
            }

            GameObject newSelectedSegment = Resources.Load<GameObject>($"Prefabs/Models/World/{clickedSlot.SegmentName}");
            _selectedSegment = newSelectedSegment;
            _buildingPreviewObject = Instantiate(newSelectedSegment);
            _buildingPreviewObject.GetComponent<Destroyable>().isPlaced = false;

            HideDisplayContext();
        }

        private void OnSegmentUnlocked(string segmentName)
        {
            foreach (SegmentUnlockData unlockData in _segmentUnlockData)
            {
                if (segmentName == unlockData.segmentName)
                {
                    unlockData.unlocked = true;
                    _uiSegmentSlots.Find(slot => slot.SegmentName == segmentName).OnSegmentUnlocked();
                    Debug.Log("Unlocked " + segmentName);
                }
            }  
        }

        public bool IsSegmentUnlocked(string segmentName)
        {
            return _segmentUnlockData.Find(data => data.segmentName == segmentName).unlocked;
        }

        public void DisableBuildingPreview()
        {
            Destroy(_buildingPreviewObject);
            _buildingPreviewObject = null;
        }
        
        public void ShowDisplayContext()
        {
            _displayContextActive = true;
            _uiBuildMenuDisplayContext.SetActive(true);
        }

        public void HideDisplayContext()
        {
            _displayContextActive = false;
            _uiBuildMenuDisplayContext.SetActive(false);
            MouseTooltip.Instance.Hide();
        }
        
        public override void LoadData(GameData data)
        {
            if (data.segmentUnlockData.Count <= 0) return;

            _segmentUnlockData = data.segmentUnlockData;
            
            base.LoadData(data);
        }

        public override void SaveData(ref GameData data)
        {
            data.segmentUnlockData = _segmentUnlockData;
        }
    }
}