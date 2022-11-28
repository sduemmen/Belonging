using SaveSystem;
using SaveSystem.Data;
using UnityEngine;

namespace Player
{
    public class Player : Entity, IDataPersistence
    {
        public Transform m_player;

        public Vector3 m_spawnPoint;

        public PlayerSkills m_playerSkills = new PlayerSkills();

        public FXList m_levelUpFX;


        private void Awake()
        {
            m_playerSkills.skillLevelUpDelegate += OnLevelUp;
        }

        private void OnLevelUp()
        {
            m_levelUpFX.PlayFX(transform.position + Vector3.up);
        }

        public void LoadData(GameData data)
        {
            m_spawnPoint = data.playerSpawnPosition;
            m_player.position = data.firstLoad ? m_spawnPoint : data.playerPosition;
            m_player.rotation = data.playerRotation;
            m_playerSkills.m_skills = data.firstLoad ? m_playerSkills.m_skills : data.playerSkills;
        }

        public void SaveData(ref GameData data)
        {
            data.playerSpawnPosition = m_spawnPoint;
            data.playerPosition = m_player.position;
            data.playerRotation = m_player.rotation;
            data.playerSkills = m_playerSkills.m_skills;
        }
    }
}