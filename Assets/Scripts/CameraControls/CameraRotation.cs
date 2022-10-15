using Cinemachine;
using Flags;
using Player.Input;
using UnityEngine;
using UnityEngine.InputSystem;
using Values.References;

namespace CameraControls
{
    [RequireComponent(typeof(CinemachineVirtualCamera))]
    public class CameraRotation : MonoBehaviour
    {
        [SerializeField] private FloatReference xSensitivity;
        [SerializeField] private BoolReference xInverted;
        [SerializeField] private FloatReference ySensitivity;
        [SerializeField] private BoolReference yInverted;
        [SerializeField] private FloatReference generalSensitivity;

        [SerializeField] private Transform _cameraTarget;
        
        private Vector3 targetRotation;
        
        public void HandleCameraRotation()
        {
            if (UserInputFlags.SUPPRESS_CAMERA_ROTATION_KEY_PRESSED || GameFlags.INVENTORY_OPEN || GameFlags.SLOT_EQUIPPED) {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                return;
            }
        
            if (UserInputFlags.SUPPRESS_CAMERA_ROTATION_KEY_RELEASED || GameFlags.INVENTORY_CLOSED) {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
        
            Vector2 cameraRotationInput = Mouse.current.delta.ReadValue();
            
            float rotationAroundX = cameraRotationInput.y * ySensitivity.value * generalSensitivity.value;
            float rotationAroundY = cameraRotationInput.x * xSensitivity.value * generalSensitivity.value;
            rotationAroundX = xInverted.value ? -rotationAroundX : rotationAroundX;
            rotationAroundY = yInverted.value ? -rotationAroundY : rotationAroundY;
            
            targetRotation.x = Mathf.Clamp(targetRotation.x - rotationAroundX, 5, 60);
            targetRotation.y += rotationAroundY;
            
            _cameraTarget.rotation = Quaternion.Euler(targetRotation);
        }
    }
}