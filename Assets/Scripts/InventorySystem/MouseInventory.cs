using Flags;
using InventorySystem.Items;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Utility;
using World;

namespace InventorySystem
{
    public class MouseInventory : MonoBehaviour
    {
        private static MouseInventory _instance;

        public static MouseInventory Instance {
            get {
                if (_instance == null)
                {
                    _instance = (MouseInventory)FindObjectOfType(typeof(MouseInventory));
                }

                return _instance;
            }
        }

        [SerializeField] private Image _image;
        [SerializeField] private TextMeshProUGUI _stackSizeLabel;
        [SerializeField] private Transform player;

        [SerializeField] public InventorySlot assignedInventorySlot;

        private bool AssignedInventorySlotIsEmpty => assignedInventorySlot == null || assignedInventorySlot.IsEmpty();
        private bool MouseClickedOutsideInventory => UserInputFlags.LEFT_MOUSE_BUTTON_WAS_PRESSED && !Raycast.MouseOverUI();

        private void Awake()
        {
            SetAssignedInventorySlot(new InventorySlot());
        }

        private void Update()
        {
            // Update Position in UI
            if ((!AssignedInventorySlotIsEmpty && !GameFlags.SLOT_EQUIPPED) || GameFlags.SLOT_EQUIPPED)
            {
                Vector2 mousePosition = Mouse.current.position.ReadValue();
                transform.position = mousePosition;
            }

            // Drop Inventory contents when clicking outside of UI
            if (!AssignedInventorySlotIsEmpty && !GameFlags.SLOT_EQUIPPED && MouseClickedOutsideInventory) DropContents(4);
        }

        public void OnCloseInventory()
        {
            if (!AssignedInventorySlotIsEmpty)
            {
                DropContents(2);
            }
        }

        public void SetAssignedInventorySlot(InventorySlot inventorySlot)
        {
            assignedInventorySlot = inventorySlot;
            RefreshUI();
        }

        public void RefreshUI()
        {
            if (AssignedInventorySlotIsEmpty)
            {
                _image.sprite = null;
                _image.color = Color.clear;
                _stackSizeLabel.text = "";
            }
            else
            {
                _image.sprite = assignedInventorySlot.Item.Icon;
                _image.color = Color.white;
                _stackSizeLabel.text = assignedInventorySlot.StackSize > 1 ? assignedInventorySlot.StackSize.ToString() : "";
            }
        }

        private void DropContents(float pickupDelay)
        {
            for (int i = 0; i < assignedInventorySlot.StackSize; i++)
            {
                MaterialItemObject materialItem = (MaterialItemObject)assignedInventorySlot.Item;
                GameObject item = Instantiate(materialItem.Prefab, player.position + new Vector3(Random.Range(-.5f, .5f), Random.Range(.2f, .5f), Random.Range(-.5f, .5f)), Quaternion.identity);
                item.GetComponent<Pickupable>().Initialize(pickupDelay, true);
            }

            SetAssignedInventorySlot(new InventorySlot());
        }
    }
}