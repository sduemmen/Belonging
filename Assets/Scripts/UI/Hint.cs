using System;
using TMPro;
using UnityEngine;

namespace UI
{
    public class Hint : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _label;
        [SerializeField] private string _message;
        [SerializeField] private float _fadeInSpeed = .1f;
        [SerializeField] private float _fadeOutSpeed = .1f;
        private bool _fadeIn;
        private bool _fadeOut;
        private int _stackCount;    // allows for stacking of hints of the same type when collecting multiple items of the same kind

        public TextMeshProUGUI Label => _label;
        public string Message {
            get => _message;
            set => _message = value;
        }
        
        public static Action<Hint> OnHintLifecycleCompleteDelegate;

        private void Awake()
        {
            _label.alpha = 0;
            _fadeIn = true;
            _stackCount = 1;
        }

        private void Update()
        {
            if (_fadeIn)
            {
                _label.alpha = Mathf.Lerp(_label.alpha, 1, _fadeInSpeed);
                if (_label.alpha > .95f)
                {
                    _label.alpha = 1;
                    _fadeIn = false;
                    Invoke(nameof(FadeOut), 3);
                }
            }
            else if (_fadeOut)
            {
                _label.alpha = Mathf.Lerp(_label.alpha, 0, _fadeOutSpeed);
                if (_label.alpha < .05f)
                {
                    _label.alpha = 0;
                    _fadeOut = false;
                    OnHintLifecycleCompleteDelegate?.Invoke(this);
                }
            }
        }

        private void FadeOut()
        {
            _fadeOut = true;
        }

        public void Stack()
        {
            _stackCount++;
            _label.text = $"{_message} {_stackCount}x";
            
            if (_fadeIn)
            {
                _label.alpha = 0;
            }
            else if (_fadeOut)
            {
                _fadeOut = false;
                _label.alpha = 1;
                CancelInvoke();
                Invoke(nameof(FadeOut), 3);
            }
            else if (!_fadeIn && !_fadeOut)
            {
                CancelInvoke();
                Invoke(nameof(FadeOut), 3);
            }
        }
    }
}