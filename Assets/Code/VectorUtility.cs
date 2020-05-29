using UnityEngine;
using System.Collections;

public static class VectorUtility
{
    public static Vector3 One { get { return new Vector3(1, 1, 1); } }

    public static Vector3 InPlane(this Vector3 vector, Vector3 normal)
    {
        return vector - normal * vector.Dot(normal.normalized);
    }

    public static Vector3 InAxis(this Vector3 vector, Vector3 axis)
    {
        return axis * vector.Dot(axis.normalized);
    }

    public static Vector3 Scale(this Vector3 vector, float scalar)
    {
        vector.x *= scalar;
        vector.y *= scalar;
        vector.z *= scalar;

        return vector;
    }

    public static Vector3 WithMagnitudeChangedTo(this Vector3 vector, float magnitude)
    {
        return vector.normalized * magnitude;
    }

    public static Vector3 WithMagnitudeChangedBy(this Vector3 vector, float magnitude_delta)
    {
        return vector.normalized * (vector.magnitude + magnitude_delta);
    }

    public static float AngleBetween(this Vector3 vector, Vector3 other)
    {
        return MathUtility.DegreesToRadians(Vector3.Angle(vector, other));
    }

    public static float Distance(this Vector3 vector, Vector3 other) { return Vector3.Distance(vector, other); }
    public static float Dot(this Vector3 vector, Vector3 other) { return Vector3.Dot(vector, other); }
    public static Vector3 Scaled(this Vector3 vector, Vector3 other) { return Vector3.Scale(vector, other); }
    public static Vector3 Crossed(this Vector3 vector, Vector3 other) { return Vector3.Cross(vector, other); }
    public static Vector3 Lerped(this Vector3 vector, Vector3 other, float factor) { return Vector3.Lerp(vector, other, factor); }

    public static Vector3 XChangedTo(this Vector3 vector, float x) { return new Vector3(x, vector.y, vector.z); }
    public static Vector3 YChangedTo(this Vector3 vector, float y) { return new Vector3(vector.x, y, vector.z); }
    public static Vector3 ZChangedTo(this Vector3 vector, float z) { return new Vector3(vector.x, vector.y, z); }

    public static Vector3 XXX(this Vector3 vector) { return new Vector3(vector.x, vector.x, vector.x); }
    public static Vector3 XXY(this Vector3 vector) { return new Vector3(vector.x, vector.x, vector.y); }
    public static Vector3 XXZ(this Vector3 vector) { return new Vector3(vector.x, vector.x, vector.z); }
    public static Vector3 XYX(this Vector3 vector) { return new Vector3(vector.x, vector.y, vector.x); }
    public static Vector3 XYY(this Vector3 vector) { return new Vector3(vector.x, vector.y, vector.y); }
    public static Vector3 XYZ(this Vector3 vector) { return new Vector3(vector.x, vector.y, vector.z); }
    public static Vector3 XZX(this Vector3 vector) { return new Vector3(vector.x, vector.z, vector.x); }
    public static Vector3 XZY(this Vector3 vector) { return new Vector3(vector.x, vector.z, vector.y); }
    public static Vector3 XZZ(this Vector3 vector) { return new Vector3(vector.x, vector.z, vector.z); }

    public static Vector3 YXX(this Vector3 vector) { return new Vector3(vector.y, vector.x, vector.x); }
    public static Vector3 YXY(this Vector3 vector) { return new Vector3(vector.y, vector.x, vector.y); }
    public static Vector3 YXZ(this Vector3 vector) { return new Vector3(vector.y, vector.x, vector.z); }
    public static Vector3 YYX(this Vector3 vector) { return new Vector3(vector.y, vector.y, vector.x); }
    public static Vector3 YYY(this Vector3 vector) { return new Vector3(vector.y, vector.y, vector.y); }
    public static Vector3 YYZ(this Vector3 vector) { return new Vector3(vector.y, vector.y, vector.z); }
    public static Vector3 YZX(this Vector3 vector) { return new Vector3(vector.y, vector.z, vector.x); }
    public static Vector3 YZY(this Vector3 vector) { return new Vector3(vector.y, vector.z, vector.y); }
    public static Vector3 YZZ(this Vector3 vector) { return new Vector3(vector.y, vector.z, vector.z); }

    public static Vector3 ZXX(this Vector3 vector) { return new Vector3(vector.z, vector.x, vector.x); }
    public static Vector3 ZXY(this Vector3 vector) { return new Vector3(vector.z, vector.x, vector.y); }
    public static Vector3 ZXZ(this Vector3 vector) { return new Vector3(vector.z, vector.x, vector.z); }
    public static Vector3 ZYX(this Vector3 vector) { return new Vector3(vector.z, vector.y, vector.x); }
    public static Vector3 ZYY(this Vector3 vector) { return new Vector3(vector.z, vector.y, vector.y); }
    public static Vector3 ZYZ(this Vector3 vector) { return new Vector3(vector.z, vector.y, vector.z); }
    public static Vector3 ZZX(this Vector3 vector) { return new Vector3(vector.z, vector.z, vector.x); }
    public static Vector3 ZZY(this Vector3 vector) { return new Vector3(vector.z, vector.z, vector.y); }
    public static Vector3 ZZZ(this Vector3 vector) { return new Vector3(vector.z, vector.z, vector.z); }

    public static Vector3 XY0(this Vector3 vector) { return new Vector3(vector.x, vector.y, 0); }
    public static Vector3 X0Z(this Vector3 vector) { return new Vector3(vector.x, 0, vector.z); }
    public static Vector3 _0YZ(this Vector3 vector) { return new Vector3(0, vector.y, vector.z); }

    public static Vector2 XX(this Vector3 vector) { return new Vector2(vector.x, vector.x); }
    public static Vector2 XY(this Vector3 vector) { return new Vector2(vector.x, vector.y); }
    public static Vector2 XZ(this Vector3 vector) { return new Vector2(vector.x, vector.z); }
    public static Vector2 YX(this Vector3 vector) { return new Vector2(vector.y, vector.x); }
    public static Vector2 YY(this Vector3 vector) { return new Vector2(vector.y, vector.y); }
    public static Vector2 YZ(this Vector3 vector) { return new Vector2(vector.y, vector.z); }
    public static Vector2 ZX(this Vector3 vector) { return new Vector2(vector.z, vector.x); }
    public static Vector2 ZY(this Vector3 vector) { return new Vector2(vector.z, vector.y); }
    public static Vector2 ZZ(this Vector3 vector) { return new Vector2(vector.z, vector.z); }
}
