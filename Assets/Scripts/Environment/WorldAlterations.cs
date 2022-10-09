using System;
using System.Collections.Generic;
using UnityEngine;
using Utility;

namespace Environment
{
    [Serializable]
    public class WorldAlterations
    {
        public struct DecodedAlteration
        {
            public int chunkX;
            public int chunkY;
            public int x;
            public int y;

            public DecodedAlteration(UInt128 encodedAlteration)
            {
                chunkX = (int)(encodedAlteration >> 96 & 0xFFFFFFFF);
                chunkY = (int)(encodedAlteration >> 64 & 0xFFFFFFFF);
                x = (int)(encodedAlteration >> 32 & 0xFFFFFFFF);
                y = (int)(encodedAlteration & 0xFFFFFFFF);
            }
        }
        
        [SerializeField] private List<UInt128> alterations;

        public List<UInt128> GetAlterations()
        {
            return alterations;
        }

        public WorldAlterations()
        {
            alterations = new List<UInt128>();
        }
        
        public void AddAlteration(UInt128 alteration)
        {
            alterations.Add(alteration);
        }

        public void AddAlteration(string alteration)
        {
            alterations.Add(UInt128.Parse(alteration));
        }

        public void AddAlteration(int chunkX, int chunkY, int x, int y)
        {
            UInt128 alteration = GetEncodedAlteration(chunkX, chunkY, x, y);
            alterations.Add(alteration);
        }

        public void RemoveAlteration(int chunkX, int chunkY, int x, int y)
        {
            UInt128 alteration = GetEncodedAlteration(chunkX, chunkY, x, y);
            if (HasAlteration(alteration)) alterations.Remove(alteration);
        }

        public void RemoveAlteration(UInt128 alteration)
        {
            if (HasAlteration(alteration)) alterations.Remove(alteration);
        }

        public bool HasAlteration(int chunkX, int chunkY, int x, int y)
        {
            return alterations.Contains(GetEncodedAlteration(chunkX, chunkY, x, y));
        }

        public bool HasAlteration(UInt128 alteration)
        {
            return alterations.Contains(alteration);
        }

        public static UInt128 GetEncodedAlteration(int chunkX, int chunkY, int x, int y)
        {
            UInt128 encodedAlteration = 0x0;
            encodedAlteration |= (UInt128)chunkX << 96;
            encodedAlteration |= (UInt128)chunkY << 64;
            encodedAlteration |= (UInt128)x << 32;
            encodedAlteration |= (UInt128)y;
            return encodedAlteration;
        }

        public static DecodedAlteration GetDecodedAlteration(UInt128 encodedAlteration)
        {
            return new DecodedAlteration(encodedAlteration);
        }
    }
}