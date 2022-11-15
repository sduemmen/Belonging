using System.Collections.Generic;
using UnityEngine;

namespace Audio
{
    public class AudioController : MonoBehaviour
    {
        public static AudioController Instance { get; private set; }

        [SerializeField] private List<PlayableAudio> _playableAudios;

        private void Awake()
        {
            if (Instance != null)
            {
                Debug.Log("Found more than one AudioControllers. Destroying latest instance");
                Destroy(this);
                return;
            }
            
            Instance = this;
            DontDestroyOnLoad(this);
        }

        public void PlayAudio(string audioName)
        {
            PlayableAudio playableAudio = _playableAudios.Find(entry => entry.AudioName == audioName);
            playableAudio.PlayAudio();
        }
    }
}