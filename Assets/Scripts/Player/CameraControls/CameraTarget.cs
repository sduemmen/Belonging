using SaveSystem;
using SaveSystem.Data;
using UnityEngine;

namespace CameraControls
{
    public class CameraTarget : MonoBehaviour, IDataPersistence
    {
        public Transform player;

        private void Update()
        {
            transform.position = player.position + Vector3.up * 1.5f;
        }

        public void LoadData(GameData data)
        {
            transform.rotation = data.cameraRotation;
        }

        public void SaveData(ref GameData data)
        {
            data.cameraRotation = transform.rotation;
        }
    }
}