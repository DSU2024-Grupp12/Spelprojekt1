using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Serialization;

[System.Serializable]
public class Upgradeable
{
    [SerializeField] private float baseValue;
    [SerializeField] [CanBeNull] private UpgradeMatrix matrix;
    [FormerlySerializedAs("matrixID")] [SerializeField] private string attributeID;
    public bool highGood;

    public float value
    {
        get
        {
            if (matrix != null && matrix.Contains(attributeID))
                return matrix.GetUpgradeValue(attributeID, baseValue, highGood);
            else return baseValue;
        }
        set => baseValue = value;
    }

    public static implicit operator float(Upgradeable u) => u.value;
}
