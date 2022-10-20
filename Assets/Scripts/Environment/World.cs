using System;
using System.Collections.Generic;
using SaveSystem;
using SaveSystem.Data;
using UnityEngine;
using Utility;

namespace Environment
{
    public class World : MonoBehaviour, IDataPersistence
    {
        public GameObject player;
        
        public List<GameObject> activeChunks;
        public GameObject chunkPrefab;
        public int chunkSize;
        
        public int seed;
        public float seedOffset;
        public float treeThreshold;
        public float stoneThreshold;
        public int objectDistance;
        public WorldAlterations worldAlterations;
        public int placedSegments;

        private void Awake()
        {
            activeChunks = new List<GameObject>();
            InvokeRepeating(nameof(UpdateChunks), 0f, 0.2f);
        }

        private void UpdateChunks()
        {
            Vector2Int playerChunkPosition = GetPlayerChunkPosition(player.transform.position);
            List<Vector2Int> chunksToBeLoaded = new List<Vector2Int>();

            for (int y = -3; y <= 3; y++) {
                for (int x = -3; x <= 3; x++) {
                    chunksToBeLoaded.Add(new Vector2Int(playerChunkPosition.x + x, playerChunkPosition.y + y));
                }
            }

            // unload old chunks
            for (int i = activeChunks.Count - 1; i >= 0; i--) {
                if (!chunksToBeLoaded.Contains(activeChunks[i].GetComponent<Chunk>().chunkPosition)) {
                    Destroy(activeChunks[i]);
                    activeChunks.Remove(activeChunks[i]);
                }
            }

            // load new chunks
            foreach (Vector2Int chunkPos in chunksToBeLoaded) {
                if (!activeChunks.Exists(chunk => chunk.GetComponent<Chunk>().chunkPosition == chunkPos)) {
                    Vector3 chunkPositionInWorldSpace = new Vector3(chunkPos.x * chunkSize, 0, chunkPos.y * chunkSize);
                    GameObject chunkObj = Instantiate(chunkPrefab, chunkPositionInWorldSpace, Quaternion.identity);
                    chunkObj.transform.SetParent(this.transform);
                    Chunk chunk = chunkObj.GetComponent<Chunk>();
                    chunk.world = this;
                    chunk.chunkPosition = chunkPos;
                    chunk.size = chunkSize;
                    // chunk.InitializeMesh();
                    chunk.SpawnObjects();
                    activeChunks.Add(chunkObj);
                }
            }
        }

        public Vector2Int GetPlayerChunkPosition(Vector3 playerPosition)
        {
            int x = (int)Math.Floor(playerPosition.x / chunkSize);
            int z = (int)Math.Floor(playerPosition.z / chunkSize);
            return new Vector2Int(x, z);
        }

        public void LoadData(GameData data)
        {
            seed = data.seed;
            seedOffset = seedOffset = (float)seed / 100;
            treeThreshold = data.treeThreshold;
            stoneThreshold = data.stoneThreshold;
            placedSegments = data.placedSegments;
            foreach (string worldAlteration in data.worldAlterations) {
                worldAlterations.AddAlteration(worldAlteration);
            }
        }

        public void SaveData(ref GameData data)
        {
            data.seed = seed;
            data.treeThreshold = treeThreshold;
            data.stoneThreshold = stoneThreshold;
            data.placedSegments = placedSegments;
            data.worldAlterations.Clear();
            foreach (UInt128 worldAlteration in worldAlterations.GetAlterations()) {
                data.worldAlterations.Add(worldAlteration.ToString());
            }
        }
    }
}
