using Cinemachine;
using Flags;
using UnityEngine;
using UnityEngine.InputSystem;
using static Flags.GameSettings.InputSettings;

namespace Player.Input
{
    public class CameraController : MonoBehaviour
    {
        public CameraSettings cameraSettings;
        public InputManager inputManager;

        [SerializeField] private Transform _cameraTarget;
        [SerializeField] private CinemachineVirtualCamera _camera;
        [SerializeField] private Cinemachine3rdPersonFollow _cinemachineFramingTransposer;
        private Transform _transform;
        private Vector3 targetRotation;
        [SerializeField] private float targetZoomLevel;
        [SerializeField] private float currentZoomLevel;

        private void Awake()
        {
            _transform = GetComponent<Transform>();
            currentZoomLevel = 1f;
            targetZoomLevel = 1f;
            CinemachineComponentBase componentBase = _camera.GetCinemachineComponent(CinemachineCore.Stage.Body);
            if (componentBase is Cinemachine3rdPersonFollow cinemachineFramingTransposer) {
                _cinemachineFramingTransposer = cinemachineFramingTransposer;
                _cinemachineFramingTransposer.CameraDistance = cameraSettings.defaultCameraZoom;
            }
        }

        private void Update()
        {
            HandleCameraRotation();
            HandleCameraZoom();
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
        
            Vector2 cameraRotationInput = Mouse.current.delta.ReadValue();

            float currentZoomLevelModifier = Mathf.Sqrt(currentZoomLevel);
            float rotationAroundX = cameraRotationInput.y * cameraSettings.Y_Sensitivity * cameraSettings.generalSensitivity;
            float rotationAroundY = cameraRotationInput.x * cameraSettings.X_Sensitivity * cameraSettings.generalSensitivity;
            
            targetRotation.x = Mathf.Clamp(targetRotation.x - rotationAroundX, 5, 60);
            targetRotation.y += rotationAroundY;
            
            _transform.rotation = Quaternion.Euler(targetRotation);
            inputManager.SetCameraRotationInput(Vector2.zero);
        }

        private void HandleCameraZoom()
        {
            if (GameFlags.INVENTORY_OPEN || GameFlags.GAME_PAUSED) return;
            
            float mouseScrollDelta = UnityEngine.Input.mouseScrollDelta.y;
            
            if (mouseScrollDelta != 0) {
                float delta = mouseScrollDelta * cameraSettings.zoomSensitivity * currentZoomLevel;
                targetZoomLevel = Mathf.Clamp(targetZoomLevel - delta, cameraSettings.minZoomLevel, cameraSettings.maxZoomLevel);
            }

            currentZoomLevel = Mathf.Lerp(currentZoomLevel, targetZoomLevel, cameraSettings.zoomDampen);
            
            _cinemachineFramingTransposer.CameraDistance = cameraSettings.defaultCameraZoom * currentZoomLevel;
        }

        private void FollowCameraTarget()
        {
            _transform.position = _cameraTarget.position;
        }
    }
}
