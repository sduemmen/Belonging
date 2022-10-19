using System;
using Cinemachine;
using Flags;
using Player.Input;
using UnityEngine;
using UnityEngine.InputSystem;
using Values.Constants;
using Values.References;

namespace CameraControls
{
    [RequireComponent(typeof(CinemachineVirtualCamera))]
    public class CameraZoom : MonoBehaviour
    {
        [SerializeField] private FloatReference zoomSensitivity;
        [SerializeField] private FloatConstant minZoomLevel;
        [SerializeField] private FloatConstant maxZoomLevel;
        [SerializeField] private FloatConstant defaultCameraZoomLevel;
        [SerializeField] private FloatConstant zoomDampen;

        private CinemachineVirtualCamera _camera;
        private Cinemachine3rdPersonFollow _thirdPersonFollow;
        private float currentZoomLevel;
        private float targetZoomLevel;

        private void Awake()
        {
            currentZoomLevel = maxZoomLevel.value;
            targetZoomLevel = maxZoomLevel.value;
            _camera = GetComponent<CinemachineVirtualCamera>();
            CinemachineComponentBase componentBase = _camera.GetCinemachineComponent(CinemachineCore.Stage.Body);
            if (componentBase is Cinemachine3rdPersonFollow thirdPersonFollow) {
                _thirdPersonFollow = thirdPersonFollow;
                _thirdPersonFollow.CameraDistance = defaultCameraZoomLevel.value;
            }
        }

        private void Update()
        {
            currentZoomLevel = Mathf.Lerp(currentZoomLevel, targetZoomLevel, zoomDampen.value);
            _thirdPersonFollow.CameraDistance = defaultCameraZoomLevel.value * currentZoomLevel;
        }

        public void HandleCameraZoom()
        {
            float mouseScrollDelta = Mouse.current.scroll.ReadValue().y;
            
            if (mouseScrollDelta != 0) {
                float delta = (mouseScrollDelta / 100) * zoomSensitivity.value * currentZoomLevel;
                targetZoomLevel = Mathf.Clamp(targetZoomLevel - delta, minZoomLevel.value, maxZoomLevel.value);
            }
        }
    }
}