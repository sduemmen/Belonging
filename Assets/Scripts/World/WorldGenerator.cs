using UnityEngine;

namespace World
{
    public class WorldGenerator
    {
        /// <summary>
        ///     Sample 2-Dimensional Perlin-Noise from world coordinates
        /// </summary>
        /// <param name="x">x coordinate in world space</param>
        /// <param name="y">y coordinate in world space</param>
        /// <param name="xOffset">offset on x-axis</param>
        /// <param name="yOffset">offset on y-axis</param>
        /// <returns></returns>
        public static float SamplePerlin2d(float x, float y, float xOffset, float yOffset)
        {
            // apply seed (offset) to x and y world coordinates
            float seededX = x + xOffset;
            float seededY = y + yOffset;

            // combine chunk and seed
            float xSampleCoord = seededX / World.CHUNK_SIZE;
            float ySampleCoord = seededY / World.CHUNK_SIZE;

            return CalculateNoise(xSampleCoord, ySampleCoord);
        }

        /// <summary>
        ///     Sample 2-Dimensional Perlin-Noise from local coordinates
        /// </summary>
        /// <param name="x">local x coordinate in chunk</param>
        /// <param name="y">local y coordinate in chunk</param>
        /// <param name="chunkCoordinates">coordinates of the chunk containing x, y</param>
        /// <param name="xOffset"></param>
        /// <param name="yOffset"></param>
        /// <returns></returns>
        public static float SamplePerlin2d(float x, float y, Vector2Int chunkCoordinates, float xOffset, float yOffset)
        {
            // Get coordinates in world space
            Vector2 worldCoords = Coordinates.GetWorldCoordinates(chunkCoordinates, x, y);

            // apply seed to x and y world coordinates
            float seededX = worldCoords.x + xOffset;
            float seededY = worldCoords.y + yOffset;

            // combine chunk and seed
            float xSampleCoord = seededX / World.CHUNK_SIZE;
            float ySampleCoord = seededY / World.CHUNK_SIZE;

            return CalculateNoise(xSampleCoord, ySampleCoord);
        }

        public static float CalculateNoise(float x, float y)
        {
            float persistance = .4f;
            int roughness = 3;
            int octaves = 3;

            float noise = 0;
            float frequency = 1;
            float factor = 1;

            for (int i = 0; i < octaves; i++)
            {
                noise += Mathf.PerlinNoise(x * frequency + i, y * frequency + i) * factor;
                factor *= persistance;
                frequency *= roughness;
            }

            return noise / .45f - 1;
        }
    }
}