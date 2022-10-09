using System;
using UnityEngine;

namespace UI.MainMenu
{
    public class ViewController : MonoBehaviour
    {
        public GameObject mainMenuView;
        public GameObject optionsView;
        public GameObject playView;
        public GameObject quitGameView;

        public GameObject newGamePanel;
        public GameObject loadGamePanel;

        private void Awake()
        {
            LoadMainMenuView();
        }

        public void LoadMainMenuView()
        {
            LeanTween.Framework.LeanTween.moveLocalX(mainMenuView, 0, .4f).setEaseInOutCubic();
            LeanTween.Framework.LeanTween.moveLocalX(optionsView, -Screen.currentResolution.width, .4f).setEaseInOutCubic();
            LeanTween.Framework.LeanTween.moveLocalX(playView, Screen.currentResolution.width, .4f).setEaseInOutCubic();
        }

        public void LoadOptionsView()
        {
            LeanTween.Framework.LeanTween.moveLocalX(mainMenuView, Screen.currentResolution.width, .4f).setEaseInOutCubic();
            LeanTween.Framework.LeanTween.moveLocalX(optionsView, 0, .4f).setEaseInOutCubic();
            LeanTween.Framework.LeanTween.moveLocalX(playView, Screen.currentResolution.width * 2, .4f).setEaseInOutCubic();
        }

        public void LoadPlayView()
        {
            LeanTween.Framework.LeanTween.moveLocalX(mainMenuView, -Screen.currentResolution.width * 2, .4f).setEaseInOutCubic();
            LeanTween.Framework.LeanTween.moveLocalX(optionsView, -Screen.currentResolution.width, .4f).setEaseInOutCubic();
            LeanTween.Framework.LeanTween.moveLocalX(playView, 0, .4f).setEaseInOutCubic();
        }

        public void ShowQuitGamePrompt()
        {
            quitGameView.SetActive(true);
        }

        public void HideQuitGamePrompt()
        {
            quitGameView.SetActive(false);
        }

        public void ShowNewGamePanel()
        {
            newGamePanel.SetActive(true);
        }

        public void HideNewGamePanel()
        {
            newGamePanel.SetActive(false);
        }

        public void ShowLoadGamePanel()
        {
            loadGamePanel.SetActive(true);
        }

        public void HideLoadGamePanel()
        {
            loadGamePanel.SetActive(false);
        }
    }
}
