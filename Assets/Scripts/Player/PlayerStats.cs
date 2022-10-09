using SaveSystem;
using SaveSystem.Data;
using UnityEngine;

namespace Player
{
    public class PlayerStats : MonoBehaviour, IDataPersistence
    {
        [SerializeField] private float playtime;
        [SerializeField] public int score;
        [SerializeField] public int unlocked;

        public void LoadData(GameData data)
        {
            playtime = data.playtime;
            score = data.score;
            unlocked = data.unlocked;
        }

        public void SaveData(ref GameData data)
        {
            data.playtime = playtime;
            data.score = score;
            data.unlocked = unlocked;
        }

        private void Update()
        {
            playtime += Time.deltaTime;
        }
    }
}
