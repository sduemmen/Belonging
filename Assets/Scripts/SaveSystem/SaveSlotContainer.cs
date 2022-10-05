using System.Collections.Generic;
using System.IO;
using SaveSystem.Data;
using UnityEngine;

namespace SaveSystem
{
    public class SaveSlotContainer : MonoBehaviour
    {
        private List<SaveSlot> _saveSlots;
        [SerializeField] private GameObject saveSlotPrefab;
        [SerializeField] private GameObject noSaveSlotsHint;
        public SaveSlot selectedSaveSlot;

        private void Awake()
        {
            _saveSlots = new List<SaveSlot>();
        }

        public void LoadSaveSlots()
        {
            Dictionary<string, GameData> profileGameData = DataPersistenceManager.instance.GetAllProfiles();

            foreach (KeyValuePair<string,GameData> entry in profileGameData) {
                if (entry.Value != null) {
                    GameObject saveSlotObject = Instantiate(saveSlotPrefab, Vector3.zero, Quaternion.Euler(Vector3.zero));
                    saveSlotObject.transform.SetParent(this.transform);
                    
                    SaveSlot saveSlot = saveSlotObject.GetComponent<SaveSlot>();
                    saveSlot.uiSaveSlot.SetValues(entry.Value);
                    saveSlot.uiSaveSlot.selectButton.onClick.AddListener(() => {
                        if (selectedSaveSlot != null) selectedSaveSlot.uiSaveSlot.OnUnselect();
                        selectedSaveSlot = saveSlot;
                        selectedSaveSlot.uiSaveSlot.OnSelect();
                        DataPersistenceManager.instance.profileID = entry.Value.profileID;
                    });
                    
                    _saveSlots.Add(saveSlot);
                }
            }

            CheckSaveSlotCount();
        }

        public void UnloadSaveSlots()
        {
            DataPersistenceManager.instance.profileID = "default";
            noSaveSlotsHint.SetActive(false);
            selectedSaveSlot = null;
            
            if (_saveSlots == null) return;
            
            foreach (SaveSlot saveSlot in _saveSlots.ToArray()) {
                _saveSlots.Remove(saveSlot);
                saveSlot.uiSaveSlot.selectButton.onClick.RemoveAllListeners();
                Destroy(saveSlot.uiSaveSlot.gameObject);
            }
        }

        public void DeleteSelectedSaveSlot()
        {
            if (selectedSaveSlot == null) return;

            SaveLoadIO saveLoadIO = new SaveLoadIO(Path.Combine(Application.persistentDataPath));
            saveLoadIO.Delete(selectedSaveSlot.gameData.profileID);
            
            DataPersistenceManager.instance.profileID = "default";
            _saveSlots.Remove(selectedSaveSlot);
            Destroy(selectedSaveSlot.uiSaveSlot.gameObject);
            
            selectedSaveSlot = null;
            CheckSaveSlotCount();
        }

        public void CheckSaveSlotCount()
        {
            if (_saveSlots == null || _saveSlots.Count == 0) {
                noSaveSlotsHint.SetActive(true);
            } else {
                noSaveSlotsHint.SetActive(false);
            }
        }
    }
}
