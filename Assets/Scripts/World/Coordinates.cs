using System;
using UnityEngine;

namespace World
{
    public static class Coordinates
    {
        public static Vector2Int GetChunkCoordinates(float worldX, float worldY)
        {
            int chunkX = (int)Math.Floor((worldX + World.CHUNK_SIZE / 2) / World.CHUNK_SIZE);
            int chunkY = (int)Math.Floor((worldY + World.CHUNK_SIZE / 2) / World.CHUNK_SIZE);
            return new Vector2Int(chunkX, chunkY);
        }

        public static Vector2 GetWorldCoordinates(Vector2Int chunkCoordinates, float x, float y)
        {
            int halfChunkSize = World.CHUNK_SIZE / 2;
            float worldX = chunkCoordinates.x * World.CHUNK_SIZE + x - halfChunkSize;
            float worldY = chunkCoordinates.y * World.CHUNK_SIZE + y - halfChunkSize;
            return new Vector2(worldX, worldY);
        }
    }
}