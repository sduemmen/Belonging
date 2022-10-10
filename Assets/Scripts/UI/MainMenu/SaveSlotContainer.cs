using System;
using System.Collections.Generic;
using System.Linq;
using Flags;
using SaveSystem;
using SaveSystem.Data;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI.MainMenu
{
    public class SaveSlotContainer : MonoBehaviour
    {
        private List<SaveSlot> _saveSlots;
        [SerializeField] private GameObject saveSlotPrefab;
        [SerializeField] private TextMeshProUGUI noSaveSlotsHint;
        private static SaveSlot selectedSaveSlot;

        public static SaveSlot SelectedSaveSlot {
            get => selectedSaveSlot;
            set {
                bool selectedSaveSlotIsNull = selectedSaveSlot == null;
                bool newSaveSlotIsNull = value == null;
                
                if (newSaveSlotIsNull) {
                    DataPersistenceManager.instance.profileID = "default";
                    if (!selectedSaveSlotIsNull) selectedSaveSlot.uiSaveSlot.OnUnselect();
                } else {
                    DataPersistenceManager.instance.profileID = value.gameData.profileID;
                    if (!selectedSaveSlotIsNull) selectedSaveSlot.uiSaveSlot.OnUnselect();
                    value.uiSaveSlot.OnSelect();
                }
                selectedSaveSlot = value;
            }
        }

        private void Awake()
        {
            _saveSlots = new List<SaveSlot>();
        }

        private void Update()
        {
            if (selectedSaveSlot != null && UserInputFlags.LEFT_MOUSE_BUTTON_WAS_PRESSED) {
                selectedSaveSlot.uiSaveSlot.OnUnselect();
            }
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
                    
                    EventTrigger onPointerDown = saveSlot.uiSaveSlot.gameObject.AddComponent<EventTrigger>();
                    EventTrigger.Entry pointerDown = new EventTrigger.Entry {
                        eventID = EventTriggerType.PointerClick
                    };
                    pointerDown.callback.AddListener((e) => {
                        Debug.Log("trigger test");
                        SelectedSaveSlot = saveSlot;
                    });
                    onPointerDown.triggers.Add(pointerDown);
                    
                    _saveSlots.Add(saveSlot);
                }
            }

            CheckSaveSlotCount();
        }

        public void UnloadSaveSlots()
        {
            SelectedSaveSlot = null;
            
            if (_saveSlots == null) return;
            
            foreach (SaveSlot saveSlot in _saveSlots.ToArray()) {
                _saveSlots.Remove(saveSlot);
                saveSlot.uiSaveSlot.selectButton.onClick.RemoveAllListeners();
                Destroy(saveSlot.uiSaveSlot.gameObject);
            }
        }

        public void DeleteSelectedSaveSlot()
        {
            if (SelectedSaveSlot == null) return;

            SaveLoadIO saveLoadIO = new SaveLoadIO(Application.persistentDataPath);
            saveLoadIO.Delete(selectedSaveSlot.gameData.profileID);
            
            _saveSlots.Remove(selectedSaveSlot);
            Destroy(selectedSaveSlot.uiSaveSlot.gameObject);
            
            SelectedSaveSlot = null;
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
