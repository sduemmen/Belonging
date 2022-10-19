using Cinemachine;
using Flags;
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