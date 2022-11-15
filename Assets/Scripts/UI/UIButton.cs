using System.Collections.Generic;
using Audio;
using Sirenix.OdinInspector;
using TMPro;
using UI.MainMenu;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI
{
    [RequireComponent(typeof(EventTrigger))]
    public class UIButton : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [SerializeField] private TextMeshProUGUI _buttonText;
        [SerializeField] private UIButtonHoverEffect _hoverEffect;

        [Button("Initialize References")]
        private void InitializeReferences()
        {
            _button = GetComponent<Button>();
            _buttonText = GetComponentInChildren<TextMeshProUGUI>();
            _hoverEffect = GetComponentInChildren<UIButtonHoverEffect>();
        }

        private void Awake()
        {
            float defaultFontsize = _buttonText.fontSize;
            float scaledFontsize = defaultFontsize * 1.1f;
            
            EventTrigger.Entry pointerEnter = new EventTrigger.Entry();
            pointerEnter.eventID = EventTriggerType.PointerEnter;
            pointerEnter.callback.AddListener(_ => {
                AudioController.Instance.PlayAudio("UIHoverSound");
                _hoverEffect.Enable();
                _buttonText.fontSize = scaledFontsize;
            });

            EventTrigger.Entry pointerExit = new EventTrigger.Entry();
            pointerExit.eventID = EventTriggerType.PointerExit;
            pointerExit.callback.AddListener(_ => {
                _hoverEffect.Disable();
                _buttonText.fontSize = defaultFontsize;
            });
            
            EventTrigger trigger = GetComponent<EventTrigger>();
            trigger.triggers.AddRange(new List<EventTrigger.Entry>{pointerEnter, pointerExit});

            _button.onClick.AddListener(() => {
                AudioController.Instance.PlayAudio("UIClickSound");
                _hoverEffect.Disable();
                _buttonText.fontSize = defaultFontsize;
            });
            
            _hoverEffect.Disable();
        }
    }
}