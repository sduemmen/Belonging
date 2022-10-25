using System.Collections.Generic;
using System.Linq;
using Flags;
using SaveSystem;
using SaveSystem.Data;
using TMPro;
using UnityEngine;

namespace UI.MainMenu
{
    public class SaveSlotContainer : MonoBehaviour
    {
        private List<SaveSlot> _saveSlots;
        [SerializeField] private GameObject saveSlotPrefab;
        [SerializeField] private TextMeshProUGUI noSaveSlotsHint;

        private void Awake()
        {
            _saveSlots = new List<SaveSlot>();
        }

        public void LoadSaveSlots()
        {
            Dictionary<string, GameData> profileGameData = DataPersistenceManager.instance.GetAllProfiles();

            foreach (KeyValuePair<string,GameData> entry in profileGameData.OrderByDescending(profile => profile.Value.lastPlayed)) {
                if (entry.Value != null) {
                    GameObject saveSlotObject = Instantiate(saveSlotPrefab, Vector3.zero, Quaternion.identity);
                    saveSlotObject.transform.SetParent(this.transform);
                    saveSlotObject.gameObject.transform.localScale = Vector3.one;
                    
                    SaveSlot saveSlot = saveSlotObject.GetComponent<SaveSlot>();
                    saveSlot.gameData = entry.Value;
                    saveSlot.uiSaveSlot.SetValues(entry.Value);
                    
                    _saveSlots.Add(saveSlot);
                }
            }

            CheckSaveSlotCount();
        }

        public void UnloadSaveSlots()
        {
            if (_saveSlots == null) return;
            
            foreach (SaveSlot saveSlot in _saveSlots.ToArray()) {
                _saveSlots.Remove(saveSlot);
                Destroy(saveSlot.uiSaveSlot.gameObject);
            }
        }

        public void DeleteSelectedSaveSlot()
        {
            if (DataPersistenceManager.instance.noProfileSelected) return;
            
            SaveLoadIO saveLoadIO = new SaveLoadIO(Application.persistentDataPath);
            saveLoadIO.Delete(DataPersistenceManager.instance.profileID);
            
            UnloadSaveSlots();
            LoadSaveSlots();
            CheckSaveSlotCount();
        }

        private void CheckSaveSlotCount()
        {
            if (_saveSlots == null || _saveSlots.Count == 0) {
                noSaveSlotsHint.text = "No saved games found";
            } else {
                noSaveSlotsHint.text = "";
            }
        }
    }
}
