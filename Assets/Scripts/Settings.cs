using System;
using UnityEngine;

public static class Settings 
{
    public static class InputSettings
    {
        [Serializable]
        public class CameraSettings
        {
            [Range(0, 100), SerializeField] private float x_Sensitivity;
            [SerializeField] private bool x_inverted;
        
            [Range(0, 100), SerializeField] private float y_Sensitivity;
            [SerializeField] private bool y_inverted;

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
