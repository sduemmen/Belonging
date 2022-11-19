using System;
using Audio;
using UnityEngine;

[Serializable]
public class FXAudioClipCollection
{
    public AudioClipCollection m_audioClipCollection;

    public AudioClipSelectionMode m_clipSelectionMode;
    
    public float m_volume;
    

    public void PlayFX(Vector3 position)
    {
        AudioSource.PlayClipAtPoint(m_audioClipCollection.Get(m_clipSelectionMode), position, m_volume);
    }
}