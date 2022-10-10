using Environment;
using Items;
using SaveSystem;
using SaveSystem.Data;
using Unity.VisualScripting;
using UnityEngine;
using Utility;

namespace Models
{
    public enum DestroyableType
    {
        TreeModel,
        StoneModel,
        FloorModel,
        WallModel,
        RoofModel,
    }
    
    public class Destroyable : MonoBehaviour, IDataPersistence
    {
        public ItemType requiredTool;
        public DestroyableType modeltype;
        public bool wasBuiltByPlayer;
        public Item dropItem;
        public int dropQuantity;
        public int health;
        public World world;
        public Vector2Int chunkPosition;
        public Vector2Int positionInChunk;
        public ParticleSystem hitParticles;

        private void Awake()
        {
            hitParticles = GetComponent<ParticleSystem>();
            hitParticles.Pause();
        }

        public void OnClick(Transform player)
        {
            // adjust hitParticles to be ejected in the players general direction
            Vector3 diff = (player.position - transform.position).normalized;
            float randomOffset1 = Random.Range(1f, 2f);
            float randomOffset2 = Random.Range(1f, 2f);
            
            var hitParticlesVelocityOverLifetime = hitParticles.velocityOverLifetime;
            hitParticlesVelocityOverLifetime.x = new ParticleSystem.MinMaxCurve(diff.x - randomOffset1, diff.x + randomOffset2);
            hitParticlesVelocityOverLifetime.z = new ParticleSystem.MinMaxCurve(diff.z - randomOffset2, diff.z + randomOffset1);
            
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
                Instantiate(dropItem.Prefab, this.transform.position + new Vector3(Random.Range(-.2f, .2f), .2f, Random.Range(-.2f, .2f)), Quaternion.identity);
            }
            if (!wasBuiltByPlayer) world.worldAlterations.AddAlteration(chunkPosition.x, chunkPosition.y, positionInChunk.x, positionInChunk.y);
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
            PersistentDestroyableData persistentData = new PersistentDestroyableData(t.position, t.rotation, modeltype);
            data.persistentDestroyables.Add(persistentData);
        }
    }
}