using UnityEngine;

namespace SaveSystem.Data
{
    public class PlayerDataPersistence : MonoBehaviour, IDataPersistence
    {
        [SerializeField] private Transform _player;
        [SerializeField] private Vector3 _playerSpawnPosition;
        [SerializeField] private bool _firstLoad = true;

        public void LoadData(GameData data)
        {
            if (data.firstLoad)
            {
                _playerSpawnPosition = data.playerSpawnPosition;
                _player.position = data.playerSpawnPosition;
            }
            else
            {
                _playerSpawnPosition = data.playerSpawnPosition;
                _player.position = data.playerPosition;
                _player.rotation = data.playerRotation;
            }

            _firstLoad = false;
        }

        public void SaveData(ref GameData data)
        {
            data.playerSpawnPosition = _playerSpawnPosition;
            data.playerPosition = _player.position;
            data.playerRotation = _player.rotation;
            data.firstLoad = _firstLoad;
        }
    }
}