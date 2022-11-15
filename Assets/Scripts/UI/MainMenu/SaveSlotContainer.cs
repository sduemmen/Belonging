using System.Collections.Generic;
using System.Linq;
using SaveSystem;
using TMPro;
using UnityEngine;

namespace UI.MainMenu
{
    public class SaveSlotContainer : MonoBehaviour
    {
        [SerializeField] private GameObject saveSlotPrefab;
        [SerializeField] private TextMeshProUGUI noSaveSlotsHint;
        private List<SaveSlot> _saveSlots;

        private void Awake()
        {
            _saveSlots = new List<SaveSlot>();
        }

        public void LoadSaveSlots()
        {
            _saveSlots.Clear();
            
            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                SaveSlot saveSlot = transform.GetChild(i).GetComponent<SaveSlot>();
                Destroy(saveSlot.gameObject);
            }
            
            var profileGameData = DataPersistenceManager.Instance.GetAllProfiles();

            foreach (var entry in profileGameData.OrderByDescending(profile => profile.Value.lastPlayed))
            {
                if (entry.Value != null)
                {
                    GameObject saveSlotObject = Instantiate(saveSlotPrefab, Vector3.zero, Quaternion.identity);
                    saveSlotObject.transform.SetParent(transform);
                    saveSlotObject.gameObject.transform.localScale = Vector3.one;

                    SaveSlot saveSlot = saveSlotObject.GetComponent<SaveSlot>();
                    saveSlot.gameData = entry.Value;
                    saveSlot.uiSaveSlot.Initialize(entry.Value);

                    _saveSlots.Add(saveSlot);
                }
            }

            CheckSaveSlotCount();
        }

        public void UnloadSaveSlots()
        {
            if (_saveSlots == null) return;

            foreach (SaveSlot saveSlot in _saveSlots)
            {
                Destroy(saveSlot.uiSaveSlot.gameObject);
            }
            
            _saveSlots.Clear();
        }

        public void DeleteSelectedSaveSlot()
        {
            if (DataPersistenceManager.Instance.NoProfileSelected) return;

            SaveLoadIO saveLoadIO = new SaveLoadIO(Application.persistentDataPath);
            saveLoadIO.Delete(DataPersistenceManager.Instance.profileID);

            UnloadSaveSlots();
            LoadSaveSlots();
            CheckSaveSlotCount();
        }

        private void CheckSaveSlotCount()
        {
            if (_saveSlots == null || _saveSlots.Count == 0)
            {
                noSaveSlotsHint.text = "No saved games found";
            }
            else
            {
                noSaveSlotsHint.text = "";
            }
        }
    }
}