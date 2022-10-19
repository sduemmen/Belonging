using System;
using Events.Events;
using Flags;
using SaveSystem.Data;
using TMPro;
using UnityEngine;

namespace Player.Input
{
    public class EventManager : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI gameFlagsLabel;
        
        [SerializeField] private SimpleEvent movementInputEvent;
        [SerializeField] private SimpleEvent mouseMoveEvent;
        [SerializeField] private SimpleEvent mouseScrollEvent;
        
        [SerializeField] private SimpleEvent openInventoryEvent;
        [SerializeField] private SimpleEvent closeInventoryEvent;
        
        [SerializeField] private SimpleEvent openBuildMenuEvent;
        [SerializeField] private SimpleEvent closeBuildMenuEvent;

        [SerializeField] private IntEvent equipSlotEvent;
        [SerializeField] private SimpleEvent toolUsedEvent;
        
        private static Vector2 _movementInput;
            
        public static Vector2 MovementInput => _movementInput;
    
        private PlayerControls _playerControls;

        private void Awake()
        {
            _playerControls = new PlayerControls();
            
            // Movement Input
            _playerControls.Character.Movement.performed += inputEvent => {
                if (GameFlags.GAME_PAUSED) return;

                _movementInput = inputEvent.ReadValue<Vector2>();
                movementInputEvent.Raise();
            };
            // Mouse Move
            _playerControls.Camera.MouseDelta.performed += inputEvent => {
                if (GameFlags.GAME_PAUSED || GameFlags.INVENTORY_OPEN || GameFlags.SLOT_EQUIPPED) return;
                
                mouseMoveEvent.Raise();     // Camera Rotation
            };
            // Mouse Scroll
            _playerControls.Camera.MouseScrollDelta.performed += inputEvent => {
                if (GameFlags.GAME_PAUSED) return;
                
                mouseScrollEvent.Raise();   // Camera Zoom
            };
            // Open/Close Inventory
            _playerControls.Character.InventoryDisplayContext.performed += inputEvent => {
                if (GameFlags.GAME_PAUSED) return;

                HideBuildMenu();
                SetEquippedSlot(-1);
                
                if (GameFlags.INVENTORY_CLOSED) {
                    OpenInventory();
                } else {
                    HideInventory();
                }
            };
            // Equip Slot 1
            _playerControls.Character.EquipSlot1.performed += inputEvent => {
                if (GameFlags.GAME_PAUSED) return;
                
                HideInventory();
                HideBuildMenu();
                
                if (GameFlags.AXE_EQUIPPED) {
                    SetEquippedSlot(-1);
                } else {
                    SetEquippedSlot(0);
                }
            };
            // Equip Slot 2
            _playerControls.Character.EquipSlot2.performed += inputEvent => {
                if (GameFlags.GAME_PAUSED) return;

                HideInventory();
                HideBuildMenu();
                
                SetCursorState(false, CursorLockMode.None);
                
                if (GameFlags.PICKAXE_EQUIPPED) {
                    SetEquippedSlot(-1);
                } else {
                    SetEquippedSlot(1);
                }
            };
            // Equip Slot 3
            _playerControls.Character.EquipSlot3.performed += inputEvent => {
                if (GameFlags.GAME_PAUSED) return;

                HideInventory();
                
                if (GameFlags.HAMMER_EQUIPPED && GameFlags.BUILD_MENU_CLOSED) {
                    OpenBuildMenu();
                    SetEquippedSlot(2);
                    return;
                }

                if (GameFlags.HAMMER_EQUIPPED) {
                    SetEquippedSlot(-1);
                } else {
                    SetEquippedSlot(2);
                }
                
                if (GameFlags.BUILD_MENU_CLOSED) {
                    OpenBuildMenu();
                } else {
                    HideBuildMenu();
                }
            };
            // Action (left click)
            _playerControls.Character.Action.performed += inputEvent => {
                if (GameFlags.GAME_PAUSED || GameFlags.INVENTORY_OPEN || !GameFlags.SLOT_EQUIPPED) return;
                
                toolUsedEvent.Raise();  // use equipped tool
            };
            // Cancel Action (right click)
            _playerControls.Character.CancelAction.performed += inputEvent => {
                if (GameFlags.GAME_PAUSED) return;
                
                if (GameFlags.HAMMER_EQUIPPED && GameFlags.BUILD_MENU_CLOSED) {
                    OpenBuildMenu();
                }
            };
            
            DisableCursor();
            closeInventoryEvent.Raise();
            closeBuildMenuEvent.Raise();
        }

        private void OnEnable()
        {
            _playerControls.Enable();
        }

        private void Update()
        {
            string result = "";
            result += nameof(GameFlags.GAME_PAUSED) + "=" + GetColorString(GameFlags.GAME_PAUSED) + "\n";
            result += nameof(GameFlags.MAIN_MENU_ACTIVE) + "=" + GetColorString(GameFlags.MAIN_MENU_ACTIVE) + "\n";
            
            result += nameof(GameFlags.INVENTORY_OPEN) + "=" + GetColorString(GameFlags.INVENTORY_OPEN) + "\n";
            result += nameof(GameFlags.INVENTORY_CLOSED) + "=" + GetColorString(GameFlags.INVENTORY_CLOSED) + "\n";
            result += nameof(GameFlags.BUILD_MENU_OPEN) + "=" + GetColorString(GameFlags.BUILD_MENU_OPEN) + "\n";
            result += nameof(GameFlags.BUILD_MENU_CLOSED) + "=" + GetColorString(GameFlags.BUILD_MENU_CLOSED) + "\n";
            result += nameof(GameFlags.UI_ELEMENT_OPEN) + "=" + GetColorString(GameFlags.UI_ELEMENT_OPEN) + "\n";
            
            result += nameof(GameFlags.AXE_EQUIPPED) + "=" + GetColorString(GameFlags.AXE_EQUIPPED) + "\n";
            result += nameof(GameFlags.PICKAXE_EQUIPPED) + "=" + GetColorString(GameFlags.PICKAXE_EQUIPPED) + "\n";
            result += nameof(GameFlags.HAMMER_EQUIPPED) + "=" + GetColorString(GameFlags.HAMMER_EQUIPPED) + "\n";
            result += nameof(GameFlags.SLOT_EQUIPPED) + "=" + GetColorString(GameFlags.SLOT_EQUIPPED) + "\n";
            gameFlagsLabel.text = result;
        }

        public static string GetColorString(bool b)
        {
            return b ? "<color=#00ff00>True<color=#4fc0f1>" : "<color=#ff0000>False<color=#4fc0f1>";
        }

        private void OnDisable()
        {
            _playerControls.Disable();
        }

        private void OpenInventory()
        {
            if (GameFlags.INVENTORY_OPEN) return;
            
            openInventoryEvent.Raise();
            GameFlags.INVENTORY_OPEN = true;
            SetCursorState(true, CursorLockMode.None);
        }

        private void HideInventory()
        {
            if (GameFlags.INVENTORY_CLOSED) return;
            
            closeInventoryEvent.Raise();
            GameFlags.INVENTORY_OPEN = false;
            SetCursorState(false, CursorLockMode.Locked);
        }
        
        private void OpenBuildMenu()
        {
            if (GameFlags.BUILD_MENU_OPEN) return;
            
            openBuildMenuEvent.Raise();
            GameFlags.BUILD_MENU_OPEN = true;
            SetCursorState(true, CursorLockMode.None);
        }

        private void HideBuildMenu()
        {
            if (GameFlags.BUILD_MENU_CLOSED) return;
            
            closeBuildMenuEvent.Raise();
            GameFlags.BUILD_MENU_OPEN = false;
            SetCursorState(false, CursorLockMode.Locked);
        }

        private void SetEquippedSlot(int index)
        {
            switch (index) {
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
