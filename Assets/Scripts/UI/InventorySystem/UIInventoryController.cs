using InventorySystem;
using UnityEngine;

namespace UI.InventorySystem
{
    public class UIInventoryController : MonoBehaviour
    {
        public DynamicInventoryDisplay inventoryPanel;

        private void Awake()
        {
            InventoryHolder.OnDynamicInventoryDisplayContextRequested += DisplayInventory;
            InventoryHolder.OnDynamicInventoryDisplayContextClosed += HideInventory;
            HideInventory();
        }

        private void OnDestroy()
        {
            InventoryHolder.OnDynamicInventoryDisplayContextRequested -= DisplayInventory;
            InventoryHolder.OnDynamicInventoryDisplayContextClosed -= HideInventory;
        }

        private void DisplayInventory(Inventory inventory)
        {
            inventoryPanel.gameObject.SetActive(true);
            inventoryPanel.Inventory = inventory;
        }

        private void HideInventory()
        {
            inventoryPanel.gameObject.SetActive(false);
        }
    }
}
