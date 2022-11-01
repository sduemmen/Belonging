using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using Values.Constants;
using Values.References;

namespace CameraControls
{
    [RequireComponent(typeof(CinemachineVirtualCamera))]
    public class CameraZoom : MonoBehaviour
    {
        [SerializeField] private FloatReference _zoomSensitivity;
        [SerializeField] private FloatConstant _minZoomLevel;
        [SerializeField] private FloatConstant _maxZoomLevel;
        [SerializeField] private FloatConstant _defaultCameraZoomLevel;
        [SerializeField] private FloatConstant _zoomDampen;

        private CinemachineVirtualCamera _camera;
        private Cinemachine3rdPersonFollow _thirdPersonFollow;
        private float _currentZoomLevel;
        private float _targetZoomLevel;

        private void Awake()
        {
            _currentZoomLevel = _maxZoomLevel.value;
            _targetZoomLevel = _maxZoomLevel.value;
            _camera = GetComponent<CinemachineVirtualCamera>();
            CinemachineComponentBase componentBase = _camera.GetCinemachineComponent(CinemachineCore.Stage.Body);
            if (componentBase is Cinemachine3rdPersonFollow thirdPersonFollow)
            {
                _thirdPersonFollow = thirdPersonFollow;
                _thirdPersonFollow.CameraDistance = _defaultCameraZoomLevel.value;
            }
        }

        private void Update()
        {
            _currentZoomLevel = Mathf.Lerp(_currentZoomLevel, _targetZoomLevel, _zoomDampen.value);
            _thirdPersonFollow.CameraDistance = _defaultCameraZoomLevel.value * _currentZoomLevel;
        }

        public void HandleCameraZoom()
        {
            float mouseScrollDelta = Mouse.current.scroll.ReadValue().y;

            if (mouseScrollDelta != 0)
            {
                float delta = mouseScrollDelta / 100 * _zoomSensitivity.value * _currentZoomLevel;
                _targetZoomLevel = Mathf.Clamp(_targetZoomLevel - delta, _minZoomLevel.value, _maxZoomLevel.value);
            }
        }
    }
}