using TMPro;
using UnityEngine;

namespace QuestSystem
{
    public class UIQuest : MonoBehaviour
    {
        public TextMeshProUGUI questTitleLabel;
        public TextMeshProUGUI questDescriptionLabel;
        public TextMeshProUGUI questProgressLabel;

        public void SetProgress(float current, float target)
        {
            questProgressLabel.text = current + "/" + target;
        }

        public void OnComplete()
        {
            Destroy(gameObject);
        }
    }
}