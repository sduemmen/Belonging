using UnityEngine;

namespace SaveSystem.Data
{
    public class PlayerDataPersistence : MonoBehaviour, IDataPersistence
    {
        [SerializeField] private Transform _transform;
        [SerializeField] private Vector3 _playerSpawnPosition;
        [SerializeField] private bool _firstLoad = true;
        
        private void Awake()
        {
            _transform = transform;
        }

        public void LoadData(GameData data)
        {
            if (data.firstLoad) {
                _playerSpawnPosition = data.playerSpawnPosition;
                _transform.position = data.playerSpawnPosition;
                _firstLoad = false;
            } else {
                _playerSpawnPosition = data.playerSpawnPosition;
                _transform.position = data.playerPosition;
                _transform.rotation = data.playerRotation;
                _firstLoad = false;
            }
        }

        public void SaveData(ref GameData data)
        {
            data.playerSpawnPosition = _playerSpawnPosition;
            data.playerPosition = _transform.position;
            data.playerRotation = _transform.rotation;
            data.firstLoad = _firstLoad;
        }
    }
}