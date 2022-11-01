using System;
using UnityEngine;

namespace Utility
{
    public enum Comparison
    {
        LessThanTarget,
        LessOrEqualTarget,
        EqualTarget,
        GreaterOrEqualTarget,
        GreaterThanTarget
    }

    public static class MathUtilities
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

        public static bool Evaluate(Comparison comparison, float a, float target)
        {
            switch (comparison)
            {
                case Comparison.LessThanTarget:
                    return a < target;
                case Comparison.LessOrEqualTarget:
                    return a <= target;
                case Comparison.EqualTarget:
                    return Math.Abs(a - target) < 0.000001f;
                case Comparison.GreaterOrEqualTarget:
                    return a >= target;
                case Comparison.GreaterThanTarget:
                    return a > target;
            }

            return false;
        }

        public static bool Evaluate(Comparison comparison, int a, int target)
        {
            switch (comparison)
            {
                case Comparison.LessThanTarget:
                    return a < target;
                case Comparison.LessOrEqualTarget:
                    return a <= target;
                case Comparison.EqualTarget:
                    return a == target;
                case Comparison.GreaterOrEqualTarget:
                    return a >= target;
                case Comparison.GreaterThanTarget:
                    return a > target;
            }

            return false;
        }
    }
}