using System;
using System.Collections.Generic;
using System.Linq;
using GameEventSystem;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace Player.Input
{
    public class InputManager : MonoBehaviour
    {
        [SerializeField] private GameEvent movementInputEvent;
        [SerializeField] private GameEvent mouseMoveEvent;
        [SerializeField] private GameEvent mouseScrollEvent;
        
        private static Vector2 _movementInput;
            
        public static Vector2 MovementInput => _movementInput;
    
        private PlayerControls _playerControls;

        private void Awake()
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
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
    }
}
