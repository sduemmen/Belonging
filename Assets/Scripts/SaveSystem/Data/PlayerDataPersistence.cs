using UnityEngine;

namespace SaveSystem.Data
{
    public class PlayerDataPersistence : MonoBehaviour, IDataPersistence
    {
        [SerializeField] private Transform _transform;
        
        private void Awake()
        {
            _transform = transform;
        }

        public void LoadData(GameData data)
        {
            _transform.position = data.playerPosition;
            _transform.rotation = data.playerRotation;
        }

        public void SaveData(ref GameData data)
        {
            data.playerPosition = _transform.position;
            data.playerRotation = _transform.rotation;
        }
    }
}