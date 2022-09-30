using UnityEngine;

namespace SaveSystem.Data
{
    public class CameraDataPersistence : MonoBehaviour, IDataPersistence
    {
        private Transform _transform;

        private void Awake()
        {
            _transform = GetComponent<Transform>();
        }

        public void LoadData(GameData data)
        {
            _transform.rotation = data.cameraRotation;
        }

        public void SaveData(ref GameData data)
        {
            data.cameraRotation = _transform.rotation;
        }
    }
}