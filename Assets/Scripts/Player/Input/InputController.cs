using BuildSystem;
using Events.Events;
using Flags;
using InventorySystem;
using UnityEngine;

namespace Player.Input
{
    public class InputController : MonoBehaviour
    {
        [SerializeField] private SimpleEvent movementInputEvent;
        [SerializeField] private SimpleEvent mouseMoveEvent;
        [SerializeField] private SimpleEvent mouseScrollEvent;

        [SerializeField] private SimpleEvent openQuestDisplayEvent;
        [SerializeField] private SimpleEvent closeQuestDisplayEvent;

        [SerializeField] private IntEvent equipSlotEvent;
        [SerializeField] private SimpleEvent toolUsedEvent;
        [SerializeField] private IntEvent rotateSegmentEvent;

        private PlayerControls _playerControls;

        public static Vector2 MovementInput { get; private set; }

        private void Awake()
        {
            _playerControls = new PlayerControls();

            // Mouse Move
            _playerControls.Camera.MouseDelta.performed += inputEvent => {
                bool middleMouseButtonHeldDown = _playerControls.Character.MiddleMouseButton.inProgress;

                if ((GameFlags.GAME_PAUSED || GameFlags.INVENTORY_OPEN || GameFlags.SLOT_EQUIPPED) && !middleMouseButtonHeldDown) return;

                mouseMoveEvent.Raise();
            };
            // Mouse Scroll
            _playerControls.Camera.MouseScrollDelta.performed += inputEvent => {
                if (GameFlags.GAME_PAUSED || GameFlags.BUILD_MENU_OPEN) return;

                mouseScrollEvent.Raise();
            };
            
            // Movement Input
            _playerControls.Character.Movement.performed += inputEvent => {
                if (GameFlags.GAME_PAUSED) return;

                MovementInput = inputEvent.ReadValue<Vector2>();
                movementInputEvent.Raise();
            };
            // Open/Close Inventory
            _playerControls.Character.InventoryDisplayContext.performed += inputEvent => {
                if (GameFlags.GAME_PAUSED) return;

                BuildingController.Instance.HideDisplayContext();
                HideBuildMenu();
                HideQuestDisplay();

                if (GameFlags.INVENTORY_CLOSED)
                    OpenInventory();
                else
                    HideInventory();
            };
            // Open/Close Quest Menu
            _playerControls.Character.QuestDisplayContext.performed += inputEvent => {
                if (GameFlags.GAME_PAUSED) return;

                HideInventory();
                HideBuildMenu();

                if (GameFlags.HAMMER_EQUIPPED) SetEquippedSlot(-1);

                if (GameFlags.QUEST_DISPLAY_CLOSED)
                    OpenQuestDisplay();
                else
                    HideQuestDisplay();
            };
            // Equip Slot 1
            _playerControls.Character.EquipSlot1.performed += inputEvent => {
                if (GameFlags.GAME_PAUSED) return;

                HideInventory();
                HideBuildMenu();

                if (GameFlags.AXE_EQUIPPED)
                    SetEquippedSlot(-1);
                else
                    SetEquippedSlot(0);
            };
            // Equip Slot 2
            _playerControls.Character.EquipSlot2.performed += inputEvent => {
                if (GameFlags.GAME_PAUSED) return;

                HideInventory();
                HideBuildMenu();

                SetCursorState(false, CursorLockMode.None);

                if (GameFlags.PICKAXE_EQUIPPED)
                    SetEquippedSlot(-1);
                else
                    SetEquippedSlot(1);
            };
            // Equip Slot 3
            _playerControls.Character.EquipSlot3.performed += inputEvent => {
                if (GameFlags.GAME_PAUSED) return;

                HideInventory();
                HideQuestDisplay();

                if (GameFlags.HAMMER_EQUIPPED && GameFlags.BUILD_MENU_CLOSED)
                {
                    OpenBuildMenu();
                    SetEquippedSlot(2);
                    return;
                }

                if (GameFlags.HAMMER_EQUIPPED)
                    SetEquippedSlot(-1);
                else
                    SetEquippedSlot(2);

                if (GameFlags.BUILD_MENU_CLOSED)
                    OpenBuildMenu();
                else
                    HideBuildMenu();
            };
            // rotate segment
            _playerControls.Character.RotateSegment.performed += inputEvent => {
                if (GameFlags.GAME_PAUSED || GameFlags.UI_ELEMENT_OPEN || !GameFlags.HAMMER_EQUIPPED) return;

                if (_playerControls.Character.Modifier1.inProgress)
                    rotateSegmentEvent.Raise(-90);
                else
                    rotateSegmentEvent.Raise(90);
            };
            // Action (left click)
            _playerControls.Character.Action.performed += inputEvent => {
                if (GameFlags.GAME_PAUSED || GameFlags.INVENTORY_OPEN || GameFlags.BUILD_MENU_OPEN || !GameFlags.SLOT_EQUIPPED) return;

                toolUsedEvent.Raise(); // use equipped tool
            };
            // Cancel Action (right click)
            _playerControls.Character.CancelAction.performed += inputEvent => {
                if (GameFlags.GAME_PAUSED) return;

                if (GameFlags.HAMMER_EQUIPPED && GameFlags.BUILD_MENU_CLOSED)
                {
                    SetEquippedSlot(2);
                    OpenBuildMenu();
                }
            };

            DisableCursor();
            InventoryController.Instance.HideDisplayContext();
            BuildingController.Instance.HideDisplayContext();
            closeQuestDisplayEvent.Raise();
        }

        private void OnEnable()
        {
            _playerControls?.Enable();
        }

        private void OnDisable()
        {
            _playerControls?.Disable();
        }

        private void OpenInventory()
        {
            if (GameFlags.INVENTORY_OPEN) return;

            SetEquippedSlot(-1);
            InventoryController.Instance.ShowDisplayContext();
            SetCursorState(true, CursorLockMode.None);
        }

        private void HideInventory()
        {
            if (GameFlags.INVENTORY_CLOSED) return;

            InventoryController.Instance.HideDisplayContext();
            SetCursorState(false, CursorLockMode.Locked);
        }

        private void OpenBuildMenu()
        {
            if (GameFlags.BUILD_MENU_OPEN) return;

            BuildingController.Instance.ShowDisplayContext();
            SetCursorState(true, CursorLockMode.None);
        }

        private void HideBuildMenu()
        {
            if (GameFlags.BUILD_MENU_CLOSED) return;

            BuildingController.Instance.HideDisplayContext();
            SetCursorState(false, CursorLockMode.Locked);
        }

        public void OpenQuestDisplay()
        {
            if (GameFlags.QUEST_DISPLAY_OPEN) return;

            GameFlags.QUEST_DISPLAY_OPEN = true;
            openQuestDisplayEvent.Raise();
        }

        public void HideQuestDisplay()
        {
            if (GameFlags.QUEST_DISPLAY_CLOSED) return;

            GameFlags.QUEST_DISPLAY_OPEN = false;
            closeQuestDisplayEvent.Raise();
        }

        private void SetEquippedSlot(int index)
        {
            switch (index)
            {
                case -1:
                    SetEquipSlotFlags(false, false, false);
                    SetCursorState(false, CursorLockMode.Locked);
                    break;
                case 0:
                    SetEquipSlotFlags(true, false, false);
                    SetCursorState(false, CursorLockMode.None);
                    break;
                case 1:
                    SetEquipSlotFlags(false, true, false);
                    SetCursorState(false, CursorLockMode.None);
                    break;
                case 2:
                    SetEquipSlotFlags(false, false, true);
                    SetCursorState(true, CursorLockMode.None);
                    break;
            }

            equipSlotEvent.Raise(index);
        }

        private void SetEquipSlotFlags(bool axe, bool pickaxe, bool hammer)
        {
            GameFlags.AXE_EQUIPPED = axe;
            GameFlags.PICKAXE_EQUIPPED = pickaxe;
            GameFlags.HAMMER_EQUIPPED = hammer;
        }

        public static void SetCursorState(bool visible, CursorLockMode lockMode)
        {
            Cursor.visible = visible;
            Cursor.lockState = lockMode;
        }

        public static void EnableCursor()
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        public static void DisableCursor()
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
}