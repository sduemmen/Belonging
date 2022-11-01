using SaveSystem;
using SaveSystem.Data;
using UnityEngine;

namespace Player
{
    public class PlayerStats : MonoBehaviour, IDataPersistence
    {
        [SerializeField] private float playtime;
        [SerializeField] public int unlocked;

        private void Update()
        {
            playtime += Time.deltaTime;
        }

        public void LoadData(GameData data)
        {
            playtime = data.playtime;
            unlocked = data.unlocked;
        }

        public void SaveData(ref GameData data)
        {
            data.playtime = playtime;
            data.unlocked = unlocked;
        }
    }
}