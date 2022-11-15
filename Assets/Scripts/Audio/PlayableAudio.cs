using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Audio
{
    public enum AudioClipSelectionMode
    {
        Single,
        Random,
        First,
        Last,
        Index,
    }
    
    [RequireComponent(typeof(AudioSource))]
    [Serializable]
    public class PlayableAudio : MonoBehaviour
    {
        [SerializeField] private string _audioName;
        [SerializeField] private bool _playOneShot;
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private AudioClipSelectionMode _audioClipSelectionMode;
        [SerializeField, ShowIf("@_audioClipSelectionMode == AudioClipSelectionMode.Single")] private AudioClip _audioClip;
        [SerializeField, ShowIf("@_audioClipSelectionMode != AudioClipSelectionMode.Single")] private AudioClipCollection _audioClipCollection;
        [SerializeField, ShowIf("@_audioClipSelectionMode == AudioClipSelectionMode.Index")] private int _audioClipIndex;

        public string AudioName => _audioName;
        
        public void PlayAudio()
        {
            AudioClip clip = _audioClipSelectionMode == AudioClipSelectionMode.Single ? _audioClip : _audioClipCollection.Get(_audioClipSelectionMode, _audioClipIndex);
            
            if (_playOneShot)
            {
                _audioSource.PlayOneShot(clip);
            }
            else
            {
                _audioSource.clip = clip;
                _audioSource.Play();
            }
        }
    }
}