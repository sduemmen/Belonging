using InventorySystem.Items;
using SaveSystem;
using SaveSystem.Data;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Environment
{
    public class Destroyable : MonoBehaviour, IDataPersistence
    {
        public ToolItemObject requiredTool;
        public string prefabName;
        public bool wasBuiltByPlayer;
        public bool isPlaced = true;
        public bool isSnapped;
        public MaterialItemObject dropItem;
        public int dropQuantity;
        public int health;
        public World world;
        public Vector2Int chunkPosition;
        public Vector2Int positionInChunk;
        public ParticleSystem hitParticles;

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
                shape.position = transform.rotation * hitPosition;
            }
            
            hitParticles.Emit(20);
            health--;
            if (health <= 0) OnHealthDepleted();
        }

        private void OnHealthDepleted()
        {
            // ParticleSystem p = Instantiate(new ParticleSystem(), this.transform.position, Quaternion.identity);
            // p = hitParticles;
            // p.AddComponent<DestroyAfterTime>();
            
            for (int i = 0; i < dropQuantity; i++) {
                Instantiate(dropItem.prefab, this.transform.position + new Vector3(Random.Range(-.5f, .5f), Random.Range(.2f, .5f), Random.Range(-.5f, .5f)), Quaternion.identity);
            }

            if (!wasBuiltByPlayer)
                world.worldAlterations.AddAlteration(chunkPosition.x, chunkPosition.y, positionInChunk.x, positionInChunk.y);
            else 
                world.placedSegments -= 1;
            Destroy(this.gameObject);
        }

        public void LoadData(GameData data)
        {
            if (wasBuiltByPlayer) Destroy(this.gameObject);
        }

        public void SaveData(ref GameData data)
        {
            if (!wasBuiltByPlayer) return;
            Transform t = GetComponent<Transform>();
            PersistentDestroyableData persistentData = new PersistentDestroyableData(t.position, t.rotation, prefabName);
            data.persistentDestroyables.Add(persistentData);
        }
    }
}