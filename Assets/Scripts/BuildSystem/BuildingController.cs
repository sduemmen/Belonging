using System.Collections.Generic;
using System.Linq;
using BuildSystem.UI;
using Collections;
using Flags;
using InventorySystem;
using InventorySystem.Items;
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
        private enum GhostPlacementStatus
        {
            Valid,
            Blocked,
            Invalid,
            TooExpensive,
        }

        private static BuildingController instance;
        public static BuildingController Instance {
            get {
                if (instance == null)
                {
                    instance = (BuildingController)FindObjectOfType(typeof(BuildingController));
                }
                return instance;
            }
        }

        [SerializeField, TitleGroup("General")] private List<SegmentCollection> _segmentCollections;
        [SerializeField, TitleGroup("General")] private List<SegmentUnlockData> _segmentUnlockData;
        [SerializeField, TitleGroup("UI")] private GameObject _uiBuildMenuDisplayContext;
        [SerializeField, TitleGroup("UI")] private Transform _uiBuildMenuTarget;
        [SerializeField, TitleGroup("UI")] private GameObject _uiCollectionRowPrefab;
        [SerializeField, TitleGroup("UI")] private UISegmentSlot _uiSegmentSlotPrefab;
        private List<UISegmentSlot> m_uiSegmentSlots;
        private bool m_displayContextActive;

        public bool m_inDeleteMode;
        private Material m_validPlacementMaterial;
        private Material m_invalidPlacementMaterial;
        private GameObject m_ghostSegment;
        private LayerMask m_placementMask;
        private GhostPlacementStatus m_ghostPlacementStatus;
        private int m_segmentRotationQuadrant;
        private List<Transform> m_snapPointsAroundGhost = new List<Transform>();
        private List<Transform> m_snapPointsInGhost = new List<Transform>();
        private List<Segment> m_segmentsAroundGhost = new List<Segment>();

        public bool m_noBuildCost;
        public bool m_unlockEverything;

        public static UnityAction<UISegmentSlot> OnSegmentSlotClickedDelegate;
        public static UnityAction<string> OnSegmentUnlockedDelegate;

        public bool DisplayContextActive => m_displayContextActive;
        
        
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
        
        [Button("Clear Unlock Data"), TitleGroup("General")]
        private void ClearUnlockData()
        {
            _segmentUnlockData = new List<SegmentUnlockData>();
        }
        
        [Button("Unlock All"), TitleGroup("General")]
        private void UnlockAll()
        {
            for (int i = 0; i < _segmentUnlockData.Count; i++)
            {
                _segmentUnlockData[i].unlocked = true;
            }
        }
        
        [Button("Lock All"), TitleGroup("General")]
        private void LockAll()
        {
            for (int i = 0; i < _segmentUnlockData.Count; i++)
            {
                _segmentUnlockData[i].unlocked = false;
            }
        }
#endif

        [Button("Load Manually"), TitleGroup("Debugging")]
        protected override void OnLoadCompleted()
        {
            m_uiSegmentSlots = new List<UISegmentSlot>();
            
            foreach (SegmentCollection segmentCollection in _segmentCollections)
            {
                GameObject row = Instantiate(_uiCollectionRowPrefab, _uiBuildMenuTarget, false);
                UICollectionRow uiCollectionRow = row.GetComponent<UICollectionRow>();
                uiCollectionRow.collectionTitleLabel.text = " " + segmentCollection.CollectionTitle;

                for (int i = 0; i < segmentCollection.Count; i++)
                {
                    UISegmentSlot slot = Instantiate(_uiSegmentSlotPrefab, uiCollectionRow.content.transform, false);
                    slot.Initialize(segmentCollection.Segments[i]);
                    m_uiSegmentSlots.Add(slot);
                }
            }
            
            OnSegmentSlotClickedDelegate += OnSegmentSlotClicked;

            m_ghostSegment = null;
            
            m_placementMask = LayerMask.GetMask("Default", "Destructible", "Segment", "Ground");
            
            m_ghostPlacementStatus = GhostPlacementStatus.Valid;
            m_segmentRotationQuadrant = 0;
        }

        private void Awake()
        {
            m_validPlacementMaterial = Resources.Load<Material>("Materials/Shaders/GreenFresnel");
            m_invalidPlacementMaterial = Resources.Load<Material>("Materials/Shaders/RedFresnel");
        }

        private void Update()
        {
            if (!GameFlags.HAMMER_EQUIPPED) return;

            Cursor.visible = !m_inDeleteMode;
            Cursor.lockState = CursorLockMode.None;
            
            UpdateGhostSegment();

            if (Input.GetKeyDown(KeyCode.O))
            {
                m_noBuildCost = !m_noBuildCost;
            }
        }
        
        private void UpdateGhostSegment()
        {
            if (m_ghostSegment == null)
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
                if (m_ghostSegment.TryGetComponent(out Segment ghostSegment))
                {
                    m_ghostSegment.SetActive(true);
                    m_ghostPlacementStatus = GhostPlacementStatus.Valid;
                    
                    if (ghostSegment.m_needsGroundContact && hit.transform.gameObject.layer != LayerMask.NameToLayer("Ground"))
                    {
                        m_ghostPlacementStatus = GhostPlacementStatus.Invalid;
                    }

                    if (ghostSegment.m_needsFloorContact && hit.normal.y < .9f)
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
                    
                    Vector3 extrudedNormal = point + normal * 20;
                    Quaternion rotation = Quaternion.Euler(0, 22.5f * m_segmentRotationQuadrant, 0);
                    m_ghostSegment.transform.position = extrudedNormal;
                    m_ghostSegment.transform.rotation = rotation;
                    List<Collider> ghostColliders = m_ghostSegment.GetComponentsInChildren<Collider>().ToList();
                    
                    Vector3 closestPointOnGhostToHitPoint = Vector3.zero;
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
                    
                    Vector3 closestPointOnGhostToGhostCenter = extrudedNormal - closestPointOnGhostToHitPoint;
                    
                    m_ghostSegment.transform.position = point + closestPointOnGhostToGhostCenter;
                    m_ghostSegment.transform.rotation = rotation;
                    // Debug.Log($"{ extrudedNormal } { closestPointOnGhostToHitPoint } { closestPointOnGhostToGhostCenter } { point + closestPointOnGhostToGhostCenter}");
                    
                    if (ghostSegment.m_enableSnapping && FindClosestSnapPoints(m_ghostSegment.transform, 0.5f, out Transform snapPointInGhost, out Transform closestSnapPoint))
                    {
                        Vector3 position = closestSnapPoint.position - (snapPointInGhost.position - m_ghostSegment.transform.position);
                        if (!CheckGhostOverlap(position))
                        {
                            m_ghostSegment.transform.position = position;
                        }
                    }

                    foreach (Collider ghostCollider in ghostColliders)
                    {
                        LayerMask playerLayerMask = LayerMask.GetMask("Player");
                        Collider[] results = new Collider[1];
                        int overlappingObjects = Physics.OverlapBoxNonAlloc(m_ghostSegment.transform.position, ghostCollider.bounds.extents, results, m_ghostSegment.transform.rotation, playerLayerMask);
                        if (overlappingObjects > 0)
                        {
                            m_ghostPlacementStatus = GhostPlacementStatus.Blocked;
                        }
                    }
                    
                    ghostSegment.GetComponent<Ghost>().SetMaterial(m_ghostPlacementStatus == GhostPlacementStatus.Valid ? m_validPlacementMaterial : m_invalidPlacementMaterial);
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
            
            Segment.GetSnapPointsInRadius(m_ghostSegment.transform.position, 5f, m_snapPointsAroundGhost, m_segmentsAroundGhost);
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
                
                bool identicalRotation = Quaternion.Angle(m_ghostSegment.transform.rotation, segment.transform.rotation) < 5;
                if (!identicalRotation)
                {
                    continue;
                }
                
                bool identicalName = segment.name.StartsWith(m_ghostSegment.name);
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
                    Vector3 objectPosition = m_ghostSegment.transform.position;
                    Quaternion objectRotation = m_ghostSegment.transform.rotation;

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

                    if (!m_noBuildCost)
                    {
                        foreach (ItemStack buildCost in segment.m_requirements)
                        {
                            if (InventoryController.Instance.PlayerInventory.Contains(buildCost.Item, buildCost.Amount))
                            {
                                InventoryController.Instance.PlayerInventory.RemoveItem(buildCost.Item, buildCost.Amount);
                            }
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
            PlaceSegment(m_ghostSegment.GetComponent<Segment>());
        }

        public void TryDeleteSegment()
        {
            if (Raycast.SegmentRayCast(LayerMask.GetMask("Segment"), 30f, out RaycastHit hit, out Segment segment))
            {
                Destructible destructible = segment.GetComponent<Destructible>();

                foreach (ItemStack itemDrop in destructible.m_itemDrops)
                {
                    for (int i = 0; i < itemDrop.Amount; i++)
                    {
                        MaterialItemObject materialItemObject = (MaterialItemObject)itemDrop.Item;
                        GameObject itemInstance = Instantiate(materialItemObject.Prefab, segment.transform.position + Random.insideUnitSphere + Vector3.up, Quaternion.identity);
                        itemInstance.GetComponent<Pickupable>().Initialize(0, true);
                    }
                }

                if (destructible.m_destructionFX)
                {
                    destructible.m_destructionFX.PlayFX(segment.transform.position);
                }
                
                Destroy(segment.gameObject);
            }
        }

        public void EnterDeleteMode()
        {
            m_inDeleteMode = true;
                
            ToolItemObject hammer = (ToolItemObject)ToolbarInventoryController.Instance.ToolbarInventory.GetSlotAtIndex(2).Item;
            MouseInventory.Instance.SetAssignedInventorySlot(new InventorySlot(hammer, 1));
            
            HideDisplayContext();
        }

        public void LeaveDeleteMode()
        {
            m_inDeleteMode = false;
                
            MouseInventory.Instance.SetAssignedInventorySlot(new InventorySlot(null, -1));
        }

        private void OnSegmentSlotClicked(UISegmentSlot clickedSlot)
        {
            if (m_ghostSegment != null)
            {
                Destroy(m_ghostSegment);
            }

            GameObject newSelectedSegment = Resources.Load<GameObject>($"Prefabs/Models/Segment/{clickedSlot.SegmentName}");
            m_ghostSegment = Instantiate(newSelectedSegment);
            Segment ghostSegment = m_ghostSegment.GetComponent<Segment>();

            if (ghostSegment)
            {
                if (ghostSegment.m_isLightSource)
                {
                    ghostSegment.GetComponentInChildren<Light>().enabled = false;
                }
            }
            else
            {
                Debug.Log($"No Segment script attached to GameObject {m_ghostSegment.name}");
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
                    m_uiSegmentSlots.Find(slot => slot.SegmentName == segmentName).OnSegmentUnlocked();
                    Debug.Log("Unlocked " + segmentName);
                }
            }
            
            OnSegmentUnlockedDelegate?.Invoke(segmentName);
        }

        public bool IsSegmentUnlocked(string segmentName)
        {
            Debug.Log(segmentName);
            return _segmentUnlockData.Find(data => data.segmentName.Equals(segmentName)).unlocked;
        }

        public void DestroyGhostSegment()
        {
            if (m_ghostSegment != null)
            {
                Destroy(m_ghostSegment);
                m_ghostSegment = null;
            }
        }
        
        public void ShowDisplayContext()
        {
            m_displayContextActive = true;
            _uiBuildMenuDisplayContext.SetActive(true);

            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            
            if (m_inDeleteMode)
            {
                LeaveDeleteMode();
            }
        }

        public void HideDisplayContext()
        {
            m_displayContextActive = false;
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