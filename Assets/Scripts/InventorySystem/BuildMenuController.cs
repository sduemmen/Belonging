using System.Collections.Generic;
using Events.Events;
using Flags;
using InventorySystem.Collections;
using InventorySystem.UI;
using Player;
using Player.Input;
using UnityEngine;

namespace InventorySystem
{
    public class BuildMenuController : InventoryController
    {
        [SerializeField] private List<Collection<SegmentCollectionEntry>> _segmentCollections;
        [SerializeField] private PlayerWorldBuilding _playerWorldBuilding;
        [SerializeField] private SimpleEvent closeBuildMenuEvent;

        protected override void Start()
        {
            if (_inventoryDisplay == null) Debug.LogError("No inventoryDisplay set in InventoryController");

            BuildMenuDisplay buildMenuDisplay = (BuildMenuDisplay)_inventoryDisplay;
            
            foreach (Collection<SegmentCollectionEntry> segmentCollection in _segmentCollections) {
                
                GameObject row = Instantiate(buildMenuDisplay.UICollectionRowPrefab, buildMenuDisplay.transform, false);
                UICollectionRow uiCollectionRow = row.GetComponent<UICollectionRow>();
                uiCollectionRow.collectionTitleLabel.text = " " + segmentCollection.collectionTitle;
                
                for (int i = 0; i < segmentCollection.entries.Count; i++) {
                    UIBuildMenuSlot uiSlot = buildMenuDisplay.AddSlot();
                    uiSlot.Initialize(segmentCollection.entries[i]);
                    uiSlot.parentDisplay = buildMenuDisplay;
                    uiSlot.transform.SetParent(uiCollectionRow.content.transform);
                }
            }

            _inventoryDisplay.OnSlotClicked += InteractWithSlot;
        }
        
        protected override void OnDestroy()
        {
            _inventoryDisplay.OnSlotClicked -= InteractWithSlot;
        }

        protected override void InteractWithSlot(UIInventorySlot clickedUISlot)
        {
            if (_playerWorldBuilding.previewGameObject != null) Destroy(_playerWorldBuilding.previewGameObject);

            UIBuildMenuSlot uiBuildMenuSlot = (UIBuildMenuSlot)clickedUISlot;
            GameObject newSelectedSegment = Resources.Load<GameObject>($"Prefabs/Models/World/{uiBuildMenuSlot.prefabName}");
            _playerWorldBuilding.selectedSegment = newSelectedSegment;
            _playerWorldBuilding.previewGameObject = Instantiate(newSelectedSegment);
            if (GameFlags.BUILD_MENU_OPEN) {
                GameFlags.BUILD_MENU_OPEN = false;
                EventManager.SetCursorState(true, CursorLockMode.None);
                closeBuildMenuEvent.Raise();
            }
        }
    }
}