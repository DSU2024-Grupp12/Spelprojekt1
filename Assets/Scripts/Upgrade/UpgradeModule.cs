using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewUpgradeModuleFloat", menuName = "Upgrade Module")]
public class UpgradeModule : ScriptableObject
{
    [Tooltip("The matrix id of the upgrade matrix that this module is designed for")]
    public string matrixID;
    [Tooltip("The attribute id of the attribute this module will modify")]
    public string attributeID;

    public string moduleDisplayName;
    [TextArea(1, 4)]
    public string moduleDescription;

    public int greenResourceCost;
    public int purpleResourceCost;

    public ModuleTypes moduleType;
    public float modifier;

    /// <summary>
    /// Returns a function that takes a value and returns the upgraded version of it, based on the module type and modifier
    /// </summary>
    public Func<float, float> UpgradeFunction() {
        switch (moduleType) {
            case ModuleTypes.Additive: return Additive(modifier);
            case ModuleTypes.Set: return Set(modifier);
            case ModuleTypes.Multiply: return Multiply(modifier);
            default: return v => v;
        }
    }

    private static Func<float, float> Additive(float mod) {
        return v => v + mod;
    }
    private static Func<float, float> Set(float mod) {
        return _ => mod;
    }
    private static Func<float, float> Multiply(float mod) {
        return v => v * mod;
    }

    public enum ModuleTypes
    {
        Additive,
        Set,
        Multiply,
    }
}