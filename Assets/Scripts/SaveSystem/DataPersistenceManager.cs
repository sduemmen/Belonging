using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BuildSystem;
using Environment;
using Flags;
using SaveSystem.Data;
using UnityEngine;
using UnityEngine.SceneManagement;
using World;

namespace SaveSystem
{
    public class DataPersistenceManager : MonoBehaviour
    {
        private static DataPersistenceManager _instance;
        public static DataPersistenceManager Instance {
            get {
                if (_instance == null)
                {
                    _instance = (DataPersistenceManager)FindObjectOfType(typeof(DataPersistenceManager));
                }

                return _instance;
            }
        }

        public string profileID = "default";
        [SerializeField] private GameData _gameData;
        private List<IDataPersistence> _persistentObjects;
        private SaveLoadIO _saveLoadIO;

        public bool NoProfileSelected => profileID == "default";

        private void Awake()
        {
            if (Instance != null)
            {
                // Destroy latest instance
                Destroy(this.gameObject);
                return;
            }
            
            DontDestroyOnLoad(this.gameObject);

            _saveLoadIO = new SaveLoadIO(Path.Combine(Application.persistentDataPath));
        }

        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.sceneUnloaded += OnSceneUnloaded;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            SceneManager.sceneUnloaded -= OnSceneUnloaded;
        }

        private void OnApplicationQuit()
        {
            SaveGame();
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            // Debug.Log("Scene loaded");
            bool loadedSceneIsGameScene = scene.name == "GameScene";
            if (loadedSceneIsGameScene) LoadGame();
        }

        private void OnSceneUnloaded(Scene scene)
        {
            // Debug.Log("Scene unloaded");
        }

        public void NewGame(GameData gameData)
        {
            _gameData = gameData;
            SaveGame(true);
        }

        private void LoadGame()
        {
            if (NoProfileSelected) return;

            _gameData = _saveLoadIO.Load(profileID);

            _persistentObjects = FindAllPersistentObjects();

            foreach (IDataPersistence dataPersistenceObject in _persistentObjects) dataPersistenceObject.LoadData(_gameData);

            foreach (PersistentItemData persistentItemData in _gameData.persistentItems)
            {
                GameObject gameObjectToInstantiate = PersistentItemData.GetGameObjectFromType(persistentItemData.prefabName);
                GameObject item = Instantiate(gameObjectToInstantiate, persistentItemData.worldPosition, persistentItemData.worldRotation);
                item.GetComponent<Pickupable>().Initialize(persistentItemData);
            }

            foreach (PersistentDestroyableData persistentDestroyableData in _gameData.persistentDestroyables)
            {
                GameObject gameObjectToInstantiate = PersistentDestroyableData.GetGameObjectFromType(persistentDestroyableData.prefabName);
                GameObject obj = Instantiate(gameObjectToInstantiate, persistentDestroyableData.worldPosition, persistentDestroyableData.worldRotation);
                SegmentPreview preview = obj.GetComponent<SegmentPreview>();
                if (preview != null)
                {
                    preview.ResetMaterial();
                    Destroy(preview);
                }
            }

            Debug.Log($"Loading complete (profile={profileID})");
        }

        private void SaveGame(bool forceSave = false)
        {
            if ((NoProfileSelected || GameFlags.MAIN_MENU_ACTIVE) && !forceSave) return;

            GameData storedData = _saveLoadIO.Load(profileID);

            if (storedData != null)
            {
                _gameData = storedData;
                _gameData.lastPlayed = DateTime.Now.ToFileTime();
            }
            else if (_gameData != null)
            {
                _gameData.lastPlayed = DateTime.Now.ToFileTime();
            }
            else
            {
                _gameData = new GameData();
            }

            _gameData.persistentDestroyables.Clear();
            _gameData.persistentItems.Clear();

            _persistentObjects = FindAllPersistentObjects();

            if (_persistentObjects.Count > 0)
            {
                foreach (IDataPersistence persistentObject in _persistentObjects)
                {
                    persistentObject.SaveData(ref _gameData);
                }
            }

            _saveLoadIO.Save(_gameData, profileID);

            Debug.Log($"Saving complete (profile={profileID})");
        }

        private List<IDataPersistence> FindAllPersistentObjects()
        {
            var dataPersistentObjects = FindObjectsOfType<MonoBehaviour>().OfType<IDataPersistence>();
            return new List<IDataPersistence>(dataPersistentObjects);
        }

        public Dictionary<string, GameData> GetAllProfiles()
        {
            return _saveLoadIO.GetAllProfiles();
        }
    }
}