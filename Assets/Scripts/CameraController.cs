using UnityEngine;
using UnityEngine.InputSystem;
using static Settings.InputSettings;

public class CameraController : MonoBehaviour
{
    public CameraSettings cameraSettings;
    public InputManager inputManager;
    
    private Vector3 targetRotation;

    public Transform cameraTarget;
    private Transform _transform;

    private void Awake()
    {
        _transform = GetComponent<Transform>();
    }

    private void Update()
    {
        HandleCameraRotation();
        FollowCameraTarget();
    }

    private void HandleCameraRotation()
    {
        if (Keyboard.current.leftShiftKey.isPressed) {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            return;
        }
        
        if (Keyboard.current.leftShiftKey.wasReleasedThisFrame) {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
        
        Vector2 cameraRotationInput = inputManager.CameraRotationInput;
        float rotationAngle = cameraRotationInput.x * cameraSettings.X_Sensitivity * Time.deltaTime;
        _transform.Rotate(Vector3.up, rotationAngle, Space.World);
        inputManager.SetCameraRotationInput(Vector2.zero);
    }

    private void FollowCameraTarget()
    {
        _transform.position = cameraTarget.position;
    }
}
