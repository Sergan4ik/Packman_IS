using UnityEngine;

public static class Tools
{
    public static int ManhattanDistanceTo(this Vector2Int a, Vector2Int b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }
}