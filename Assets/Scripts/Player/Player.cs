using System.Collections.Generic;
using System.Linq;
using BuildSystem;
using SaveSystem;
using SaveSystem.Data;
using UI;
using UnityEngine;
using Utility;

namespace Player
{
    public class Player : Entity, IDataPersistence
    {
        public Transform m_player;

        public Vector3 m_spawnPoint;

        private LayerMask m_placementMask;
        
        private GameObject m_ghostSegment;

        // private GhostPlacementStatus m_ghostPlacementStatus;
        
        private int m_segmentRotationQuadrant;

        private List<Transform> m_snapPointsAroundGhost = new List<Transform>();
        
        private List<Transform> m_snapPointsInGhost = new List<Transform>();
        
        private List<Segment> m_segmentsAroundGhost;
        

        private void Awake()
        {
            m_placementMask = LayerMask.GetMask("Default", "Destructible", "Segment", "Ground");
            
            m_ghostSegment = null;
            // m_ghostPlacementStatus = GhostPlacementStatus.Valid;
            m_segmentRotationQuadrant = 0;
        }

        public void LoadData(GameData data)
        {
            m_spawnPoint = data.playerSpawnPosition;
            m_player.position = data.firstLoad ? m_spawnPoint : data.playerPosition;
            m_player.rotation = data.playerRotation;
        }

        public void SaveData(ref GameData data)
        {
            data.playerSpawnPosition = m_spawnPoint;
            data.playerPosition = m_player.position;
            data.playerRotation = m_player.rotation;
        }
    }
}