using System;
using System.Collections;
using System.Collections.Generic;
using BuildSystem;
using InventorySystem.Items;
using SaveSystem;
using SaveSystem.Data;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Environment
{
    [Serializable]
    public class Destroyable : MonoBehaviour, IDataPersistence
    {
        [SerializeField] private string prefabName;
        [SerializeField] private int health;
        [SerializeField] private bool wasBuiltByPlayer;
        [SerializeField] public List<GameObject> objectsToBeDeactivatedOnDestroy;
        
        public List<BuildCost> itemDrops;
        public ToolItemObject requiredTool;
        public GameObject colliders;
        public World world;
        public Vector2Int chunkPosition;
        public Vector2Int positionInChunk;
        public bool isPlaced = true;
        public bool isSnapped;

        private ParticleSystem hitParticles;

        private void Awake()
        {
            hitParticles = GetComponentInChildren<ParticleSystem>();
            hitParticles.Pause();
        }

        [Button("Set Prefab Name")]
        private void SetPrefabName()
        {
            prefabName = this.gameObject.name;
        }

        public void OnClick(Transform player, RaycastHit hitResult)
        {
            // adjust hitParticles to be ejected in the players general direction
            var position = transform.position;
            Vector3 diff = (player.position - position).normalized;
            float randomOffset1 = Random.Range(1f, 2f);
            float randomOffset2 = Random.Range(1f, 2f);
            
            var hitParticlesVelocityOverLifetime = hitParticles.velocityOverLifetime;
            hitParticlesVelocityOverLifetime.x = new ParticleSystem.MinMaxCurve(diff.x - randomOffset1, diff.x + randomOffset2);
            hitParticlesVelocityOverLifetime.z = new ParticleSystem.MinMaxCurve(diff.z - randomOffset2, diff.z + randomOffset1);

            if (this.wasBuiltByPlayer) {
                Vector3 hitPosition = hitResult.point - position;
                ParticleSystem.ShapeModule shape = hitParticles.shape;
                // shape.position = transform.rotation * hitPosition;
                shape.position = hitPosition;
            }

            hitParticles.Emit(20);
            health--;
            if (health <= 0) OnHealthDepleted();
        }

        private void OnHealthDepleted()
        {
            foreach (BuildCost itemDrop in itemDrops) {
                for (int i = 0; i < itemDrop.amount; i++) {
                    MaterialItemObject materialItem = (MaterialItemObject)itemDrop.item;
                    Instantiate(materialItem.prefab, this.transform.position + new Vector3(Random.Range(-.5f, .5f), Random.Range(.2f, .5f), Random.Range(-.5f, .5f)), Quaternion.identity);
                }
            }

            if (!wasBuiltByPlayer)
                world.worldAlterations.AddAlteration(chunkPosition.x, chunkPosition.y, positionInChunk.x, positionInChunk.y);
            else 
                world.placedSegments -= 1;

            StartCoroutine(DestroyAfterTime());
        }

        private IEnumerator DestroyAfterTime()
        {
            foreach (GameObject obj in objectsToBeDeactivatedOnDestroy) {
                obj.SetActive(false);
            }

            this.transform.GetComponent<Collider>().enabled = false;
            
            yield return new WaitForSeconds(2);
            Destroy(this.gameObject);
        }

        public void LoadData(GameData data)
        {
            if (wasBuiltByPlayer) Destroy(this.gameObject);
        }

        public void SaveData(ref GameData data)
        {
            if (!wasBuiltByPlayer || GetComponent<SegmentPreview>() != null) return;
            Transform t = GetComponent<Transform>();
            PersistentDestroyableData persistentData = new PersistentDestroyableData(t.position, t.rotation, prefabName);
            data.persistentDestroyables.Add(persistentData);
        }
    }
}