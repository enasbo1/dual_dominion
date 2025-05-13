using UnityEngine;

namespace Shared
{
    public static class Vector2Extension
    {
        public static Vector2 FromV3(Vector3? v3)
        {
            return new Vector2(v3?.x ?? 0, v3?.z ?? 0);
        }
        
        public static Vector2 RotateDeg(Vector2 vector2, float degrees)
        {
            return RotateRad(vector2, degrees * Mathf.Deg2Rad);
        }

        public static Vector2 RotateRad(Vector2 vector2, float radians)
        {
            float sin = Mathf.Sin(radians);
            float cos = Mathf.Cos(radians);

            float tx = vector2.x;
            float ty = vector2.y;
            return new Vector2(cos * tx - sin * ty, cos * ty + sin * tx);
        }
    }
}