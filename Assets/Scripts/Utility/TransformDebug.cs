using System;
using UnityEngine;

namespace Utility
{
    [Serializable]
    public class TransformDebug : MonoBehaviour
    { 
        public void OnDrawGizmos() {
            if (DebugInformation.Instance.drawTransformPositions ) {
                Transform t = transform;
                Vector3 position = t.position;
                DebugInformation.DrawArrow(position, t.right, Color.red);
                DebugInformation.DrawArrow(position, t.forward, Color.blue);
                DebugInformation.DrawArrow(position, t.up, Color.green);
            }
        }
    }
}