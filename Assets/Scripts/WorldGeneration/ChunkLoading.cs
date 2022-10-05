using System;
using System.Collections.Generic;
using UnityEngine;

namespace WorldGeneration
{
    public class ChunkLoading : MonoBehaviour
    {
        public GameObject player;
        public List<GameObject> activeChunks;
        public int chunkSize;
        public GameObject chunkPrefab;

        private void Awake()
        {
            activeChunks = new List<GameObject>();
            InvokeRepeating(nameof(UpdateChunks), 0f, 0.1f);
        }

        private void UpdateChunks()
        {
            Vector2 playerChunkPosition = GetPlayerChunkPosition(player.transform.position);
            List<Vector2> chunksToBeLoaded = new List<Vector2>();

            for (int y = -2; y <= 2; y++) {
                for (int x = -2; x <= 2; x++) {
                    chunksToBeLoaded.Add(new Vector2(playerChunkPosition.x + x, playerChunkPosition.y + y));
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
            foreach (Vector2 chunkPos in chunksToBeLoaded) {
                if (!activeChunks.Exists(chunk => chunk.GetComponent<Chunk>().chunkPosition == chunkPos)) {
                    Vector3 chunkPositionInWorldSpace = new Vector3(chunkPos.x * chunkSize, 0, chunkPos.y * chunkSize);
                    GameObject chunkObj = Instantiate(chunkPrefab, chunkPositionInWorldSpace, Quaternion.Euler(Vector3.zero));
                    chunkObj.transform.SetParent(this.transform);
                    Chunk chunk = chunkObj.GetComponent<Chunk>();
                    chunk.size = chunkSize;
                    chunk.chunkPosition = chunkPos;
                    chunk.InitializeMesh();
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
    }
}
