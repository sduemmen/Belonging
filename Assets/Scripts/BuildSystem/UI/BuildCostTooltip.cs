using System.Collections.Generic;
using InventorySystem;
using InventorySystem.UI;
using TMPro;
using UI.MouseHover;
using UnityEngine;

namespace BuildSystem.UI
{
    public class BuildCostTooltip : Tooltip
    {
        [SerializeField] private TextMeshProUGUI _segmentNameLabel;
        [SerializeField] private UIInventorySlot _slotPrefab;
        [SerializeField] private GameObject _uiCostHolder;
        [SerializeField] private Color _costAffordableColor = Color.white;
        [SerializeField] private Color _costUnaffordableColor = Color.red;
        private string _segmentName;
        private List<ItemStack> _buildCosts;

        public void Initialize(string displayName, List<ItemStack> buildCosts)
        {
            _segmentName = displayName;
            _buildCosts = buildCosts;
            
            _segmentNameLabel.text = displayName;
            
            RefreshUI();

            InventoryController.Instance.PlayerInventory.OnSlotChangedDelegate += RefreshUI;
            BuildingController.OnSegmentUnlockedDelegate += RefreshUI;
        }

        private void OnDestroy()
        {
            if (InventoryController.Instance != null)
            {
                InventoryController.Instance.PlayerInventory.OnSlotChangedDelegate -= RefreshUI;
            }
            
            if (BuildingController.OnSegmentUnlockedDelegate != null)
            {
                BuildingController.OnSegmentUnlockedDelegate -= RefreshUI;
            }
        }

        private void RefreshUI(object o)
        {
            RefreshUI();
        }

        private void RefreshUI()
        {
            _segmentNameLabel.fontStyle = BuildingController.Instance.IsSegmentUnlocked(_segmentName)
                ? FontStyles.Normal
                : FontStyles.Strikethrough;

            foreach (ItemStack buildCost in _buildCosts)
            {
                UIInventorySlot slot = Instantiate(_slotPrefab, _uiCostHolder.transform);
                slot.Initialize(new InventorySlot(buildCost.Item, buildCost.Amount));

                bool costIsAffordable = InventoryController.Instance.PlayerInventory.Contains(buildCost.Item, buildCost.Amount);
                slot.StackSizeLabel.color = costIsAffordable ? _costAffordableColor : _costUnaffordableColor;
            }
        }
    }
}