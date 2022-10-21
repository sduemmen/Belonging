using System;
using UnityEngine;
using Random = System.Random;

namespace Environment
{
    public class Chunk : MonoBehaviour
    {
        public World world;
        public Vector2Int chunkPosition;
        
        public GameObject treePrefab;
        public GameObject stonePrefab;

        public void SpawnObjects()
        {
            // for (int i = transform.childCount - 1; i >= 0; i--)
            // {
            //     DestroyImmediate(transform.GetChild(i).gameObject);
            // }

            int chunkEncoding = chunkPosition.x << 16 | chunkPosition.y;
            Random randomPositioner = new Random(world.seed + chunkEncoding);
            Random random = new Random(world.seed);
            float xOffset = random.Next(-10000, 10000);
            float yOffset = random.Next(-10000, 10000);

            for (int y = 0; y < World.CHUNK_SIZE; y+=3) {
                for (int x = 0; x < World.CHUNK_SIZE; x+=3) {
                    if (world.worldAlterations.HasAlteration(chunkPosition.x, chunkPosition.y, x, y)) continue;
                
                    float treeSample = World.SamplePerlin2d(x, y, chunkPosition, xOffset, yOffset);
                    float stoneSample = World.SamplePerlin2d(x + 1000, y + 1000, chunkPosition, xOffset, yOffset);
                
                    if (treeSample < world.treeDensityThreshold && stoneSample < world.stoneDensityThreshold) {
                        bool decider = Convert.ToBoolean(random.Next(0, 2));
                        GameObject obj = decider ? stonePrefab : treePrefab;
                        InstantiatePrefabRandomized(obj, new Vector3(x, 0, y), randomPositioner);
                        continue;
                    }
                
                    if (treeSample < world.treeDensityThreshold) {
                        InstantiatePrefabRandomized(treePrefab, new Vector3(x, 0, y), randomPositioner);
                    } else if (stoneSample < world.stoneDensityThreshold) {
                        InstantiatePrefabRandomized(stonePrefab, new Vector3(x, 0, y), randomPositioner);
                    }
                }
            }
        }

        private void InstantiatePrefabRandomized(GameObject prefab, Vector3 localPosition, Random random)
        {
            Vector3 parentPos = this.transform.position;
            
            Vector3 randomOffset = new Vector3(
                (float)random.Next(-world.objectDistance, world.objectDistance) / 100, 
                0, 
                (float)random.Next(-world.objectDistance, world.objectDistance) / 100);
                        
            Quaternion randomRotation = Quaternion.Euler(new Vector3(0, random.Next(0, 360), 0));
                        
            GameObject obj = Instantiate(prefab, parentPos + localPosition + randomOffset, randomRotation);
            obj.transform.SetParent(this.transform);
            Destroyable destroyable = obj.GetComponentInChildren<Destroyable>();
            destroyable.world = this.world;
            destroyable.chunkPosition = this.chunkPosition;
            destroyable.positionInChunk = new Vector2Int((int)localPosition.x, (int)localPosition.z);
        } 
    }
}