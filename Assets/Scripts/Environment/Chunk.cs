using System;
using System.Threading.Tasks;
using UnityEngine;
using Random = System.Random;

namespace Environment
{
    public class Chunk : MonoBehaviour
    {
        [Range(10, 40)] public int size;

        public World world;
        public Vector2Int chunkPosition;
        
        public float persistance = .4f;
        public int roughness = 3;
        public int octaves = 3;

        public GameObject treePrefab;
        public GameObject stonePrefab;

        public void SpawnObjects()
        {
            // for (int i = transform.childCount - 1; i >= 0; i--)
            // {
            //     DestroyImmediate(transform.GetChild(i).gameObject);
            // }

            int chunkEncoding = chunkPosition.x << 16 | chunkPosition.y;
            Random random = new Random(world.seed + chunkEncoding);

            for (int y = 0; y < size; y+=3) {
                for (int x = 0; x < size; x+=3) {
                    if (world.worldAlterations.HasAlteration(chunkPosition.x, chunkPosition.y, x, y)) continue;
                
                    float seededX = x + world.seedOffset;
                    float seededY = y + world.seedOffset;
                
                    float treeSample = CalculateNoise(seededX, seededY);
                    float stoneSample = CalculateNoise(seededX + 50f, seededY + 50f);
                
                    if (treeSample > world.treeThreshold && stoneSample < world.stoneThreshold) {
                        bool decider = Convert.ToBoolean(random.Next(0, 2));
                        GameObject obj = decider ? stonePrefab : treePrefab;
                        InstantiatePrefabRandomized(obj, new Vector3(x, 0, y), random);
                        continue;
                    }
                
                    if (treeSample > world.treeThreshold) {
                        InstantiatePrefabRandomized(treePrefab, new Vector3(x, 0, y), random);
                    } else if (stoneSample < world.stoneThreshold) {
                        InstantiatePrefabRandomized(stonePrefab, new Vector3(x, 0, y), random);
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
            Destroyable destroyable = obj.GetComponent<Destroyable>();
            destroyable.world = this.world;
            destroyable.chunkPosition = this.chunkPosition;
            destroyable.positionInChunk = new Vector2Int((int)localPosition.x, (int)localPosition.z);
        } 

        private float CalculateNoise(float x, float y)
        {
            float xCoord = chunkPosition.x + x / size;
            float yCoord = chunkPosition.y + y / size;
            
            float noise = 0;
            float frequency = 1;
            float factor = 1;

            for (int i = 0; i < octaves; i++) {
                noise += Mathf.PerlinNoise(xCoord * frequency + i, yCoord * frequency + i) * factor;
                factor *= persistance;
                frequency *= roughness;
            }
                
            return noise - .25f;
        }
    }
}