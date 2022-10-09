using UnityEngine;

namespace Utility
{
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
    }
}
