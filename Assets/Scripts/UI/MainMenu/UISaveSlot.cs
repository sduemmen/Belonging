using System;
using SaveSystem.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.MainMenu
{
    public class UISaveSlot : MonoBehaviour
    {
        public TextMeshProUGUI displayName;
        public TextMeshProUGUI playtime;
        public TextMeshProUGUI score;
        public TextMeshProUGUI unlocked;
        public Button selectButton;
        public Image background;
        public Color selectColor = new Color(46, 46, 46);
        public Color defaultColor = new Color(30, 30, 30);
        
        private void Awake()
        {
            selectButton = GetComponent<Button>();
        }

        public void SetDisplayName(string text)
        {
            displayName.text = text;
        }
        
        public void SetPlaytime(float time)
        {
            time /= 3600;
            playtime.text = time.ToString("F1") + " h";
        }
        
        public void SetScore(int score)
        {
            string s = score == 1 ? " Segment" : " Segments";
            this.score.text = $"{score} {s}";
        }
        
        public void SetUnlocked(int unlocked, int total)
        {
            this.unlocked.text = $"{unlocked}/{total} Segments";
        }

        public void OnSelect()
        {
            background.color = selectColor;
        }

        public void OnUnselect()
        {
            background.color = defaultColor;
        }

        public void SetValues(GameData gameData)
        {
            SetDisplayName(gameData.name);
            SetPlaytime(gameData.playtime);
            SetScore(gameData.score);
            SetUnlocked(gameData.unlocked, GameParameters.MAX_UNLOCKABLE_SEGMENTS);
        }
    }
}
