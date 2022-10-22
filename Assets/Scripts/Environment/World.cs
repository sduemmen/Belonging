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
        
        private List<GameObject> loadedChunks;
        private List<GameObject> halfLoadedChunks;
        public GameObject chunkPrefab;
        public const int CHUNK_SIZE = 30;
        
        public int seed;
        public float treeDensityThreshold;
        public float stoneDensityThreshold;
        public int objectDistance;
        public WorldAlterations worldAlterations;
        public int placedSegments;

        /// <summary>
        /// Sample 2-Dimensional Perlin-Noise from world coordinates
        /// </summary>
        /// <param name="x">x coordinate in world space</param>
        /// <param name="y">y coordinate in world space</param>
        /// <param name="xOffset">offset on x-axis</param>
        /// <param name="yOffset">offset on y-axis</param>
        /// <returns></returns>
        public static float SamplePerlin2d(float x, float y, float xOffset, float yOffset)
        {
            // apply seed (offset) to x and y world coordinates
            float seededX = x + xOffset;
            float seededY = y + yOffset;

            // combine chunk and seed
            float xSampleCoord = seededX / World.CHUNK_SIZE;
            float ySampleCoord = seededY / World.CHUNK_SIZE;

            return CalculateNoise(xSampleCoord, ySampleCoord);
        }
        
        /// <summary>
        /// Sample 2-Dimensional Perlin-Noise from local coordinates
        /// </summary>
        /// <param name="x">local x coordinate in chunk</param>
        /// <param name="y">local y coordinate in chunk</param>
        /// <param name="chunkCoordinates">coordinates of the chunk containing x, y</param>
        /// <param name="xOffset"></param>
        /// <param name="yOffset"></param>
        /// <returns></returns>
        public static float SamplePerlin2d(float x, float y, Vector2Int chunkCoordinates, float xOffset, float yOffset)
        {
            // Get coordinates in world space
            Vector2 worldCoords = GetWorldCoordinates(chunkCoordinates, x, y);
            
            // apply seed to x and y world coordinates
            float seededX = worldCoords.x + xOffset;
            float seededY = worldCoords.y + yOffset;
            
            // combine chunk and seed
            float xSampleCoord = seededX / World.CHUNK_SIZE;
            float ySampleCoord = seededY / World.CHUNK_SIZE;

            return CalculateNoise(xSampleCoord, ySampleCoord);
        }
        
        public static Vector2Int GetChunkCoordinates(float worldX, float worldY)
        {
            int chunkX = (int)Math.Floor(worldX / World.CHUNK_SIZE);
            int chunkY = (int)Math.Floor(worldY / World.CHUNK_SIZE);
            return new Vector2Int(chunkX, chunkY);
        }

        public static Vector2 GetWorldCoordinates(Vector2Int chunkCoordinates, float x, float y)
        {
            int halfChunkSize = World.CHUNK_SIZE / 2;
            float worldX = chunkCoordinates.x * World.CHUNK_SIZE + x - halfChunkSize;
            float worldY = chunkCoordinates.y * World.CHUNK_SIZE + y - halfChunkSize;
            return new Vector2(worldX, worldY);
        }
        
        public static float CalculateNoise(float x, float y)
        {
            float persistance = .4f;
            int roughness = 3;
            int octaves = 3;
            
            float noise = 0;
            float frequency = 1;
            float factor = 1;

            for (int i = 0; i < octaves; i++) {
                noise += Mathf.PerlinNoise(x * frequency + i, y * frequency + i) * factor;
                factor *= persistance;
                frequency *= roughness;
            }

            return noise / .45f - 1;
        }

        private void Awake()
        {
            loadedChunks = new List<GameObject>();
            halfLoadedChunks = new List<GameObject>();
            InvokeRepeating(nameof(UpdateChunks), 0f, 0.2f); // update chunks every .2 seconds
        }

        private void UpdateChunks()
        {
            var playerPosition = player.transform.position;
            Vector2Int playerChunkPosition = GetChunkCoordinates(playerPosition.x, playerPosition.z);
            List<Vector2Int> chunksToBeLoaded = new List<Vector2Int>();
            List<Vector2Int> chunksToBeHalfLoaded = new List<Vector2Int>();

            // get chunks around player
            for (int y = -3; y <= 3; y++) {
                for (int x = -3; x <= 3; x++) {
                    chunksToBeLoaded.Add(new Vector2Int(playerChunkPosition.x + x, playerChunkPosition.y + y));
                }
            }

            // unload old chunks
            for (int i = loadedChunks.Count - 1; i >= 0; i--) {
                if (!chunksToBeLoaded.Contains(loadedChunks[i].GetComponent<Chunk>().chunkPosition)) {
                    Destroy(loadedChunks[i]);
                    loadedChunks.Remove(loadedChunks[i]);
                }
            }

            // load new chunks
            foreach (Vector2Int chunkPos in chunksToBeLoaded) {
                if (!loadedChunks.Exists(chunk => chunk.GetComponent<Chunk>().chunkPosition == chunkPos)) {
                    Vector2 chunkWorldPos = GetWorldCoordinates(chunkPos, 0, 0);
                    
                    GameObject chunkObj = Instantiate(chunkPrefab, new Vector3(chunkWorldPos.x, 0, chunkWorldPos.y), Quaternion.identity);
                    chunkObj.transform.SetParent(this.transform);
                    
                    Chunk chunk = chunkObj.GetComponent<Chunk>();
                    chunk.world = this;
                    chunk.chunkPosition = chunkPos;
                    chunk.SpawnObjects();
                    
                    loadedChunks.Add(chunkObj);
                }
            }
            
            // load bigger radius of chunks without instantiating objects
            for (int y = -5; y <= 5; y++) {
                for (int x = -5; x <= 5; x++) {
                    chunksToBeHalfLoaded.Add(new Vector2Int(playerChunkPosition.x + x, playerChunkPosition.y + y));
                }
            }
            
            for (int i = halfLoadedChunks.Count - 1; i >= 0; i--) {
                if (!chunksToBeHalfLoaded.Contains(halfLoadedChunks[i].GetComponent<Chunk>().chunkPosition)) {
                    Destroy(halfLoadedChunks[i]);
                    halfLoadedChunks.Remove(halfLoadedChunks[i]);
                }
            }

            foreach (Vector2Int chunkPos in chunksToBeHalfLoaded) {
                bool alreadyLoaded = loadedChunks.Exists(chunk => chunk.GetComponent<Chunk>().chunkPosition == chunkPos);
                bool alreadyHalfLoaded = halfLoadedChunks.Exists(chunk => chunk.GetComponent<Chunk>().chunkPosition == chunkPos);
                if (!alreadyLoaded && !alreadyHalfLoaded) {
                    Vector2 chunkWorldPos = GetWorldCoordinates(chunkPos, 0, 0);
                    
                    GameObject chunkObj = Instantiate(chunkPrefab, new Vector3(chunkWorldPos.x, 0, chunkWorldPos.y), Quaternion.identity);
                    chunkObj.transform.SetParent(this.transform);
                    
                    Chunk chunk = chunkObj.GetComponent<Chunk>();
                    chunk.world = this;
                    chunk.chunkPosition = chunkPos;
                    
                    halfLoadedChunks.Add(chunkObj);
                }
            }
        }

        public void LoadData(GameData data)
        {
            seed = data.seed;
            treeDensityThreshold = data.treeDensityThreshold;
            stoneDensityThreshold = data.stoneDensityThreshold;
            placedSegments = data.placedSegments;
            foreach (string worldAlteration in data.worldAlterations) {
                worldAlterations.AddAlteration(worldAlteration);
            }
        }

        public void SaveData(ref GameData data)
        {
            data.seed = seed;
            data.treeDensityThreshold = treeDensityThreshold;
            data.stoneDensityThreshold = stoneDensityThreshold;
            data.placedSegments = placedSegments;
            data.worldAlterations.Clear();
            foreach (UInt128 worldAlteration in worldAlterations.GetAlterations()) {
                data.worldAlterations.Add(worldAlteration.ToString());
            }
        }
    }
}
