using System;
using System.Collections;
using System.Collections.Generic;
using Environment;
using SaveSystem;
using SaveSystem.Data;
using UnityEditor;
using UnityEngine;
using Utility;
using Random = UnityEngine.Random;

namespace World
{
    public class World : MonoBehaviour, IDataPersistence
    {
        public const int CHUNK_SIZE = 30;
        private static World _instance;
        public static World Instance {
            get {
                if (_instance == null) _instance = (World)FindObjectOfType(typeof(World));
                return _instance;
            }
        }
        
        [SerializeField] private Transform _player;
        [SerializeField] private int _chunkLoadingRadius = 3;
        [SerializeField] private int _chunkHalfLoadingRadius = 5;
        [SerializeField] private GameObject _chunkPrefab;

        public int seed;
        public float treeDensityThreshold;
        public float stoneDensityThreshold;
        public int objectDistance;
        public WorldAlterations worldAlterations;
        public int placedSegments;
        public float playtime;
        private List<GameObject> _halfLoadedChunks;
        private List<GameObject> _loadedChunks;

        private void Awake()
        {
            seed = Random.Range(1, 1000000);
            _loadedChunks = new List<GameObject>();
            _halfLoadedChunks = new List<GameObject>();
            InvokeRepeating(nameof(UpdateChunks), 0f, 0.2f);
        }

        private void Update()
        {
            playtime += Time.deltaTime;
        }

        private void UpdateChunks()
        {
            Vector3 playerPosition = _player.position;
            Vector2Int playerChunkPosition = Coordinates.GetChunkCoordinates(playerPosition.x, playerPosition.z);
            var chunksToBeLoaded = new List<Vector2Int>();
            var chunksToBeHalfLoaded = new List<Vector2Int>();

            // get chunks around player
            for (int y = -_chunkLoadingRadius; y <= _chunkLoadingRadius; y++)
            { 
                for (int x = -_chunkLoadingRadius; x <= _chunkLoadingRadius; x++)
                { 
                    chunksToBeLoaded.Add(new Vector2Int(playerChunkPosition.x + x, playerChunkPosition.y + y));
                }
            }

            // unload old chunks
            for (int i = _loadedChunks.Count - 1; i >= 0; i--)
            {
                if (!chunksToBeLoaded.Contains(_loadedChunks[i].GetComponent<Chunk>().chunkPosition))
                {
                    Destroy(_loadedChunks[i]);
                    _loadedChunks.Remove(_loadedChunks[i]);
                }
            }

            // load new chunks
            foreach (Vector2Int chunkPos in chunksToBeLoaded)
            {
                if (!_loadedChunks.Exists(chunk => chunk.GetComponent<Chunk>().chunkPosition == chunkPos))
                {
                    Vector2 chunkWorldPos = Coordinates.GetWorldCoordinates(chunkPos, 0, 0);

                    GameObject chunkObj = Instantiate(_chunkPrefab, new Vector3(chunkWorldPos.x, 0, chunkWorldPos.y), Quaternion.identity);
                    chunkObj.transform.SetParent(transform);

                    Chunk chunk = chunkObj.GetComponent<Chunk>();
                    chunk.chunkPosition = chunkPos;
                    chunk.SpawnObjects();

                    _loadedChunks.Add(chunkObj);
                }
            }

            // load bigger radius of chunks without instantiating objects
            for (int y = -_chunkHalfLoadingRadius; y <= _chunkHalfLoadingRadius; y++)
            {
                for (int x = -_chunkHalfLoadingRadius; x <= _chunkHalfLoadingRadius; x++)
                {
                    chunksToBeHalfLoaded.Add(new Vector2Int(playerChunkPosition.x + x, playerChunkPosition.y + y));
                }
            }

            for (int i = _halfLoadedChunks.Count - 1; i >= 0; i--)
            {
                if (!chunksToBeHalfLoaded.Contains(_halfLoadedChunks[i].GetComponent<Chunk>().chunkPosition))
                {
                    Destroy(_halfLoadedChunks[i]);
                    _halfLoadedChunks.Remove(_halfLoadedChunks[i]);
                }
            }

            foreach (Vector2Int chunkPos in chunksToBeHalfLoaded)
            {
                bool alreadyLoaded = _loadedChunks.Exists(chunk => chunk.GetComponent<Chunk>().chunkPosition == chunkPos);
                bool alreadyHalfLoaded = _halfLoadedChunks.Exists(chunk => chunk.GetComponent<Chunk>().chunkPosition == chunkPos);
                if (!alreadyLoaded && !alreadyHalfLoaded)
                {
                    Vector2 chunkWorldPos = Coordinates.GetWorldCoordinates(chunkPos, 0, 0);

                    GameObject chunkObj = Instantiate(_chunkPrefab, new Vector3(chunkWorldPos.x, 0, chunkWorldPos.y), Quaternion.identity);
                    chunkObj.transform.SetParent(transform);

                    Chunk chunk = chunkObj.GetComponent<Chunk>();
                    chunk.chunkPosition = chunkPos;

                    _halfLoadedChunks.Add(chunkObj);
                }
            }
        }
        
        public void LoadData(GameData data)
        {
            seed = data.seed;
            treeDensityThreshold = data.treeDensityThreshold;
            stoneDensityThreshold = data.stoneDensityThreshold;
            placedSegments = data.placedSegments;
            playtime = data.playtime;
            
            foreach (string worldAlteration in data.worldAlterations)
            {
                worldAlterations.AddAlteration(worldAlteration);
            }
        }

        public void SaveData(ref GameData data)
        {
            data.seed = data.seed != 0 ? data.seed : seed;
            data.treeDensityThreshold = data.treeDensityThreshold != 0 ? data.treeDensityThreshold : treeDensityThreshold;
            data.stoneDensityThreshold = data.stoneDensityThreshold != 0 ? data.stoneDensityThreshold : stoneDensityThreshold;
            data.placedSegments = placedSegments;
            data.playtime = playtime;
            
            data.worldAlterations.Clear();
            foreach (UInt128 worldAlteration in worldAlterations.GetAlterations())
            {
                data.worldAlterations.Add(worldAlteration.ToString());
            }
        }
    }
}