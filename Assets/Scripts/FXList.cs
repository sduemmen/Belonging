using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[CreateAssetMenu(menuName = "FX List"), Serializable]
public class FXList : ScriptableObject
{
    [SerializeField] private List<FXAudioClipCollection> m_audioClipCollections = new List<FXAudioClipCollection>();
    
    [SerializeField] private List<FXParticleSystem> m_particles = new List<FXParticleSystem>();


    public void SetParticleSystemLookDirection(Vector3 direction)
    {
        foreach (FXParticleSystem fxParticleSystem in m_particles)
        {
            float randomOffset1 = Random.Range(1f, 2f);
            float randomOffset2 = Random.Range(1f, 2f);

            ParticleSystem.VelocityOverLifetimeModule hitParticlesVelocityOverLifetime = fxParticleSystem.m_particles.velocityOverLifetime;
            hitParticlesVelocityOverLifetime.x = new ParticleSystem.MinMaxCurve(direction.x - randomOffset1, direction.x + randomOffset2);
            hitParticlesVelocityOverLifetime.z = new ParticleSystem.MinMaxCurve(direction.z - randomOffset2, direction.z + randomOffset1);
        }
    }

    public void PlayFX(Vector3 position)
    {
        foreach (FXAudioClipCollection clipCollection in m_audioClipCollections)
        {
            GameObject audioObject = new GameObject("AudioObject");
            audioObject.transform.position = position;
            
            AudioSource audioSource = audioObject.AddComponent<AudioSource>();
            audioSource.Stop();
            audioSource.playOnAwake = false;
            audioSource.loop = false;
            audioSource.clip = clipCollection.m_audioClipCollection.Get(clipCollection.m_clipSelectionMode);
            audioSource.volume = clipCollection.m_volume;
            audioSource.spatialBlend = 1;
            audioSource.Play();
            
            Destroy(audioSource.gameObject, 4);
        }

        foreach (FXParticleSystem particleSystem in m_particles)
        {
            FXParticleSystem instance = Instantiate(particleSystem, position, Quaternion.identity);
            instance.PlayFX();
        }
    }
}