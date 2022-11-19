using System;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(ParticleSystem)), Serializable]
public class FXParticleSystem : MonoBehaviour
{
    public ParticleSystem m_particles;
    
    public int m_particleCount = 20;

    public bool m_randomParticleCount;

    public int m_minParticleCount;

    public int m_maxParticleCount;
    
    public bool m_emit = true;

    public bool m_selfDestroyAfterPlaying = true;

    public float m_selfDestroyDelay = 3f;
    

    private void Awake()
    {
        m_particles = GetComponent<ParticleSystem>();
        m_particles.Stop();
    }

    public void PlayFX()
    {
        if (m_emit)
        {
            int particleCount = m_randomParticleCount ? Random.Range(m_minParticleCount, m_maxParticleCount + 1) : m_particleCount;
            
            m_particles.Emit(particleCount);
        }
        else
        {
            m_particles.Play();
        }
        
        if (m_selfDestroyAfterPlaying)
        {
            Destroy(this.gameObject, m_selfDestroyDelay);
        }
    }
}