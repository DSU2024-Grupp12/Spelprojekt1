using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UpgradeMatrix : MonoBehaviour
{
    public string matrixID;
    public string displayName;

    [Tooltip("If selected, addtivit and multiply modules affecting the same attribute will additively stack. " +
             "If there are any set modules also affecting the attribute, the best set model will be used and " +
             "then any additive and multiply modules will be calculated")]
    public bool stackModules;

    private List<UpgradeModule> modules;

    private Dictionary<string, (float b, float u)> upgradeDict;
    private Dictionary<string, bool> recalculated;

    private bool initialized;

    private void Start() {
        modules = new();
        upgradeDict = new();
        recalculated = new();
        initialized = true;
    }

    private void Update() {
        // recalculate = false;
        // // if the number of modules in the matrix has changed, we recalculate
        // if (modulesLastFrame.Count != modules.Count) recalculate = true;
        // else {
        //     for (int i = 0; i < modules.Count; i++) {
        //         if (!modules[i] || !modulesLastFrame[i]) break;
        //         // if at least one module does not match the last frame, we recalculate
        //         if (!modulesLastFrame[i].Equals(modules[i])) {
        //             recalculate = true;
        //             break;
        //         }
        //     }
        // }
        // modulesLastFrame.Clear();
        // for (int i = 0; i < modules.Count; i++) {
        //     modulesLastFrame.Add(modules[i]);
        // }
    }

    public float GetUpgradeValue(string id, float baseValue, bool highGood) {
        if (!initialized) return baseValue;

        // if this value has not been seen before we add recalculation check
        recalculated.TryAdd(id, false);

        // if the modules list is unchanged and the attribute has been calculated, retrieve from dictionary
        if (upgradeDict.ContainsKey(id) && recalculated[id]) {
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            // if the baseValue has changed, eg. from the inspector, then we recalculate
            if (upgradeDict[id].b != baseValue) {
                recalculated[id] = false;
                return GetUpgradeValue(id, baseValue, highGood);
            }
        }

        // find all modules that modify attribute with id
        UpgradeModule[] relevantModules = modules.Where(m => m.attributeID == id).ToArray();
        // and get their upgrade function
        Func<float, float>[] upgradeFunctions = relevantModules.Select(m => m.UpgradeFunction()).ToArray();

        float ret;
        if (!stackModules) {
            ret = NonStackedModuleUpgrade(baseValue, highGood, upgradeFunctions);
        }
        else {
            ret = StackedModuleUpgrade(baseValue, highGood, relevantModules);
        }

        // add to dictionary for faster retrieval
        if (!upgradeDict.TryAdd(id, (baseValue, ret))) {
            upgradeDict[id] = (baseValue, ret);
        }
        recalculated[id] = true;
        return ret;
    }

    public bool AttachModule(UpgradeModule module) {
        if (!modules.Contains(module)) {
            modules.Add(module);
            CallForRecalculate();
            return true;
        }
        return false;
    }

    public bool RemoveModule(UpgradeModule module) {
        if (modules.Contains(module)) {
            modules.Remove(module);
            CallForRecalculate();
            return true;
        }
        return false;
    }

    private void CallForRecalculate() {
        foreach (string s in recalculated.Keys.ToArray()) {
            recalculated[s] = false;
        }
    }

    private float GetBestValue(float[] values, bool highGood) {
        if (highGood) {
            return values.Max();
        }
        else {
            return values.Min();
        }
    }

    private float NonStackedModuleUpgrade(float baseValue, bool highGood, Func<float, float>[] upgradeFunctions) {
        // get all the modules upgraded values for this attribute
        List<float> upgradeValues = new() { baseValue };
        foreach (Func<float, float> upgrade in upgradeFunctions) {
            upgradeValues.Add(upgrade(baseValue));
        }
        // pick the best value
        return GetBestValue(upgradeValues.ToArray(), highGood);
    }

    private float StackedModuleUpgrade(float baseValue, bool highGood, UpgradeModule[] relevantModules) {
        Func<float, float>[] setFunctions =
            relevantModules.Where(m => m.moduleType == UpgradeModule.ModuleTypes.Set)
                           .Select(m => m.UpgradeFunction()).ToArray();
        List<float> setValues = new();
        foreach (Func<float, float> upgrade in setFunctions) {
            setValues.Add(upgrade(baseValue));
        }
        float bestSet = GetBestValue(setValues.ToArray(), highGood);

        Func<float, float>[] addFunctions =
            relevantModules.Where(m => m.moduleType == UpgradeModule.ModuleTypes.Additive)
                           .Select(m => m.UpgradeFunction()).ToArray();

        List<float> addValues = new();
        foreach (Func<float, float> upgrade in setFunctions) {
            addValues.Add(upgrade(0));
        }

        Func<float, float>[] multiplyFuncs =
            relevantModules.Where(m => m.moduleType == UpgradeModule.ModuleTypes.Multiply)
                           .Select(m => m.UpgradeFunction()).ToArray();
        List<float> multiplyValueDiffs = new();
        foreach (Func<float, float> multiplyFunc in multiplyFuncs) {
            multiplyValueDiffs.Add(multiplyFunc(baseValue) - baseValue);
        }

        return bestSet + addValues.Sum() + multiplyValueDiffs.Sum();
    }
}