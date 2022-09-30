using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    private Vector2 _movementInput;
    private Vector2 _cameraRotationInput;
    
    public Vector2 CameraRotationInput => _cameraRotationInput;
    public Vector2 MovementInput => _movementInput;
    
    private PlayerControls _playerControls;

    private void Awake()
    {
        _playerControls = new PlayerControls();
        _playerControls.Character.Movement.performed += inputEvent => {
            _movementInput = inputEvent.ReadValue<Vector2>();
        };
        _playerControls.Camera.MouseDelta.performed += inputEvent => {
            _cameraRotationInput = inputEvent.ReadValue<Vector2>();
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

    public void SetCameraRotationInput(Vector2 value)
    {
        _cameraRotationInput = value;
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
