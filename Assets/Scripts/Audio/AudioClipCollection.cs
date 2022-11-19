using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Audio
{
    [Serializable, CreateAssetMenu(menuName = "Audioclip Collection")]
    public class AudioClipCollection : ScriptableObject
    {
        [SerializeField] private string _name;
        
        [SerializeField] private List<AudioClip> _audioClips;

        public string Name => _name;
        
        private AudioClip GetRandom()
        {
            int randomIndex = Random.Range(0, _audioClips.Count);
            return _audioClips[randomIndex];
        }
        
        private AudioClip GetFirst()
        {
            return _audioClips[0];
        }
        
        private AudioClip GetLast()
        {
            return _audioClips[^1];
        }

        public AudioClip Get(AudioClipSelectionMode selectionMode)
        {
            switch (selectionMode)
            {
                case AudioClipSelectionMode.First:
                    return GetFirst();
                case AudioClipSelectionMode.Last:
                    return GetLast();
                case AudioClipSelectionMode.Random:
                    return GetRandom();
                default:
                    return null;
            }
        }
    }
}