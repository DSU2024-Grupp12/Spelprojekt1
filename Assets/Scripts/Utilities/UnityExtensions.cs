using UnityEngine;

public static class UnityExtensions
{
    //https://discussions.unity.com/t/check-if-layer-is-in-layermask/16007
    /// <summary>
    /// Checks wether the layer is in the layermask
    /// </summary>
    public static bool Contains(this LayerMask mask, int layer) {
        return mask == (mask | (1 << layer));
    }

    /// <summary>
    /// Converts a 0-360 degree value to a normalized vector2
    /// </summary>
    public static Vector2 DegreesToVector2(this float eulerAngleZ) {
        float theta = (eulerAngleZ % 360) * Mathf.Deg2Rad;
        return new Vector2(-Mathf.Sin(theta), Mathf.Cos(theta));
    }
}