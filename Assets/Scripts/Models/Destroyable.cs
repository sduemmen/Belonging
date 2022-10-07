using Items;
using UnityEngine;
using WorldGeneration;

namespace Models
{
    public class Destroyable : MonoBehaviour
    {
        public ItemType requiredTool;
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

        public void OnClick()
        {
            hitParticles.Emit(20);
            health--;
            if (health <= 0) OnHealthDepleted();
        }

        private void OnHealthDepleted()
        {
            for (int i = 0; i < dropQuantity; i++) {
                Instantiate(dropItem.GetPrefab(), this.transform.position + new Vector3(Random.Range(-.2f, .2f), .2f, Random.Range(-.2f, .2f)), Quaternion.Euler(Vector3.zero));
            }
            Destroy(this.gameObject);
        }
        
        private void OnDestroy()
        {
            world.worldAlterations.AddAlteration(chunkPosition.x, chunkPosition.y, positionInChunk.x, positionInChunk.y);
        }
    }
}