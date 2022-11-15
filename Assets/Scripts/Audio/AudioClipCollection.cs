using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Audio
{
    [Serializable, CreateAssetMenu(menuName = "Collection/Audioclip")]
    public class AudioClipCollection : ScriptableObject
    {
        [SerializeField] private string _name;
        [SerializeField] private List<AudioClip> _audioClips;

        public string Name => _name;
        
        public AudioClip GetRandom()
        {
            int randomIndex = Random.Range(0, _audioClips.Count);
            return _audioClips[randomIndex];
        }
        
        public AudioClip GetFirst()
        {
            return _audioClips[0];
        }
        
        public AudioClip GetLast()
        {
            return _audioClips[^1];
        }
        
        public AudioClip Get(int index)
        {
            return _audioClips[index];
        }

        public AudioClip Get(AudioClipSelectionMode selectionMode, int index)
        {
            switch (selectionMode)
            {
                case AudioClipSelectionMode.First:
                    return GetFirst();
                case AudioClipSelectionMode.Last:
                    return GetLast();
                case AudioClipSelectionMode.Index:
                    return Get(index);
                case AudioClipSelectionMode.Random:
                    return GetRandom();
                default:
                    return null;
            }
        }
    }
}