using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boosters : MonoBehaviour, IUIValueProvider<float>
{
    [SerializeField, Tooltip("(Min 1)")]
    private Upgradeable boostFactor;
    [SerializeField, Tooltip("The maximum length of the boost in seconds\n(Min 0)")]
    private Upgradeable boostLength;
    [SerializeField, Tooltip("The amount of seconds of boost recovered every second\n(Min 0)")]
    private Upgradeable boostRecovery;
    [SerializeField, Tooltip("The time in seconds until recovery starts if you fully deplete your boost\n(Min 0)")]
    private Upgradeable boostDepletedDelay;
    [SerializeField,
     Tooltip("The amount of boost in seconds that needs to recover after " +
             "depletiong before you can boost again\n(Min 0)")]
    private Upgradeable boostActiveDelay;

    [HideInInspector]
    public bool boosting;

    private bool inDelay;
    private bool inactive;
    private bool depleted;

    private float remaingBoost;

    private void Start() {
        remaingBoost = boostLength;
    }

    public float GetBoost(float thrust) {
        if (inactive) return 0;
        if (boosting && remaingBoost > 0) {
            return thrust * (boostFactor - 1);
        }
        return 0;
    }

    public void Update() {
        if (inDelay) {
            Debug.Log("Delay");
            return;
        }
        if (inactive) {
            Debug.Log("inactive");
            return;
        }
        if (boosting && remaingBoost > 0) {
            // boost normally
            Debug.Log("boost");
            remaingBoost -= Time.deltaTime;
        }
        if (boosting && remaingBoost < 0) {
            // boost depleted
            Debug.Log("boost depleted");
            StartCoroutine(BoostRecoveryDelay(boostDepletedDelay, boostActiveDelay));
            remaingBoost = 0;
        }
        if (!boosting) {
            Debug.Log("Recovering");
            remaingBoost = Mathf.Max(0, remaingBoost);
            remaingBoost += boostRecovery * Time.deltaTime;
            remaingBoost = Mathf.Min(boostLength, remaingBoost);
        }
    }

    private IEnumerator BoostRecoveryDelay(float recoveryDelay, float inactiveDelay) {
        inDelay = true;
        inactive = true;
        yield return new WaitForSeconds(recoveryDelay);
        inDelay = false;
        while (remaingBoost < inactiveDelay) {
            Debug.Log("Recovering");
            remaingBoost += boostRecovery * Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        inactive = false;
    }

    /// <inheritdoc />
    public string GetID() {
        return "boosters";
    }
    /// <inheritdoc />
    public float BaseValue() {
        return boostLength;
    }
    /// <inheritdoc />
    public float CurrentValue() {
        return Mathf.Max(remaingBoost, 0);
    }
}