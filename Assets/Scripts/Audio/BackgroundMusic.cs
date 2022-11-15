using System.Collections.Generic;
using UnityEngine;

namespace Audio
{
    [RequireComponent(typeof(AudioSource))]
    public class BackgroundMusic : MonoBehaviour
    {
        public static BackgroundMusic Instance { get; private set; }

        [SerializeField] private List<AudioClip> _audioClips;
        private AudioSource _audioSource;
        private int _songIndex;

        private void Awake()
        {
            if (Instance != null)
            {
                Debug.Log("Found more than one BackgroundMusic. Destroying latest instance");
                Destroy(this);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(this);

            _audioSource = GetComponent<AudioSource>();
            _audioSource.clip = _audioClips[_songIndex];
            _audioSource.Play();
            
            InvokeRepeating(nameof(CheckSongEnded), 0f, 1f);
        }

        private void CheckSongEnded()
        {
            if (!_audioSource.isPlaying)
            {
                SkipToNextSong();
            }
        }

        private void SkipToNextSong()
        {
            _audioSource.Stop();
            _songIndex = (_songIndex + 1) % _audioClips.Count;
            _audioSource.clip = _audioClips[_songIndex];
            _audioSource.Play();
        }
    }
}