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

    //https://docs.unity3d.com/6000.0/Documentation/Manual/layermask-add.html
    /// <summary>
    /// Attempts to add a layer to the layer mask and returns true if the layer added did not already exist on the mask;
    /// </summary>
    public static bool Add(ref this LayerMask mask, int layer) {
        if (mask.Contains(layer)) return false;
        mask |= (1 << layer);
        return true;
    }

    //https://docs.unity3d.com/6000.0/Documentation/Manual/layermask-remove.html
    /// <summary>
    /// Attempts to removes a layer from the layer mask and returns true if the layer to remove existed in the mask
    /// </summary>
    public static bool Remove(ref this LayerMask mask, int layer) {
        if (!mask.Contains(layer)) return false;
        mask &= ~(1 << layer);
        return true;
    }

    /// <summary>
    /// Converts a 0-360 degree value to a normalized vector2
    /// </summary>
    public static Vector2 DegreesToVector2(this float eulerAngleZ) {
        float theta = (eulerAngleZ % 360) * Mathf.Deg2Rad;
        return new Vector2(-Mathf.Sin(theta), Mathf.Cos(theta));
    }
}