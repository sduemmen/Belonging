using UnityEngine;

namespace SaveSystem.Data
{
    public class PlayerDataPersistence : MonoBehaviour, IDataPersistence
    {
        [SerializeField] private Transform _transform;
        [SerializeField] private Vector3 playerSpawnPosition;
        [SerializeField] private bool firstLoad = true;
        
        private void Awake()
        {
            _transform = transform;
        }

        public void LoadData(GameData data)
        {
            if (data.firstLoad) {
                playerSpawnPosition = data.playerSpawnPosition;
                _transform.position = data.playerSpawnPosition;
                firstLoad = false;
            } else {
                _transform.position = data.playerPosition;
                _transform.rotation = data.playerRotation;
            }
        }

        public void SaveData(ref GameData data)
        {
            data.playerSpawnPosition = playerSpawnPosition;
            data.playerPosition = _transform.position;
            data.playerRotation = _transform.rotation;
            data.firstLoad = firstLoad;
        }
    }
}