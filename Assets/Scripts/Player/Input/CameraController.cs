using System;
using Flags;
using UnityEngine;
using static Flags.GameSettings.InputSettings;

namespace Player.Input
{
    public class CameraController : MonoBehaviour
    {
        public CameraSettings cameraSettings;
        public InputManager inputManager;
    

        public Transform cameraTarget;
        private Transform _transform;
        private Vector3 targetRotation;

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
            if (UserInputFlags.SUPPRESS_CAMERA_ROTATION_KEY_PRESSED || GameFlags.INVENTORY_OPEN || GameFlags.SLOT_EQUIPPED) {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                return;
            }
        
            if (UserInputFlags.SUPPRESS_CAMERA_ROTATION_KEY_RELEASED || GameFlags.INVENTORY_CLOSED) {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
        
            Vector2 cameraRotationInput = inputManager.CameraRotationInput;
            
            float rotationAroundX = cameraRotationInput.y * cameraSettings.Y_Sensitivity * Time.deltaTime;
            float rotationAroundY = cameraRotationInput.x * cameraSettings.X_Sensitivity * Time.deltaTime;
            
            targetRotation.x = Mathf.Clamp(targetRotation.x - rotationAroundX, 10, 60);
            targetRotation.y += rotationAroundY;
            
            _transform.rotation = Quaternion.Euler(targetRotation);
            inputManager.SetCameraRotationInput(Vector2.zero);
        }

        private void FollowCameraTarget()
        {
            _transform.position = cameraTarget.position;
        }
    }
}
