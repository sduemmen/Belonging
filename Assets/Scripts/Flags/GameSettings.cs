using System;
using UnityEngine;

namespace Flags
{
    public static class GameSettings 
    {
        public static class InputSettings
        {
            [Serializable]
            public class CameraSettings
            {
                [Range(0, 0.1f)] public float generalSensitivity = .012f;
                
                [Range(0, 100), SerializeField] private float x_Sensitivity;
                [SerializeField] private bool x_inverted;
        
                [Range(0, 100), SerializeField] private float y_Sensitivity;
                [SerializeField] private bool y_inverted;

                [SerializeField] public float defaultCameraZoom = 12f;
                [SerializeField] public float zoomSensitivity = .1f;
                [Range(0.001f, 1), SerializeField] public float maxZoomLevel;
                [Range(0.001f, 1), SerializeField] public float minZoomLevel;
                [Range(0, 1), SerializeField] public float zoomDampen;

                public float X_Sensitivity => x_inverted ? -x_Sensitivity : x_Sensitivity;
                public float Y_Sensitivity => y_inverted ? -y_Sensitivity : y_Sensitivity;
            }
        }

        public static class PlayerSettings
        {
            [Serializable]
            public class MovementSettings
            {
                public float movementSpeed;
                public float rotationDamping;
            }
        }
    }
}
