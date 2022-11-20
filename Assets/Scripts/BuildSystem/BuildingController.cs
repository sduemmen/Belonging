using System;
using System.Collections.Generic;
using System.Linq;
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
        public enum GhostPlacementStatus
        {
            Valid,
            Blocked,
            Invalid,
            TooExpensive,
        }

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
        [SerializeField, TitleGroup("UI")] private GameObject _uiBuildMenuDisplayContext;
        [SerializeField, TitleGroup("UI")] private Transform _uiBuildMenuTarget;
        [SerializeField, TitleGroup("UI")] private GameObject _uiCollectionRowPrefab;
        [SerializeField, TitleGroup("UI")] private UISegmentSlot _uiSegmentSlotPrefab;
        private List<UISegmentSlot> _uiSegmentSlots;
        private bool _displayContextActive;

        [SerializeField, TitleGroup("Internal")] private Transform _player;
        [SerializeField, TitleGroup("Internal")] private float _maxBuildingDistance;
        private Material _validPlacementMaterial;
        private Material _invalidPlacementMaterial;
        private GameObject _ghostSegment;
        private LayerMask m_placementMask;
        private GhostPlacementStatus m_ghostPlacementStatus;
        private int m_segmentRotationQuadrant;
        private List<Transform> m_snapPointsAroundGhost = new List<Transform>();
        private List<Transform> m_snapPointsInGhost = new List<Transform>();
        private List<Segment> m_segmentsAroundGhost = new List<Segment>();
        private Vector3 extrudedNormal;
        private Vector3 closestPointOnGhostToHitPoint;
        private Vector3 closestPointOnGhostToGhostCenter;
        private Vector3 finalPosition;

        public static UnityAction<UISegmentSlot> OnSegmentSlotClickedDelegate;
        public static UnityAction<string> OnSegmentUnlockedDelegate;

        public bool GhostSegmentVisible => _ghostSegment != null;
        public bool DisplayContextActive => _displayContextActive;
        
        
#if UNITY_EDITOR
        [Button("Setup Unlock Data"), TitleGroup("General")]
        private void SetupUnlockData()
        {
            foreach (SegmentCollection segmentCollection in _segmentCollections)
            {
                foreach (Segment segment in segmentCollection.Segments)
                {
                    if (!_segmentUnlockData.Exists(entry => entry.segmentName.Equals(segment.m_name)))
                    {
                        _segmentUnlockData.Add(new SegmentUnlockData(segment.m_name, false));
                    }
                }
            }
        }
#endif

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

            _ghostSegment = null;
            
            m_placementMask = LayerMask.GetMask("Default", "Destructible", "Segment", "Ground");
            
            m_ghostPlacementStatus = GhostPlacementStatus.Valid;
            m_segmentRotationQuadrant = 0;
        }

        private void OnDrawGizmos()
        {
            // Gizmos.color = Color.yellow;
            // Gizmos.DrawLine(Vector3.zero, extrudedNormal);
            // Gizmos.color = Color.blue;
            // Gizmos.DrawLine(Vector3.zero, closestPointOnGhostToHitPoint);
            // Gizmos.color = Color.red;
            // Gizmos.DrawLine(Vector3.zero, closestPointOnGhostToGhostCenter);
            // Gizmos.color = Color.magenta;
            // Gizmos.DrawLine(Vector3.zero, finalPosition);
        }

        private void Awake()
        {
            _validPlacementMaterial = Resources.Load<Material>("Materials/Shaders/GreenFresnel");
            _invalidPlacementMaterial = Resources.Load<Material>("Materials/Shaders/RedFresnel");
        }

        private void Update()
        {
            if (!GameFlags.HAMMER_EQUIPPED) return;

            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            UpdateGhostSegment();
        }
        
        private void UpdateGhostSegment()
        {
            if (_ghostSegment == null)
            {
                return;
            }

            if (Input.GetKeyDown(KeyCode.UpArrow) || Input.mouseScrollDelta.y > 0)
            {
                m_segmentRotationQuadrant++;

                if (m_segmentRotationQuadrant < 0)
                {
                    m_segmentRotationQuadrant = 16 - m_segmentRotationQuadrant;
                }

                m_segmentRotationQuadrant %= 16;
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.mouseScrollDelta.y < 0)
            {
                m_segmentRotationQuadrant--;

                if (m_segmentRotationQuadrant < 0)
                {
                    m_segmentRotationQuadrant = 16 + m_segmentRotationQuadrant;
                }

                m_segmentRotationQuadrant %= 16;
            }
            

            if (Raycast.SegmentRayCast(m_placementMask, 30f, out RaycastHit hit, out Vector3 point, out Vector3 normal, out Segment hitSegment))
            {
                if (_ghostSegment.TryGetComponent(out Segment ghostSegment))
                {
                    _ghostSegment.SetActive(true);
                    m_ghostPlacementStatus = GhostPlacementStatus.Valid;
                    
                    if (ghostSegment.m_needsGroundContact && hit.transform.gameObject.layer != LayerMask.NameToLayer("Ground"))
                    {
                        m_ghostPlacementStatus = GhostPlacementStatus.Invalid;
                    }
                    if (ghostSegment.m_needsCeilingContact && (normal.y > -0.5f || !hitSegment))
                    {
                        m_ghostPlacementStatus = GhostPlacementStatus.Invalid;
                    }
                    if (ghostSegment.m_needsWallContact && (Mathf.Abs(normal.y) > 0.05f || !hitSegment))
                    {
                        m_ghostPlacementStatus = GhostPlacementStatus.Invalid;
                    }
                    foreach (ItemStack buildCost in ghostSegment.m_requirements)
                    {
                        if (InventoryController.Instance.PlayerInventory.Contains(buildCost.Item, buildCost.Amount)) continue; 
                        
                        m_ghostPlacementStatus = GhostPlacementStatus.TooExpensive;
                    }
                    
                    extrudedNormal = point + normal * 20;
                    Quaternion rotation = Quaternion.Euler(0, 22.5f * m_segmentRotationQuadrant, 0);
                    _ghostSegment.transform.position = extrudedNormal;
                    _ghostSegment.transform.rotation = rotation;
                    List<Collider> ghostColliders = _ghostSegment.GetComponentsInChildren<Collider>().ToList();
                    
                    closestPointOnGhostToHitPoint = Vector3.zero;
                    float closestDistance = float.PositiveInfinity;

                    foreach (Collider ghostCollider in ghostColliders)
                    {
                        MeshCollider meshCollider = ghostCollider as MeshCollider;
                        if (!meshCollider || !meshCollider.convex)
                        {
                            continue;
                        }

                        if (!meshCollider.enabled)
                        {
                            meshCollider.enabled = true;
                        }

                        Vector3 p = ghostCollider.ClosestPoint(point);
                        float d = Vector3.Distance(p, point);
                        
                        if (d < closestDistance)
                        {
                            closestPointOnGhostToHitPoint = p;
                            closestDistance = d;
                        }

                        meshCollider.enabled = false;
                    }
                    
                    closestPointOnGhostToGhostCenter = extrudedNormal - closestPointOnGhostToHitPoint;
                    finalPosition = point + closestPointOnGhostToGhostCenter;
                    
                    _ghostSegment.transform.position = point + closestPointOnGhostToGhostCenter;
                    _ghostSegment.transform.rotation = rotation;
                    // Debug.Log($"{ extrudedNormal } { closestPointOnGhostToHitPoint } { closestPointOnGhostToGhostCenter } { point + closestPointOnGhostToGhostCenter}");
                    
                    if (ghostSegment.m_enableSnapping && FindClosestSnapPoints(_ghostSegment.transform, 0.5f, out Transform snapPointInGhost, out Transform closestSnapPoint))
                    {
                        Vector3 position = closestSnapPoint.position - (snapPointInGhost.position - _ghostSegment.transform.position);
                        if (!CheckGhostOverlap(position))
                        {
                            _ghostSegment.transform.position = position;
                        }
                    }

                    foreach (Collider ghostCollider in ghostColliders)
                    {
                        LayerMask playerLayerMask = LayerMask.GetMask("Player");
                        Collider[] results = new Collider[1];
                        int overlappingObjects = Physics.OverlapBoxNonAlloc(_ghostSegment.transform.position, ghostCollider.bounds.extents, results, _ghostSegment.transform.rotation, playerLayerMask);
                        if (overlappingObjects > 0)
                        {
                            m_ghostPlacementStatus = GhostPlacementStatus.Blocked;
                        }
                    }
                    
                    ghostSegment.GetComponent<Ghost>().SetMaterial(m_ghostPlacementStatus == GhostPlacementStatus.Valid ? _validPlacementMaterial : _invalidPlacementMaterial);
                }
            }
        }
        
        private bool FindClosestSnapPoints(Transform ghost, float radius, out Transform a, out Transform b)
        {
            a = null;
            b = null;
            
            m_snapPointsAroundGhost.Clear();
            m_snapPointsInGhost.Clear();
            m_segmentsAroundGhost.Clear();
            
            Segment.GetSnapPointsInRadius(_ghostSegment.transform.position, 5f, m_snapPointsAroundGhost, m_segmentsAroundGhost);
            ghost.GetComponent<Segment>().GetOwnSnapPoints(m_snapPointsInGhost);
            
            float closestDistance = float.PositiveInfinity;
            foreach (Transform snapPointInGhost in m_snapPointsInGhost)
            {
                if (GetClosestSnapPoint(snapPointInGhost.position, radius, m_snapPointsAroundGhost, out Transform closest, out float distance) && distance < closestDistance)
                {
                    closestDistance = distance;
                    a = snapPointInGhost;
                    b = closest;
                }
            }
            
            return a != null;
        }
        
        private bool GetClosestSnapPoint(Vector3 center, float radius, List<Transform> snapPoints, out Transform closest, out float closestDistance)
        {
            closest = null;
            closestDistance = float.PositiveInfinity;
            foreach (Transform snapPoint in snapPoints)
            {
                float distance = Vector3.Distance(snapPoint.position, center);
                if (distance <= radius && distance < closestDistance)
                {
                    closest = snapPoint;
                    closestDistance = distance;
                }
            }
            return closest != null;
        }

        public bool CheckGhostOverlap(Vector3 checkPosition)
        {
            foreach (Segment segment in m_segmentsAroundGhost)
            {
                bool identicalPosition = Vector3.Distance(checkPosition, segment.transform.position) < .01f;
                if (!identicalPosition)
                {
                    continue;
                }
                
                bool identicalRotation = Quaternion.Angle(_ghostSegment.transform.rotation, segment.transform.rotation) < 5;
                if (!identicalRotation)
                {
                    continue;
                }
                
                bool identicalName = segment.name.StartsWith(_ghostSegment.name);
                if (identicalName)
                {
                    return true;
                }
            }

            return false;
        }

        public bool PlaceSegment(Segment segment)
        {
            switch (m_ghostPlacementStatus)
            {
                case GhostPlacementStatus.Blocked:
                    HintDisplay.Instance.AddErrorHint("Area blocked");
                    return false;
                case GhostPlacementStatus.Invalid:
                    HintDisplay.Instance.AddErrorHint("Invalid Position");
                    return false;
                case GhostPlacementStatus.TooExpensive:
                    HintDisplay.Instance.AddErrorHint("Too expensive");
                    return false;
                default:
                    GameObject segmentObject = segment.gameObject;
                    Vector3 objectPosition = _ghostSegment.transform.position;
                    Quaternion objectRotation = _ghostSegment.transform.rotation;

                    GameObject instance = Instantiate(segmentObject, objectPosition, objectRotation);
                    Ghost ghost = instance.GetComponent<Ghost>();
                    ghost.ResetMaterial();

                    if (segment.m_isLightSource)
                    {
                        Light lightSource = instance.GetComponentInChildren<Light>();
                        lightSource.enabled = true;
                        FlickeringLight flickeringLight = instance.GetComponentInChildren<FlickeringLight>();
                        if (flickeringLight)
                        {
                            flickeringLight.m_originalPosition = flickeringLight.transform.position;
                            flickeringLight.m_enabled = true;
                        }
                    }
                    
                    foreach (ItemStack buildCost in segment.m_requirements)
                    {
                        if (InventoryController.Instance.PlayerInventory.Contains(buildCost.Item, buildCost.Amount))
                        {
                            InventoryController.Instance.PlayerInventory.RemoveItem(buildCost.Item, buildCost.Amount);
                        }
                    }
                    
                    World.World.Instance.placedSegments += 1;

                    if (segment.m_placementFX)
                    {
                        segment.m_placementFX.PlayFX(objectPosition);
                    }
                    
                    return true;
            }
        }
        
        public void TryPlaceSegment()
        {
            PlaceSegment(_ghostSegment.GetComponent<Segment>());
        }

        private void OnSegmentSlotClicked(UISegmentSlot clickedSlot)
        {
            if (_ghostSegment != null)
            {
                Destroy(_ghostSegment);
            }

            GameObject newSelectedSegment = Resources.Load<GameObject>($"Prefabs/Models/Segment/{clickedSlot.SegmentName}");
            _ghostSegment = Instantiate(newSelectedSegment);
            Segment ghostSegment = _ghostSegment.GetComponent<Segment>();

            if (ghostSegment)
            {
                if (ghostSegment.m_isLightSource)
                {
                    ghostSegment.GetComponentInChildren<Light>().enabled = false;
                }
            }
            else
            {
                Debug.Log($"No Segment script attached to GameObject {_ghostSegment.name}");
            }

            HideDisplayContext();
        }

        public void OnSegmentUnlocked(string segmentName)
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
            
            OnSegmentUnlockedDelegate?.Invoke(segmentName);
        }

        public bool IsSegmentUnlocked(string segmentName)
        {
            return _segmentUnlockData.Find(data => data.segmentName == segmentName).unlocked;
        }

        public void DestroyGhostSegment()
        {
            Destroy(_ghostSegment);
            _ghostSegment = null;
        }
        
        public void ShowDisplayContext()
        {
            _displayContextActive = true;
            _uiBuildMenuDisplayContext.SetActive(true);

            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        public void HideDisplayContext()
        {
            _displayContextActive = false;
            _uiBuildMenuDisplayContext.SetActive(false);
            MouseTooltip.Instance.Hide();
            
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
        
        public override void LoadData(GameData data)
        {
            if (data.segmentUnlockData.Count > 0)
            {
                _segmentUnlockData = data.segmentUnlockData;
            }
            
            base.LoadData(data);
        }

        public override void SaveData(ref GameData data)
        {
            data.segmentUnlockData = _segmentUnlockData;
        }
    }
}