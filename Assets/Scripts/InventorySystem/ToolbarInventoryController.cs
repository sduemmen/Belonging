using InventorySystem.UI;
using UnityEngine;
using UnityEngine.UI;

namespace InventorySystem
{
    public class ToolbarInventoryController : MonoBehaviour
    {
        private static ToolbarInventoryController _instance;
        public static ToolbarInventoryController Instance {
            get {
                if (_instance == null)
                {
                    _instance = (ToolbarInventoryController)FindObjectOfType(typeof(ToolbarInventoryController));
                }

                return _instance;
            }
        }
        
        [SerializeField] private Inventory _toolbarInventory;
        [SerializeField] private Image _slotHighlightImage;
        [SerializeField] private UIInventorySlot _uiInventorySlotPrefab;
        [SerializeField] private Transform _uiToolbarInventoryDisplayTarget;

        public Inventory ToolbarInventory => _toolbarInventory;

        private void Awake()
        {
            foreach (InventorySlot inventorySlot in _toolbarInventory.InventorySlots)
            {
                UIInventorySlot uiInventorySlot = Instantiate(_uiInventorySlotPrefab, _uiToolbarInventoryDisplayTarget, false);
                uiInventorySlot.Initialize(inventorySlot);
                uiInventorySlot.StackSizeLabel.text = "";
            }
            
            DisableHighlight();
        }

        public void EnableHighlightAtIndex(int index)
        {
            _slotHighlightImage.gameObject.SetActive(true);
            _slotHighlightImage.transform.position = _uiToolbarInventoryDisplayTarget.GetChild(index).position;
        }

        public void DisableHighlight()
        {
            _slotHighlightImage.gameObject.SetActive(false);
        }
    }
}