using UnityEngine;

namespace CameraControls
{
    public class CameraTarget : MonoBehaviour
    {
        public Transform player;

        private void Update()
        {
            transform.position = player.position + Vector3.up * 1.5f;
        }
    }
}