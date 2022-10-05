using System;
using UnityEngine;
using Random = System.Random;

namespace WorldGeneration
{
    public class Chunk : MonoBehaviour
    {
        [Range(2, 256)] public int resolution;
        [Range(10, 40)] public int size;
        private Mesh _mesh;
        private MeshFilter _meshFilter;
        private MeshRenderer _meshRenderer;

        public int seed;
        public Vector2 chunkPosition;
        public float treeThreshold = .5f;
        public int treeDistance;
        public float stoneThreshold = .7f;
        public int stoneDistance;
        public float persistance = .4f;
        public int roughness = 3;
        public int octaves = 4;

        public GameObject treePrefab;
        public GameObject stonePrefab;

        // private void OnValidate()
        // {
        //     var pos = transform.position;
        //     chunkPosition = new Vector2(pos.x, pos.z) / size;
        //     SpawnObjects();
        // }

        private void Awake()
        {
            _mesh = new Mesh();
            _meshFilter = GetComponent<MeshFilter>();
            _meshFilter.mesh = _mesh;
            _meshRenderer = GetComponent<MeshRenderer>();
            _meshRenderer.sharedMaterial = new Material(Shader.Find("Standard"));
        }

        public void InitializeMesh()
        {
            Vector3[] vertices = new Vector3[resolution * resolution];
            int[] triangles = new int[(resolution - 1) * (resolution - 1) * 6];

            int triIndex = 0;
            for (int y = 0; y < resolution; y++) {
                for (int x = 0; x < resolution; x++) {
                    int index = x + y * resolution;
                    Vector2 percent = new Vector2(x, y) / (resolution - 1);
                    Vector3 v = new Vector3(percent.y, 0f, percent.x) * size;
                    vertices[index] = v;

                    if (x != resolution - 1 && y != resolution - 1) {
                        triangles[triIndex] = index;
                        triangles[triIndex + 1] = index + resolution + 1;
                        triangles[triIndex + 2] = index + resolution;
                        triangles[triIndex + 3] = index;
                        triangles[triIndex + 4] = index + 1;
                        triangles[triIndex + 5] = index + resolution + 1;
                        triIndex += 6;
                    }
                }
            }

            _mesh.Clear();
            _mesh.vertices = vertices;
            _mesh.triangles = triangles;
            _mesh.RecalculateNormals();
            Debug.Log("done");
        }

        public void SpawnObjects()
        {
            for(int i = transform.childCount - 1; i >= 0; i--)
            {
                DestroyImmediate(transform.GetChild(i).gameObject);
            }

            Random random = new Random(seed);

            for (int y = 0; y < size; y+=3) {
                for (int x = 0; x < size; x+=3) {
                    float sample = CalculateNoise(x, y);
                    if (sample < stoneThreshold) {
                        Vector3 parentPos = this.transform.position;
                        Vector3 localPos = new Vector3(x, 0, y);
                        Vector3 randomOffset = new Vector3((float)random.Next(-stoneDistance, stoneDistance) / 100, 0, (float)random.Next(-stoneDistance, stoneDistance) / 100);
                        Quaternion randomRotation = Quaternion.Euler(new Vector3(0, random.Next(0, 360), 0));
                        
                        GameObject stone = Instantiate(stonePrefab, parentPos + localPos + randomOffset, randomRotation);
                        stone.transform.SetParent(this.transform);
                    } else if (sample > treeThreshold) {
                        Vector3 parentPos = this.transform.position;
                        Vector3 localPos = new Vector3(x, 0, y);
                        Vector3 randomOffset = new Vector3((float)random.Next(-treeDistance, treeDistance) / 100, 0, (float)random.Next(-treeDistance, treeDistance) / 100);
                        Quaternion randomRotation = Quaternion.Euler(new Vector3(0, random.Next(0, 360), 0));
                        
                        GameObject tree = Instantiate(treePrefab, parentPos + localPos + randomOffset, randomRotation);
                        tree.transform.SetParent(this.transform);
                    }
                }
            }
        }

        private float CalculateNoise(int x, int y)
        {
            float xCoord = seed + chunkPosition.x + (float)x / size;
            float yCoord = seed + chunkPosition.y + (float)y / size;
            float noise = 0;
            float frequency = 1;
            float factor = 1;

            for (int i = 0; i < octaves; i++) {
                noise += Mathf.PerlinNoise(xCoord * frequency + i * 0.72354f, yCoord * frequency + i * 0.72354f) * factor;
                factor *= persistance;
                frequency *= roughness;
            }
            
            return noise;
        }
    }
}