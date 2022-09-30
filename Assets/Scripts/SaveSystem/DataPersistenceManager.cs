using System;
using System.Collections.Generic;
using System.Linq;
using SaveSystem.Data;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SaveSystem
{
    public class DataPersistenceManager : MonoBehaviour
    {
        [SerializeField] private string _fileName;
        private SaveLoadIO _saveLoadIO;
        
        private GameData _gameData;
        private List<IDataPersistence> _dataPersistenceObjects;
        public static DataPersistenceManager instance { get; private set; }

        private void Awake()
        {
            if (instance != null) {
                Debug.LogError("Found more than one DataPersistenceManager in this scene");
            }

            instance = this;
        }

        private void Start()
        {
            _saveLoadIO = new SaveLoadIO(Application.persistentDataPath, _fileName);
        }

        private void Update()
        {
            if (Keyboard.current.enterKey.wasPressedThisFrame) SaveGame();
            if (Keyboard.current.rKey.wasPressedThisFrame) LoadGame();
            if (Keyboard.current.nKey.wasPressedThisFrame) NewGame();
        }

        public void NewGame()
        {
            _gameData = new GameData();
        }

        public void LoadGame()
        {
            _gameData = _saveLoadIO.Load();
            _dataPersistenceObjects = FindAllDataPersistenceObjects();
            
            if (_gameData == null) {
                Debug.LogWarning("No gameData was found. Initializing to default values");
                NewGame();
            }

            // TODO - initialize all other scripts that need it
            
            foreach (IDataPersistence dataPersistenceObject in _dataPersistenceObjects) {
                dataPersistenceObject.LoadData(_gameData);
            }
            
            foreach (PersistentGameObjectData persistentGameObjectData in _gameData.persistentGameObjects) {
                GameObject gameObjectToInstantiate = PersistentGameObjectData.GetGameObjectFromType(persistentGameObjectData.type);
                Instantiate(gameObjectToInstantiate, persistentGameObjectData.worldPosition, persistentGameObjectData.worldRotation);
            }
            
            Debug.Log("Loading complete");
        }

        public void SaveGame()
        {
            _gameData = new GameData();
            _dataPersistenceObjects = FindAllDataPersistenceObjects();
            
            if (_dataPersistenceObjects.Count > 0) {
                foreach (IDataPersistence dataPersistenceObject in _dataPersistenceObjects) {
                    dataPersistenceObject.SaveData(ref _gameData);
                }
            }
            
            _saveLoadIO.Save(_gameData);
            
            Debug.Log("Saving complete");
        }

        private List<IDataPersistence> FindAllDataPersistenceObjects()
        {
            IEnumerable<IDataPersistence> queryResult = FindObjectsOfType<MonoBehaviour>().OfType<IDataPersistence>();
            return new List<IDataPersistence>(queryResult);
        }
    }
}