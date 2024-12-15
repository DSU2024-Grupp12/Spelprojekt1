using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UpgradeMatrix : MonoBehaviour
{
    public string matrixID;
    public string displayName;

    private List<UpgradeModule> modules;
    private List<UpgradeModule> modulesLastFrame;

    private bool modulesUpdatedSinceLastFrame;

    private Dictionary<string, bool> containsDict;
    private Dictionary<string, (float b, float u)> upgradeDict;

    private void Start() {
        modules = new();
        containsDict = new();
        upgradeDict = new();

        modulesLastFrame = new();
        for (int i = 0; i < modules.Count; i++) {
            modulesLastFrame.Add(modules[i]);
        }
    }

    private void Update() {
        modulesUpdatedSinceLastFrame = false;
        // if the number of modules in the matrix has changed, we recalculate
        if (modulesLastFrame.Count != modules.Count) modulesUpdatedSinceLastFrame = true;
        else {
            for (int i = 0; i < modules.Count; i++) {
                if (!modules[i] || !modulesLastFrame[i]) break;
                // if at least one module does not match the last frame, we recalculate
                if (!modulesLastFrame[i].Equals(modules[i])) {
                    modulesUpdatedSinceLastFrame = true;
                    break;
                }
            }
        }
        modulesLastFrame.Clear();
        for (int i = 0; i < modules.Count; i++) {
            modulesLastFrame.Add(modules[i]);
        }
    }

    public bool Contains(string id) {
        if (containsDict == null) return false;
        if (containsDict.ContainsKey(id) && !modulesUpdatedSinceLastFrame) return containsDict[id];
        else {
            bool contains = modules.Select(m => m?.attributeID).Any(aid => aid == id);
            if (!containsDict.TryAdd(id, contains)) {
                containsDict[id] = contains;
            }
            return contains;
        }
    }

    public float GetUpgradeValue(string id, float baseValue, bool highGood) {
        // if the modules list is unchanged and the attribute has been calculated, retrieve from dictionary
        if (upgradeDict.ContainsKey(id) && !modulesUpdatedSinceLastFrame) {
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            // if the baseValue has changed, eg. from the inspector, then we recalculate
            if (upgradeDict[id].b == baseValue) return upgradeDict[id].u;
        }

        // find all modules that modify attribute with id, and get their upgrade function
        Func<float, float>[] upgradeFunctions = modules
                                                .Where(m => m.attributeID == id)
                                                .Select(m => m.UpgradeFunction()).ToArray();

        // get all the modules upgraded values for this attribute
        List<float> upgradeValues = new() { baseValue };
        foreach (Func<float, float> upgrade in upgradeFunctions) {
            upgradeValues.Add(upgrade(baseValue));
        }

        // pick the best value
        float ret;
        if (highGood) {
            ret = upgradeValues.Max();
        }
        else {
            ret = upgradeValues.Min();
        }

        // add to dictionary for faster retrieval
        if (!upgradeDict.TryAdd(id, (baseValue, ret))) {
            upgradeDict[id] = (baseValue, ret);
        }
        return ret;
    }

    public bool AttachModule(UpgradeModule module) {
        if (!modules.Contains(module)) {
            modules.Add(module);
            return true;
        }
        return false;
    }

    public bool RemoveModule(UpgradeModule module) {
        if (modules.Contains(module)) {
            modules.Remove(module);
            return true;
        }
        return false;
    }
}