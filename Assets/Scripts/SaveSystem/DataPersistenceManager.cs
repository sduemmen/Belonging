using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Flags;
using SaveSystem.Data;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SaveSystem
{
    public class DataPersistenceManager : MonoBehaviour
    {
        private SaveLoadIO _saveLoadIO;
        
        [SerializeField] private GameData _gameData;
        private List<IDataPersistence> _dataPersistenceObjects;
        
        public static DataPersistenceManager instance { get; private set; }
        public string profileID = "default";
        public bool noProfileSelected => profileID == "default";

        private void Awake()
        {
            if (instance != null) {
                Debug.Log("Found more than one DataPersistenceManager in this scene. Destroying latest instance");
                Destroy(this.gameObject);
                return;
            }

            instance = this;
            DontDestroyOnLoad(this.gameObject);
            
            _saveLoadIO = new SaveLoadIO(Path.Combine(Application.persistentDataPath));
        }

        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.sceneUnloaded += OnSceneUnloaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            Debug.Log("Scene loaded");
            bool loadedSceneIsGameScene = scene.name == "GameScene";
            if (loadedSceneIsGameScene) LoadGame();
        }
        
        private void OnSceneUnloaded(Scene scene)
        {
            Debug.Log("Scene unloaded");
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

        public void NewGame(GameData gameData)
        {
            _gameData = gameData;
            SaveGame(forceSave:true);
        }

        public void LoadGame()
        {
            if (noProfileSelected) return;
            
            _gameData = _saveLoadIO.Load(profileID);

            _dataPersistenceObjects = FindAllDataPersistenceObjects();

            foreach (IDataPersistence dataPersistenceObject in _dataPersistenceObjects) {
                dataPersistenceObject.LoadData(_gameData);
            }
            
            foreach (PersistentItemData persistentItemData in _gameData.persistentItems) {
                GameObject gameObjectToInstantiate = PersistentItemData.GetGameObjectFromType(persistentItemData.prefabName);
                Instantiate(gameObjectToInstantiate, persistentItemData.worldPosition, persistentItemData.worldRotation);
            }

            foreach (PersistentDestroyableData persistentDestroyableData in _gameData.persistentDestroyables) {
                GameObject gameObjectToInstantiate = PersistentDestroyableData.GetGameObjectFromType(persistentDestroyableData.prefabName);
                Instantiate(gameObjectToInstantiate, persistentDestroyableData.worldPosition, persistentDestroyableData.worldRotation);
            }
            
            Debug.Log($"Loading complete {profileID}");
        }

        public void SaveGame(bool forceSave = false)
        {
            if ((noProfileSelected || GameFlags.MAIN_MENU_ACTIVE) && !forceSave) return;

            GameData storedData = _saveLoadIO.Load(profileID);

            if (storedData != null) {
                _gameData = storedData;
                _gameData.lastPlayed = DateTime.Now.ToFileTime();
            } else if (_gameData != null) {
                _gameData.lastPlayed = DateTime.Now.ToFileTime();
            } else {
                _gameData = new GameData();
            }
            
            _gameData.persistentDestroyables.Clear();
            _gameData.persistentItems.Clear();
            
            _dataPersistenceObjects = FindAllDataPersistenceObjects();
            
            if (_dataPersistenceObjects.Count > 0) {
                foreach (IDataPersistence dataPersistenceObject in _dataPersistenceObjects) {
                    dataPersistenceObject.SaveData(ref _gameData);
                }
            }
            
            _saveLoadIO.Save(_gameData, profileID);
            
            Debug.Log($"Saving complete {profileID}");
        }

        private List<IDataPersistence> FindAllDataPersistenceObjects()
        {
            IEnumerable<IDataPersistence> queryResult = FindObjectsOfType<MonoBehaviour>().OfType<IDataPersistence>();
            return new List<IDataPersistence>(queryResult);
        }

        public Dictionary<string, GameData> GetAllProfiles()
        {
            return _saveLoadIO.GetAllProfiles(); 
        }
    }
}