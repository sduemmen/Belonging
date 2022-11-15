using System;
using UnityEngine;
using Random = System.Random;

namespace World
{
    public class Chunk : MonoBehaviour
    {
        public Vector2Int chunkPosition;

        [SerializeField] private SpawnablePrefabCollection _treePrefabs;
        [SerializeField] private SpawnablePrefabCollection _stonePrefabs;

        public void SpawnObjects()
        {
            // for (int i = transform.childCount - 1; i >= 0; i--)
            // {
            //     DestroyImmediate(transform.GetChild(i).gameObject);
            // }

            int chunkEncoding = (chunkPosition.x << 16) | chunkPosition.y;
            Random randomPositioner = new Random(World.Instance.seed + chunkEncoding);
            Random random = new Random(World.Instance.seed);
            float xOffset = random.Next(-10000, 10000);
            float yOffset = random.Next(-10000, 10000);

            for (int y = 0; y < World.CHUNK_SIZE; y += 3)
            for (int x = 0; x < World.CHUNK_SIZE; x += 3)
            {
                if (World.Instance.worldAlterations.HasAlteration(chunkPosition.x, chunkPosition.y, x, y)) continue;

                float treeSample = WorldGenerator.SamplePerlin2d(x, y, chunkPosition, xOffset, yOffset);
                float stoneSample = WorldGenerator.SamplePerlin2d(x + 1000, y + 1000, chunkPosition, xOffset, yOffset);

                if (treeSample < World.Instance.treeDensityThreshold && stoneSample < World.Instance.stoneDensityThreshold)
                {
                    bool decider = Convert.ToBoolean(random.Next(0, 2));
                    GameObject obj = decider ? _stonePrefabs.GetRandom(randomPositioner) : _treePrefabs.GetRandom(randomPositioner);
                    InstantiatePrefabRandomized(obj, new Vector3(x, 0, y), randomPositioner);
                    continue;
                }

                if (treeSample < World.Instance.treeDensityThreshold)
                {
                    InstantiatePrefabRandomized(_treePrefabs.GetRandom(randomPositioner), new Vector3(x, 0, y), randomPositioner);
                }
                else if (stoneSample < World.Instance.stoneDensityThreshold)
                {
                    InstantiatePrefabRandomized(_stonePrefabs.GetRandom(randomPositioner), new Vector3(x, 0, y), randomPositioner);
                }
            }
        }

        private void InstantiatePrefabRandomized(GameObject prefab, Vector3 localPosition, Random random)
        {
            Vector3 parentPos = transform.position;

            Vector3 randomOffset = new Vector3(
                (float)random.Next(-World.Instance.objectDistance, World.Instance.objectDistance) / 100,
                0,
                (float)random.Next(-World.Instance.objectDistance, World.Instance.objectDistance) / 100);

            Quaternion randomRotation = Quaternion.Euler(new Vector3(0, random.Next(0, 360), 0));

            GameObject obj = Instantiate(prefab, parentPos + localPosition + randomOffset, randomRotation);
            obj.transform.SetParent(transform);
            Destroyable destroyable = obj.GetComponentInChildren<Destroyable>();
            destroyable.ChunkPosition = chunkPosition;
            destroyable.PositionInChunk = new Vector2Int((int)localPosition.x, (int)localPosition.z);
        }
    }
}