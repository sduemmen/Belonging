using System;
using SaveSystem;
using SaveSystem.Data;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI.MainMenu
{
    public class GameLoader : MonoBehaviour
    {
        [SerializeField] private Button _newGameButton;
        [SerializeField] private Button _loadGameButton;
        [SerializeField] private Button _backButton;
        [SerializeField] private Button _startNewGameButton;
        [SerializeField] private Button _deleteSelectedGameButton;
        [SerializeField] private Button _loadSelectedGameButton;
        [SerializeField] private NewGameData _newGameData;
        
        public void OnNewGame()
        {
            DisableButtons();

            GameData gameData = new GameData(_newGameData);
            
            DataPersistenceManager.instance.profileID = gameData.profileID;
            DataPersistenceManager.instance.NewGame(gameData);
            
            SceneManager.LoadSceneAsync("GameScene");
        }

        public void OnLoadGame()
        {
            if (DataPersistenceManager.instance.noProfileSelected) return;
            
            DisableButtons();
            SceneManager.LoadSceneAsync("GameScene");
        }

        private void DisableButtons()
        {
            _newGameButton.interactable = false;
            _loadGameButton.interactable = false;
            _backButton.interactable = false;
            _startNewGameButton.interactable = false;
            _deleteSelectedGameButton.interactable = false;
            _loadSelectedGameButton.interactable = false;
        }
    }
}
