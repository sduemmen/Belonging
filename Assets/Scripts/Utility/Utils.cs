using System;
using System.Collections.Generic;
using UnityEngine;

namespace Utility
{
    public enum ComparisonType
    {
        LessThanTarget,
        LessOrEqualTarget,
        EqualTarget,
        GreaterOrEqualTarget,
        GreaterThanTarget
    }

    public static class Utils
    {
        public static class Math
        {
            public static Vector2 RotateVector2Rad(Vector2 v, float rad)
            {
                float x = v.x * Mathf.Cos(rad) - v.y * Mathf.Sin(rad);
                float y = v.x * Mathf.Sin(rad) + v.y * Mathf.Cos(rad);
                return new Vector2(x, y);
            }

            public static Vector2 RotateVector2Deg(Vector2 v, float deg)
            {
                float x = v.x * Mathf.Cos(Mathf.Deg2Rad * deg) - v.y * Mathf.Sin(Mathf.Deg2Rad * deg);
                float y = v.x * Mathf.Sin(Mathf.Deg2Rad * deg) + v.y * Mathf.Cos(Mathf.Deg2Rad * deg);
                return new Vector2(x, y);
            }

            public static List<Vector3> GramSchmidtOrthogonalization(List<Vector3> V_in, bool normalize)
            {
                // check if input vectors are linearly independent
                if (!CheckLinearIndependence(V_in, out float determinant))
                {
                    throw new ArgumentException($"Input vectors are not linearly independent");
                }
                
                // initialize output
                List<Vector3> V_out = new List<Vector3>();

                V_out.Add(V_in[0]);

                // Gram-Schmidt Algorithm for Orthogonalization
                for (int i = 1; i < V_in.Count; i++)
                {
                    Vector3 v = Vector3.zero;
                    
                    for (int k = 0; k < i; k++)
                    {
                        v = Vector3.Dot(V_in[i], V_out[k]) / Vector3.Dot(V_out[k], V_out[k]) * V_out[k];
                    }

                    V_out.Add(V_in[i] - v);
                }

                if (normalize)
                {
                    for (int i = 0; i < V_out.Count; i++)
                    {
                        V_out[i] = V_out[i].normalized;
                    }
                }
                
                return V_out;
            }

            public static bool CheckLinearIndependence(List<Vector3> V_in, out float determinant)
            {
                // construct determinant matrix M
                float[,] M = new float[5, 3];

                for (int i = 0; i < 5; i++)
                {
                    M[i, 0] = V_in[i % V_in.Count].x;
                    M[i, 1] = V_in[i % V_in.Count].y;
                    M[i, 2] = V_in[i % V_in.Count].z;
                }

                // calculate determinant
                float d = 0;
                
                for (int i = 0; i < 3; i++)
                {
                    d += M[i, 0] * M[i + 1, 1] * M[i + 2, 2];
                }

                for (int i = 0; i < 3; i++)
                {
                    d -= M[i, 2] * M[i + 1, 1] * M[i + 2, 0];
                }

                determinant = d;
                return d != 0;
            }
        }
        

        public static bool Evaluate(ComparisonType comparisonType, float a, float target)
        {
            switch (comparisonType)
            {
                case ComparisonType.LessThanTarget:
                    return a < target;
                case ComparisonType.LessOrEqualTarget:
                    return a <= target;
                case ComparisonType.EqualTarget:
                    return Mathf.Abs(a - target) < 0.000001f;
                case ComparisonType.GreaterOrEqualTarget:
                    return a >= target;
                case ComparisonType.GreaterThanTarget:
                    return a > target;
            }

            return false;
        }

        public static bool Evaluate(ComparisonType comparisonType, int a, int target)
        {
            switch (comparisonType)
            {
                case ComparisonType.LessThanTarget:
                    return a < target;
                case ComparisonType.LessOrEqualTarget:
                    return a <= target;
                case ComparisonType.EqualTarget:
                    return a == target;
                case ComparisonType.GreaterOrEqualTarget:
                    return a >= target;
                case ComparisonType.GreaterThanTarget:
                    return a > target;
            }

            return false;
        }
    }
}