using System;
using Flags;
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
        public TextMeshProUGUI lastPlayedOn;
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

        public void SetLastPlayedOn(string lastPlayed)
        {
            lastPlayedOn.text = $"Last played {lastPlayed} ago";
        }
        
        public void SetPlacedSegmentCount(int count)
        {
            string s = count == 1 ? " Segment" : " Segments";
            this.score.text = $"{count} {s}";
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
            SetLastPlayedOn(GetTimeUntilNow(DateTime.FromFileTime(gameData.lastPlayed)));
            SetPlacedSegmentCount(gameData.placedSegments);
            SetUnlocked(gameData.unlocked, GameConstants.MAX_UNLOCKABLE_SEGMENTS);
        }

        private string GetTimeUntilNow(DateTime dateTime)
        {
            TimeSpan timeSpan = TimeSpan.FromSeconds((DateTime.Now - dateTime).TotalSeconds);
            if (timeSpan.Days > 0) {
                string days = timeSpan.Days > 1 ? "Days" : "Day";
                return $"{timeSpan.Days} {days}";
            }
                
            if (timeSpan.Hours > 0) {
                string hours = timeSpan.Hours > 1 ? "Hours" : "Hour";
                return $"{timeSpan.Hours} {hours}";
            }
            
            if (timeSpan.Minutes > 0) {
                string minutes = timeSpan.Minutes > 1 ? "Minutes" : "Minute";
                return $"{timeSpan.Minutes} {minutes}";
            }
            
            return $"{timeSpan:%s} Seconds";
        }
    }
}
