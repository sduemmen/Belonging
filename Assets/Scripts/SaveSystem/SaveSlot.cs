using SaveSystem.Data;
using UI.MainMenu;
using UnityEngine;

namespace SaveSystem
{
    public class SaveSlot : MonoBehaviour
    {
        public GameData gameData;
        public UISaveSlot uiSaveSlot;

        public void SetProfileID()
        {
            DataPersistenceManager.instance.profileID = gameData.profileID;
        }
    }
}
