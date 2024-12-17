using JetBrains.Annotations;
using UnityEngine;

[System.Serializable]
public class Upgradeable
{
    [SerializeField]
    private float baseValue;
    [SerializeField, CanBeNull]
    private UpgradeMatrix matrix;
    [SerializeField]
    private string attributeID;
    public bool highGood = true;

    public float value {
        get {
            if (matrix) {
                return matrix.GetUpgradeValue(attributeID, baseValue, highGood);
            }
            else return baseValue;
        }
        set => baseValue = value;
    }

    public static implicit operator float(Upgradeable u) => u.value;
}