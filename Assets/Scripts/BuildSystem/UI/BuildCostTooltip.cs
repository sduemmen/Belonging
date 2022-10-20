using System.Collections.Generic;
using InventorySystem;
using InventorySystem.UI;
using TMPro;
using UnityEngine;

namespace BuildSystem.UI
{
    public class BuildCostTooltip : MonoBehaviour
    {
        public TextMeshProUGUI segmentNameLabel;
        public UIInventorySlot slotPrefab;
        public GameObject uiCostHolder;
        
        public void Initialize(string displayName, List<BuildCost> buildCosts)
        {
            segmentNameLabel.text = displayName;
            
            foreach (BuildCost buildCost in buildCosts) {
                UIInventorySlot slot = Instantiate(slotPrefab, uiCostHolder.transform);
                slot.Initialize(new InventorySlot(buildCost.item, buildCost.amount));
            }
        }
    }
}