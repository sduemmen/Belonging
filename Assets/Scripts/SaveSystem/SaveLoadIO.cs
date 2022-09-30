using System;
using System.IO;
using SaveSystem.Data;
using UnityEngine;

namespace SaveSystem
{
    public class SaveLoadIO
    {
        private string _directory = "";
        private string _fileName = "";

        public SaveLoadIO(string directory, string fileName)
        {
            _directory = directory;
            _fileName = fileName;
        }

        public GameData Load()
        {
            string path = Path.Combine(_directory, _fileName);

            GameData loadedData = null;

            try {
                string dataToLoad = "";

                using (FileStream stream = new FileStream(path, FileMode.Open)) {
                    using (StreamReader reader = new StreamReader(stream)) {
                        dataToLoad = reader.ReadToEnd();
                    }
                }

                loadedData = JsonUtility.FromJson<GameData>(dataToLoad);

            } catch (Exception e) {
                Debug.LogError($"Exception when loading from file: \n {path} \n {e}");
            }
            
            return loadedData;
        }

        public void Save(GameData data)
        {
            string path = Path.Combine(_directory, _fileName);

            try {
                if (!Directory.Exists(_directory)) Directory.CreateDirectory(_directory);

                string dataToStore = JsonUtility.ToJson(data, true);

                using (FileStream stream = new FileStream(path, FileMode.Create)) {
                    using (StreamWriter writer = new StreamWriter(stream)) {
                        writer.Write(dataToStore);
                    }
                }
            } catch (Exception e) {
                Debug.LogError($"Exception when saving to file: \n {path} \n {e}");
            }
        }
    }
}