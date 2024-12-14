using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UpgradeMatrix : MonoBehaviour
{
    [SerializeField]   
    private List<UpgradeModule> modules; 
    
    public bool Contains(string id)
    {
        return modules.Select(m => m.attributeID).Any(aid => aid == id);
    }
    
    public float GetUpgradeValue(string id, float baseValue, bool highGood)
    {
        Func<float, float>[] upgradeFunctions = modules
            .Where(m => m.attributeID == id)
            .Select(m => m.UpgradeFunction()).ToArray();

        List<float> upgradeValues = new();
        foreach (Func<float,float> upgrade in upgradeFunctions)
        {
            upgradeValues.Add(upgrade(baseValue));
        }

        if (highGood) return upgradeValues.Max();
        else return upgradeValues.Min();
    }
}
