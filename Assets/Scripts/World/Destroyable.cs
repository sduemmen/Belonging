using System;
using System.Collections;
using System.Collections.Generic;
using Audio;
using BuildSystem;
using InventorySystem.Items;
using SaveSystem;
using SaveSystem.Data;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

namespace World
{
    [Serializable]
    public class Destroyable : MonoBehaviour, IDataPersistence
    {
        [SerializeField, TitleGroup("General Settings")] private string _prefabName;
        [SerializeField, TitleGroup("General Settings")] private bool _builtByPlayer;
        [SerializeField, TitleGroup("General Settings")] private List<Collider> _collisionCollider;
        private Vector2Int _chunkPosition;
        private Vector2Int _positionInChunk;
        private bool _isPlaced = true;
        private bool _isSnapped;

        [SerializeField, TitleGroup("Interaction")] private ToolItemObject _requiredTool;
        [SerializeField, TitleGroup("Interaction")] private int _health;
        [SerializeField, TitleGroup("Interaction")] private List<ItemStack> _itemDrops;
        [SerializeField, TitleGroup("Interaction")] private List<GameObject> _objectsToBeDeactivatedOnDestroy;
        [SerializeField, TitleGroup("Interaction")] private ParticleSystem _hitParticles;
        [Space(20)]
        [SerializeField, TitleGroup("Interaction")] private AudioClipCollection _audioClipCollection;
        [SerializeField, TitleGroup("Interaction")] private AudioClip _destructionAudioClip;
        [SerializeField, TitleGroup("Interaction")] private AudioSource _audioSource;
        [Space(20)]
        [SerializeField, TitleGroup("Interaction")] private Outline _outline;
        [Space(20)]
        [SerializeField, TitleGroup("Interaction")] private GameObject _segmentColliders;

        public Vector2Int ChunkPosition {
            get => _chunkPosition;
            set => _chunkPosition = value;
        }

        public Vector2Int PositionInChunk {
            get => _positionInChunk;
            set => _positionInChunk = value;
        }

        public bool IsPlaced {
            get => _isPlaced;
            set => _isPlaced = value;
        }

        public bool IsSnapped {
            get => _isSnapped;
            set => _isSnapped = value;
        }

        public ToolItemObject RequiredTool => _requiredTool;

        public List<ItemStack> ItemDrops {
            get => _itemDrops;
            set => _itemDrops = value;
        }

        public Outline Outline => _outline;
        public GameObject SegmentColliders => _segmentColliders;

        private void Awake()
        {
            _hitParticles.Pause();
        }

        [Button("Initialize References"), PropertyOrder(-1)]
        private void InitializeReferences()
        {
            _objectsToBeDeactivatedOnDestroy.Clear();
            _collisionCollider.Clear();
            
            _prefabName = gameObject.name;
            _hitParticles = GetComponentInChildren<ParticleSystem>();
            _audioSource = GetComponent<AudioSource>();

            if (_audioSource == null)
            {
                _audioSource = this.gameObject.AddComponent<AudioSource>();
            }

            _audioSource.playOnAwake = false;
            _audioSource.spatialBlend = 1f;
            
            _outline = GetComponentInChildren<Outline>();

            foreach (Transform child in transform)
            {
                if (child.name.Contains("Colliders"))
                {
                    _segmentColliders = child.gameObject;
                }
                else
                {
                    if (child.TryGetComponent(out Collider c))
                    {
                        _collisionCollider.Add(c);
                    }
                }
            }
            
            _objectsToBeDeactivatedOnDestroy.Add(transform.GetChild(0).gameObject);
        }

        public void OnClick(Vector3 playerPosition, RaycastHit hitResult)
        {
            // adjust hitParticles to be ejected in the players general direction
            Vector3 position = transform.position;
            Vector3 diff = (playerPosition - position).normalized;
            float randomOffset1 = Random.Range(1f, 2f);
            float randomOffset2 = Random.Range(1f, 2f);

            ParticleSystem.VelocityOverLifetimeModule hitParticlesVelocityOverLifetime = _hitParticles.velocityOverLifetime;
            hitParticlesVelocityOverLifetime.x = new ParticleSystem.MinMaxCurve(diff.x - randomOffset1, diff.x + randomOffset2);
            hitParticlesVelocityOverLifetime.z = new ParticleSystem.MinMaxCurve(diff.z - randomOffset2, diff.z + randomOffset1);

            _hitParticles.transform.position = hitResult.point;
            int particleCount = Random.Range(10, 20);
            _hitParticles.Emit(particleCount);
            
            _health--;
            
            if (_audioClipCollection != null)
            {
                _audioSource.PlayOneShot(_audioClipCollection.GetRandom());
                
                if (_health <= 0)
                {
                    _audioSource.PlayOneShot(_destructionAudioClip);
                }
            }
            
            if (_health <= 0)
            {
                OnHealthDepleted();
            }
        }

        private void OnHealthDepleted()
        {
            
            
            foreach (ItemStack itemDrop in _itemDrops)
            {
                for (int i = 0; i < itemDrop.Amount; i++)
                {
                    MaterialItemObject materialItem = (MaterialItemObject)itemDrop.Item;
                    Instantiate(materialItem.Prefab, transform.position + new Vector3(Random.Range(-.5f, .5f), Random.Range(.2f, .5f), Random.Range(-.5f, .5f)), Quaternion.identity);
                }
            }

            if (_builtByPlayer)
            {
                World.Instance.placedSegments -= 1;
            }
            else
            {
                World.Instance.worldAlterations.AddAlteration(_chunkPosition.x, _chunkPosition.y, _positionInChunk.x, _positionInChunk.y);
            }

            StartCoroutine(DestroyAfterTime());
        }

        private IEnumerator DestroyAfterTime()
        {
            foreach (GameObject obj in _objectsToBeDeactivatedOnDestroy)
            {
                obj.SetActive(false);
            }

            foreach (Collider c in _collisionCollider)
            {
                c.enabled = false;
            }

            if (_segmentColliders != null)
            {
                _segmentColliders.SetActive(false);
            }

            yield return new WaitForSeconds(2);
            Destroy(gameObject);
        }
        
        public void LoadData(GameData data)
        {
            if (_builtByPlayer) Destroy(gameObject);
        }

        public void SaveData(ref GameData data)
        {
            if (!_builtByPlayer || GetComponent<SegmentPreview>() != null) return;
            Transform t = GetComponent<Transform>();
            PersistentDestroyableData persistentData = new PersistentDestroyableData(t.position, t.rotation, _prefabName);
            data.persistentDestroyables.Add(persistentData);
        }
    }
}