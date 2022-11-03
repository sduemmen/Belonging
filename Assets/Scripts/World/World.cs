using System;
using System.Collections;
using System.Collections.Generic;
using Environment;
using SaveSystem;
using SaveSystem.Data;
using UnityEditor;
using UnityEngine;
using Utility;

namespace World
{
    public class World : MonoBehaviour, IDataPersistence
    {
        public const int CHUNK_SIZE = 30;
        private static World _instance;

        [SerializeField] private Transform _player;
        [SerializeField] private GameObject _chunkPrefab;

        public int seed;
        public float treeDensityThreshold;
        public float stoneDensityThreshold;
        public int objectDistance;
        public WorldAlterations worldAlterations;
        public int placedSegments;
        private List<GameObject> _halfLoadedChunks;
        private List<GameObject> _loadedChunks;

        public static World Instance {
            get {
                if (_instance == null) _instance = (World)FindObjectOfType(typeof(World));
                return _instance;
            }
        }

        private void OnDrawGizmos()
        {
            Handles.color = Color.red;
            Handles.DrawLine(new Vector3(100, 0, 15), new Vector3(-100, 0, 15));
            Handles.DrawLine(new Vector3(100, 0, -15), new Vector3(-100, 0, -15));
            Handles.DrawLine(new Vector3(100, 0, 45), new Vector3(-100, 0, 45));
            Handles.DrawLine(new Vector3(100, 0, -45), new Vector3(-100, 0, -45));
            Handles.DrawLine(new Vector3(100, 0, 75), new Vector3(-100, 0, 75));
            Handles.DrawLine(new Vector3(100, 0, -75), new Vector3(-100, 0, -75));
            
            Handles.DrawLine(new Vector3(15, 0, 100), new Vector3(15, 0, -100));
            Handles.DrawLine(new Vector3(-15, 0, 100), new Vector3(-15, 0, -100));
            Handles.DrawLine(new Vector3(45, 0, 100), new Vector3(45, 0, -100));
            Handles.DrawLine(new Vector3(-45, 0, 100), new Vector3(-45, 0, -100));
            Handles.DrawLine(new Vector3(75, 0, 100), new Vector3(75, 0, -100));
            Handles.DrawLine(new Vector3(-75, 0, 100), new Vector3(-75, 0, -100));
            
        }

        private void Awake()
        {
            _loadedChunks = new List<GameObject>();
            _halfLoadedChunks = new List<GameObject>();
            InvokeRepeating(nameof(UpdateChunks), 0f, 0.2f); // update chunks every .2 seconds
        }

        private void UpdateChunks()
        {
            Vector3 playerPosition = _player.position;
            Vector2Int playerChunkPosition = Coordinates.GetChunkCoordinates(playerPosition.x, playerPosition.z);
            var chunksToBeLoaded = new List<Vector2Int>();
            var chunksToBeHalfLoaded = new List<Vector2Int>();

            // get chunks around player
            for (int y = -3; y <= 3; y++)
            { 
                for (int x = -3; x <= 3; x++)
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
                    
                    // StartCoroutine(LoadChunkAfterTime(chunkPos, worldSpacePosition, i * .01f));
                }
            }

            // load bigger radius of chunks without instantiating objects
            for (int y = -5; y <= 5; y++)
            {
                for (int x = -5; x <= 5; x++)
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

        private IEnumerator LoadChunkAfterTime(Vector2Int chunkPos, Vector2 worldSpacePosition, float delay)
        {
            yield return new WaitForSecondsRealtime(delay);
            
            GameObject chunkObj = Instantiate(_chunkPrefab, new Vector3(worldSpacePosition.x, 0, worldSpacePosition.y), Quaternion.identity);
            chunkObj.transform.SetParent(transform);

            Chunk chunk = chunkObj.GetComponent<Chunk>();
            chunk.chunkPosition = chunkPos;
            chunk.SpawnObjects();

            _loadedChunks.Add(chunkObj);
        }
        
        public void LoadData(GameData data)
        {
            seed = data.seed;
            treeDensityThreshold = data.treeDensityThreshold;
            stoneDensityThreshold = data.stoneDensityThreshold;
            placedSegments = data.placedSegments;
            
            foreach (string worldAlteration in data.worldAlterations)
            {
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
            
            foreach (UInt128 worldAlteration in worldAlterations.GetAlterations())
            {
                data.worldAlterations.Add(worldAlteration.ToString());
            }
        }
    }
}