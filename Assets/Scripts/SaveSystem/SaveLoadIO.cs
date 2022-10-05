using System;
using System.Collections.Generic;
using System.IO;
using SaveSystem.Data;
using UnityEngine;

namespace SaveSystem
{
    public class SaveLoadIO
    {
        private string _directory;
        private string _fileName = "save.game";
        // TODO - add encryption

        public SaveLoadIO(string directory)
        {
            _directory = directory;
        }

        public GameData Load(string profileID)
        {
            string path = Path.Combine(_directory, profileID, _fileName);

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

        public void Save(GameData data, string profileID)
        {
            string path = Path.Combine(_directory, profileID, _fileName);

            try {
                Directory.CreateDirectory(Path.GetDirectoryName(path));

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

        public void Delete(string profileID)
        {
            string fullPath = Path.Combine(_directory, profileID, _fileName);
            string path = Path.Combine(_directory, profileID);

            try {
                File.Delete(fullPath);
                Directory.Delete(path, true);
            } catch (Exception e) {
                Debug.LogError($"Exception when deleting file at: \n {fullPath} \n {e}");
            }
        }

        public Dictionary<string, GameData> GetAllProfiles()
        {
            Dictionary<string, GameData> profileDict = new Dictionary<string, GameData>();

            IEnumerable<DirectoryInfo> directoryInfos = new DirectoryInfo(_directory).EnumerateDirectories();
            foreach (DirectoryInfo directoryInfo in directoryInfos) {
                string profileID = directoryInfo.Name;

                string fullPath = Path.Combine(_directory, profileID, _fileName);
                if (!File.Exists(fullPath)) {

                    Debug.LogWarning($"No Savefile found in folder with profileID {profileID}");
                    continue;
                }

                GameData profileData = Load(profileID);

                if (profileData == null) {
                    Debug.LogError($"Tried loading Savefile with profileID {profileID} but something went wrong");
                    continue;
                }
                
                profileDict.Add(profileID, profileData);
            }

            return profileDict;
        }
    }
}