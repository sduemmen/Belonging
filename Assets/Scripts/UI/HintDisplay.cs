using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    public class HintDisplay : MonoBehaviour
    {
        private static HintDisplay _instance;
        public static HintDisplay Instance {
            get {
                if (_instance == null)
                {
                    _instance = (HintDisplay) FindObjectOfType(typeof(HintDisplay));
                }

                return _instance;
            }
        }

        public static Color achievementColor = new Color(.9f, .8f, .4f);
        public static Color errorColor = new Color(.8f, .4f, .4f);
        public static Color defaultColor = new Color(.2f, .2f, .2f);
        
        [SerializeField] private Hint _hintPrefab;
        private List<Hint> _activeHints;

        private void Awake()
        {
            _activeHints = new List<Hint>();
            Hint.OnHintLifecycleCompleteDelegate += OnHintLifecycleComplete;
        }

        public void AddHint(string message, Color color, bool stack = false)
        {
            if (_activeHints.Exists(hint => hint.Message.Equals(message)) && !stack) return;

            if (!stack || !_activeHints.Exists(hint => hint.Message.Equals(message)))
            {
                Hint hint = Instantiate(_hintPrefab, transform, false);
                hint.Label.text = message;
                hint.Message = message;
                hint.Label.color = color;
                _activeHints.Add(hint);
            }
            else
            {
                _activeHints.Find(hint => hint.Message.Equals(message)).Stack();
            }
        }

        public void AddHint(string message, bool stack)
        {
            AddHint(message, defaultColor, stack);
        }
        
        public void AddHint(string message)
        {
            AddHint(message, defaultColor);
        }
        
        public void AddErrorHint(string message, bool stack = false)
        {
            AddHint(message, errorColor, stack);
        }

        private void OnHintLifecycleComplete(Hint hint)
        {
            _activeHints.Remove(hint);
            Destroy(hint.gameObject);
        }
    }
}
