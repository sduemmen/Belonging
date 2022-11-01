using System;
using System.Linq;
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
        public Button selectSaveSlotButton;
        public Image background;
        public Color selectColor = new(46, 46, 46);
        public Color defaultColor = new(30, 30, 30);

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
            score.text = $"{count} {s}";
        }

        public void SetUnlockedSegments(int unlockedSegments, int total)
        {
            unlocked.text = $"{unlockedSegments}/{total} Segments";
        }

        public void OnMouseHoverEnter()
        {
            background.color = selectColor;
        }

        public void OnMouseHoverLeave()
        {
            background.color = defaultColor;
        }

        public void SetValues(GameData gameData)
        {
            SetDisplayName(gameData.name);
            SetPlaytime(gameData.playtime);
            SetLastPlayedOn(GetTimeUntilNow(DateTime.FromFileTime(gameData.lastPlayed)));
            SetPlacedSegmentCount(gameData.placedSegments);
            SetUnlockedSegments(gameData.segmentUnlockData.Where(data => data.unlocked).ToList().Count, gameData.segmentUnlockData.Count);
        }

        private string GetTimeUntilNow(DateTime dateTime)
        {
            TimeSpan timeSpan = TimeSpan.FromSeconds((DateTime.Now - dateTime).TotalSeconds);
            if (timeSpan.Days > 0)
            {
                string days = timeSpan.Days > 1 ? "Days" : "Day";
                return $"{timeSpan.Days} {days}";
            }

            if (timeSpan.Hours > 0)
            {
                string hours = timeSpan.Hours > 1 ? "Hours" : "Hour";
                return $"{timeSpan.Hours} {hours}";
            }

            if (timeSpan.Minutes > 0)
            {
                string minutes = timeSpan.Minutes > 1 ? "Minutes" : "Minute";
                return $"{timeSpan.Minutes} {minutes}";
            }

            return $"{timeSpan:%s} Seconds";
        }
    }
}