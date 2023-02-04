using System.Text.RegularExpressions;
using UnityEngine;

public static class ExtensionMethods {
    public static float Remap(this float value, float fromRangeStart, float fromRangeEnd, float toRangeStart, float toRangeEnd) {
        return toRangeStart + (value - fromRangeStart) * (toRangeEnd - toRangeStart) / (fromRangeEnd - fromRangeStart);
    }

    public static Vector2 Remap(this Vector2 value, Vector2 fromRangeStart, Vector2 fromRangeEnd, Vector2 toRangeStart, Vector2 toRangeEnd) {
        return toRangeStart + (value - fromRangeStart) * (toRangeEnd - toRangeStart) / (fromRangeEnd - fromRangeStart);
    }

    public static Vector3 RandomPointInCircle(this float maxDistance, float minimumDistance = 0, float y = 0) {
        return (maxDistance.RandomPointInCircle(minimumDistance)).ToV3(y);
    }

    public static Vector2 RandomPointInCircle(this float maxDistance, float minimumDistance = 0) {
        //var offset = Random.Range(minimumDistance, maxDistance);
        //var direction = Random.insideUnitCircle.normalized;

        //return direction * offset;

        var randomPointInUnitCircle = (Random.insideUnitCircle * (maxDistance - minimumDistance));
        return randomPointInUnitCircle + (randomPointInUnitCircle / randomPointInUnitCircle.Abs()) * minimumDistance;
    }

    public static Vector3 ToV3(this Vector2 value, float y = 0) {
        return new Vector3(value.x, y, value.y);
    }
    public static Vector3 To2DV3(this Vector2 value, float z = 0) {
        return new Vector3(value.x, value.y, z);
    }

    public static Vector2 Abs(this Vector2 value) {
        return new Vector2(Mathf.Abs(value.x), Mathf.Abs(value.y));
    }

    public static Vector3 WithY(this Vector3 value, float y) {
        return new Vector3(value.x, y, value.z);
    }

    public static void DestroyChildren(this Transform transform) {
        for (int i = transform.childCount - 1; i >= 0; i--) {
            Object.Destroy(transform.GetChild(i).gameObject);
        }
    }

    public static string CamelToWords(this string str) {
        return Regex.Replace(str, "(\\B[A-Z])", " $1");
    }

    public static Vector2 ToV2WithoutZ(this Vector3 value) {
        return new Vector2(value.x, value.y);
    }
}
