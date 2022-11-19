using System;
using BuildSystem;
using Events.Events;
using Flags;
using InventorySystem;
using QuestSystem;
using UnityEngine;

namespace Player.Input
{
    public class InputController : MonoBehaviour
    {
        [SerializeField] private SimpleEvent mouseMoveEvent;
        [SerializeField] private SimpleEvent mouseScrollEvent;

        [SerializeField] private IntEvent equipSlotEvent;
        [SerializeField] private SimpleEvent toolUsedEvent;
        [SerializeField] private IntEvent rotateSegmentEvent;

        [SerializeField] private InventoryController _inventoryDisplayContext;
        [SerializeField] private BuildingController _buildMenuDisplayContext;
        [SerializeField] private QuestController _questMenuDisplayContext;
        [SerializeField] private GameStateController _pauseMenuDisplayContext;

        private PlayerControls _playerControls;

        public static Vector2 MovementInput { get; private set; }
        public static bool SprintKeyHeldDown { get; private set; }
        public static bool MiddleMouseButtonHeldDown { get; private set; }
        public static bool LeftMouseButtonHeldDown { get; private set; }

        private void Awake()
        {
            _playerControls = new PlayerControls();

            // Mouse Move
            _playerControls.Camera.MouseDelta.performed += _ => {
                if ((GameFlags.GAME_PAUSED || GameFlags.UI_ELEMENT_OPEN || GameFlags.SLOT_EQUIPPED) && !MiddleMouseButtonHeldDown) return;

                mouseMoveEvent.Raise();
            };
            // Mouse Scroll
            _playerControls.Camera.MouseScrollDelta.performed += _ => {
                if (GameFlags.GAME_PAUSED || GameFlags.HAMMER_EQUIPPED) return;

                mouseScrollEvent.Raise();
            };

            // EscapeKey pressed
            _playerControls.Character.PauseGame.performed += _ => {
                if (GameFlags.UI_ELEMENT_OPEN)
                {
                    DisplayContextController.Instance.HideCurrent();
                    return;
                }

                DisplayContextController.Instance.HideCurrentAndToggle(_pauseMenuDisplayContext, GameFlags.GAME_RUNNING);
            };
            // Movement Input
            _playerControls.Character.Movement.performed += inputEvent => {
                if (GameFlags.GAME_PAUSED) return;

                MovementInput = inputEvent.ReadValue<Vector2>();
            };
            // Open/Close Inventory
            _playerControls.Character.InventoryDisplayContext.performed += _ => {
                if (GameFlags.GAME_PAUSED) return;

                if (GameFlags.SLOT_EQUIPPED)
                {
                    SetEquippedSlot(-1);
                }
                
                DisplayContextController.Instance.HideCurrentAndToggle(_inventoryDisplayContext, GameFlags.INVENTORY_CLOSED);
            };
            // Open/Close Quest Menu
            _playerControls.Character.QuestDisplayContext.performed += _ => {
                if (GameFlags.GAME_PAUSED) return;
                
                if (GameFlags.HAMMER_EQUIPPED)
                {
                    SetEquippedSlot(-1);
                }

                DisplayContextController.Instance.HideCurrentAndToggle(_questMenuDisplayContext, GameFlags.QUEST_DISPLAY_CLOSED);
            };
            // Equip Slot 1
            _playerControls.Character.EquipSlot1.performed += _ => {
                if (GameFlags.GAME_PAUSED) return;

                DisplayContextController.Instance.HideCurrent();

                if (GameFlags.AXE_EQUIPPED)
                    SetEquippedSlot(-1);
                else
                    SetEquippedSlot(0);
            };
            // Equip Slot 2
            _playerControls.Character.EquipSlot2.performed += _ => {
                if (GameFlags.GAME_PAUSED) return;

                DisplayContextController.Instance.HideCurrent();

                if (GameFlags.PICKAXE_EQUIPPED)
                    SetEquippedSlot(-1);
                else
                    SetEquippedSlot(1);
            };
            // Equip Slot 3
            _playerControls.Character.EquipSlot3.performed += _ => {
                if (GameFlags.GAME_PAUSED) return;

                if (GameFlags.HAMMER_EQUIPPED && GameFlags.BUILD_MENU_CLOSED)
                {
                    DisplayContextController.Instance.HideCurrentAndDisplay(_buildMenuDisplayContext);
                    SetEquippedSlot(2);
                    return;
                }

                if (GameFlags.HAMMER_EQUIPPED)
                    SetEquippedSlot(-1);
                else
                    SetEquippedSlot(2);

                DisplayContextController.Instance.HideCurrentAndToggle(_buildMenuDisplayContext, GameFlags.BUILD_MENU_CLOSED);
            };
            // rotate segment
            _playerControls.Character.RotateSegment.performed += _ => {
                if (GameFlags.GAME_PAUSED || GameFlags.UI_ELEMENT_OPEN || !GameFlags.HAMMER_EQUIPPED) return;

                if (_playerControls.Character.Modifier1.inProgress)
                    rotateSegmentEvent.Raise(-90);
                else
                    rotateSegmentEvent.Raise(90);
            };
            // Action (left click)
            _playerControls.Character.Action.performed += _ => {
                if (GameFlags.GAME_PAUSED || GameFlags.INVENTORY_OPEN || GameFlags.BUILD_MENU_OPEN || !GameFlags.SLOT_EQUIPPED) return;

                toolUsedEvent.Raise(); // use equipped tool
            };
            // Cancel Action (right click)
            _playerControls.Character.CancelAction.performed += _ => {
                if (GameFlags.GAME_PAUSED) return;

                if (GameFlags.HAMMER_EQUIPPED && GameFlags.BUILD_MENU_CLOSED)
                {
                    DisplayContextController.Instance.HideCurrentAndDisplay(_buildMenuDisplayContext);
                    SetEquippedSlot(2);
                }
            };

            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            DisplayContextController.Instance.HideAll();
        }

        private void OnEnable()
        {
            _playerControls?.Enable();
        }

        private void OnDisable()
        {
            _playerControls?.Disable();
        }

        private void Update()
        {
            SprintKeyHeldDown = _playerControls.Character.Sprint.inProgress;
            MiddleMouseButtonHeldDown = _playerControls.Character.MiddleMouseButton.inProgress;
            LeftMouseButtonHeldDown = _playerControls.Character.HoldAction.inProgress;

            if (LeftMouseButtonHeldDown)
            {
                toolUsedEvent.Raise();
            }
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
    }
}