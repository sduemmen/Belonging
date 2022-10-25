using UnityEngine;

namespace Environment
{
    public class InteractiveSnowParticles : MonoBehaviour
    {
        [SerializeField] private ParticleSystem _particleSystem;

        private void Awake()
        {
            _particleSystem.Stop();
        }

        private void Start()
        {
            _particleSystem.Play();
        }
    }
}