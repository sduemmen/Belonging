using System.Collections.Generic;
using System.Linq;
using Events.Events;
using Flags;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace Player.Input
{
    public class InputManager : MonoBehaviour
    {
        [SerializeField] private SimpleEvent movementInputEvent;
        [SerializeField] private SimpleEvent mouseMoveEvent;
        [SerializeField] private SimpleEvent mouseScrollEvent;
        
        [SerializeField] private SimpleEvent openInventoryEvent;
        [SerializeField] private SimpleEvent closeInventoryEvent;
        
        [SerializeField] private SimpleEvent equipSlot1Event;
        [SerializeField] private SimpleEvent equipSlot2Event;
        [SerializeField] private SimpleEvent equipSlot3Event;
        [SerializeField] private SimpleEvent toolUsedEvent;
        
        private static Vector2 _movementInput;
            
        public static Vector2 MovementInput => _movementInput;
    
        private PlayerControls _playerControls;

        private void Awake()
        {
            _playerControls = new PlayerControls();
            _playerControls.Character.Movement.performed += inputEvent => {
                _movementInput = inputEvent.ReadValue<Vector2>();
                movementInputEvent.Raise();
            };
            _playerControls.Camera.MouseDelta.performed += inputEvent => {
                mouseMoveEvent.Raise();
            };
            _playerControls.Camera.MouseScrollDelta.performed += inputEvent => {
                mouseScrollEvent.Raise();
            };
            _playerControls.Character.InventoryDisplayContext.performed += inputEvent => {
                if (GameFlags.INVENTORY_CLOSED) {
                    GameFlags.INVENTORY_OPEN = true;
                    EnableCursor();
                    openInventoryEvent.Raise();
                }
                else if (GameFlags.INVENTORY_OPEN) {
                    GameFlags.INVENTORY_OPEN = false;
                    DisableCursor();
                    closeInventoryEvent.Raise();
                }
            };
            _playerControls.Character.EquipSlot1.performed += inputEvent => {
                if (GameFlags.INVENTORY_OPEN) return;
                SetCursorState(false, CursorLockMode.None);
                equipSlot1Event.Raise();
            };
            _playerControls.Character.EquipSlot2.performed += inputEvent => {
                if (GameFlags.INVENTORY_OPEN) return;
                SetCursorState(false, CursorLockMode.None);
                equipSlot2Event.Raise();
            };
            _playerControls.Character.EquipSlot3.performed += inputEvent => {
                if (GameFlags.INVENTORY_OPEN) return;
                SetCursorState(true, CursorLockMode.None);
                equipSlot3Event.Raise();
            };
            _playerControls.Character.UseTool.performed += inputEvent => {
                if (GameFlags.INVENTORY_OPEN || !GameFlags.SLOT_EQUIPPED) return;
                toolUsedEvent.Raise();
            };
            
            DisableCursor();
            closeInventoryEvent.Raise();
        }

        private void OnEnable()
        {
            _playerControls.Enable();
        }

        private void OnDisable()
        {
            _playerControls.Disable();
        }

        public static bool IsPointerOverUIObject()
        {
            PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current) {
                position = Mouse.current.position.ReadValue()
            };
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventDataCurrentPosition, results);

            return results.Where(result => result.gameObject.layer == LayerMask.NameToLayer("UI")).ToArray().Length > 0;
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
