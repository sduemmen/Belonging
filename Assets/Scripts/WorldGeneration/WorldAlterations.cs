using System;
using System.Collections.Generic;
using UnityEngine;

namespace WorldGeneration
{
    [Serializable]
    public class WorldAlterations
    {
        [Serializable]
        public struct Alteration
        {
            public Vector2Int chunk;
            public Vector2Int coordinatesInChunk;

            public Alteration(Vector2Int chunk, Vector2Int coordinatesInChunk)
            {
                this.chunk = chunk;
                this.coordinatesInChunk = coordinatesInChunk;
            }
            
            public Alteration(int chunkX, int chunkY, int x, int y)
            {
                this.chunk = new Vector2Int(chunkX, chunkY);
                this.coordinatesInChunk = new Vector2Int(x, y);
            }
        }
        
        public List<Alteration> alterations;

        public WorldAlterations()
        {
            alterations = new List<Alteration>();
        }

        public void AddAlteration(int chunkX, int chunkY, int x, int y)
        {
            alterations.Add(new Alteration(chunkX, chunkY, x, y));
        }

        public void AddAlteration(Vector2Int chunkPosition, Vector2Int coordinatesInChunk)
        {
            alterations.Add(new Alteration(chunkPosition, coordinatesInChunk));
        }
        
        public void AddAlteration(Alteration alteration)
        {
            alterations.Add(alteration);
        }

        public void RemoveAlteration(int chunkX, int chunkY, int x, int y)
        {
            Alteration alteration = new Alteration(chunkX, chunkY, x, y);
            if (HasAlteration(alteration)) alterations.Remove(alteration);
        }
        
        public void RemoveAlteration(Vector2Int chunkPosition, Vector2Int coordinatesInChunk)
        {
            Alteration alteration = new Alteration(chunkPosition, coordinatesInChunk);
            if (HasAlteration(alteration)) alterations.Remove(alteration);
        }

        public void RemoveAlteration(Alteration alteration)
        {
            if (HasAlteration(alteration)) alterations.Remove(alteration);
        }

        public bool HasAlterationAt(int chunkX, int chunkY, int x, int y)
        {
            return alterations.Contains(new Alteration(chunkX, chunkY, x, y));
        }

        public bool HasAlterationAt(Vector2Int chunkPosition, Vector2Int positionInChunk)
        {
            return alterations.Contains(new Alteration(chunkPosition, positionInChunk));
        }

        public bool HasAlteration(Alteration alteration)
        {
            return alterations.Contains(alteration);
        }
    }
}