using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody2D))]
public class Hull : MonoBehaviour, IUIValueProvider<float>
{
    [SerializeField]
    private Shield shield;

    [SerializeField, Tooltip("The total amount of kinetic energy the hull can absorb before breaking.\n(Min 0)")]
    private Upgradeable strength;
    [SerializeField]
    private float startingStrength;

    private float currentStrength;
    public bool hullDestroyed { get; private set; }

    private float invincibilityWindow = 0.2f;
    private float endOfInvincibility;

    [HideInInspector]
    public bool inCloud;

    [SerializeField, Tooltip("The minimum amount of kinetic energy required before any damage is dealt.\n(Min 0)")]
    private Upgradeable threshold;

    [SerializeField, Tooltip("The coefficient with which the damage taken is multiplied.\n(Min 0, Max 1)")]
    private Upgradeable dampener;

    public LayerMask collideWith;
    public LayerModifier[] layerModifiers;
    public UnityEvent HullDestroyed;
    public TakeDamageEvents takeDamageEvents;

    private bool hullInitialized;

    void Awake() {
        endOfInvincibility = Time.time;
        if (startingStrength <= 0) {
            currentStrength = strength;
        }
        else currentStrength = startingStrength;
    }

    private void Update() {
        hullInitialized = true;
    }

    private void OnCollisionEnter2D(Collision2D other) {
        if (!collideWith.Contains(other.gameObject.layer)) return;
        if (Time.time < endOfInvincibility) return;

        endOfInvincibility = Time.time + invincibilityWindow;

        // other.otherRigidbody returns the wrong rigidBody for some reason so we manually get it instead
        Rigidbody2D otherBody = other.gameObject.GetComponent<Rigidbody2D>();

        float pseudoKineticEnergy = 0.5f * otherBody.mass * other.relativeVelocity.magnitude;
        TakeDamage(pseudoKineticEnergy, other.gameObject.layer);
    }

    /// <summary>
    /// Deals damage to the hull, adjusting for hulls dampening, threshold and layer modifiers
    /// </summary>
    /// <returns>True if the damage would reduce the current hull to 0 or less</returns>
    public bool TakeDamage(float damage, int incurringLayer = 0) {
        float layerModifier = 1;
        float layerMax = float.MaxValue;
        foreach (LayerModifier modifier in layerModifiers) {
            if (incurringLayer == LayerMask.NameToLayer(modifier.layer)) {
                layerModifier = modifier.modifier;
                if (modifier.max > 0) layerMax = modifier.max;
                break;
            }
        }
        float modifiedDamage = Mathf.Clamp((damage - threshold) * (1 - dampener) * layerModifier, 0, layerMax);

        if (shield) {
            if (shield.AbsorbDamage(ref modifiedDamage)) {
                takeDamageEvents.OnTakeDamageShield?.Invoke();
            }
        }

        return TakeRawDamage(modifiedDamage, incurringLayer);
    }

    /// <summary>
    /// Deals damage to the hull, not adjusting for hulls dampening, threshold, layer modifiers and shield
    /// </summary>
    /// <returns>True if the damage would reduce the current hull to 0 or less</returns>
    public bool TakeRawDamage(float rawDamage, int incurringLayer = 0) {
        if (rawDamage <= 0) return false;

        currentStrength -= rawDamage;

        takeDamageEvents.OnTakeAnyDamage?.Invoke();

        if (currentStrength <= 0 && !hullDestroyed) {
            hullDestroyed = true;
            HullDestroyed.Invoke();
            return true;
        }

        if (rawDamage >= takeDamageEvents.significantDamageThreshold * strength) {
            if (takeDamageEvents.enemyLayers.Contains(incurringLayer))
                takeDamageEvents.OnTakeDamageEnemySignificant?.Invoke();
            else if (takeDamageEvents.debrisLayers.Contains(incurringLayer))
                takeDamageEvents.OnTakeDamageDebrisSignificant?.Invoke();
            else
                takeDamageEvents.OnTakeDamageSignificant?.Invoke();
        }
        else {
            if (takeDamageEvents.enemyLayers.Contains(incurringLayer))
                takeDamageEvents.OnTakeDamageEnemy?.Invoke();
            else if (takeDamageEvents.debrisLayers.Contains(incurringLayer))
                takeDamageEvents.OnTakeDamageDebris?.Invoke();
            else takeDamageEvents.OnTakeDamage?.Invoke();
        }

        if (currentStrength <= strength * takeDamageEvents.lowHullStrengthThreshold) {
            if (!takeDamageEvents.lowHullStrengthReached) {
                takeDamageEvents.OnReachLowHullStrength?.Invoke();
                takeDamageEvents.lowHullStrengthReached = true;
            }
        }
        return false;
    }

    public bool RepairHull(float repairAmount) {
        if (Mathf.Approximately(currentStrength, strength)) return false;
        currentStrength = Mathf.Min(strength, currentStrength + repairAmount);
        if (currentStrength > strength * takeDamageEvents.lowHullStrengthThreshold) {
            takeDamageEvents.lowHullStrengthReached = false;
        }
        return true;
    }

    public bool AtFullStrength() {
        return Mathf.Approximately(currentStrength, strength);
    }

    public void SetHullStrength(float newStrength) {
        if (!hullInitialized) {
            float diff = newStrength - strength;
            strength.value = newStrength;
            currentStrength += diff;
        }
    }

    public float BaseValue() {
        return strength;
    }
    public float CurrentValue() {
        return currentStrength;
    }
    public string GetID() {
        return "hull";
    }
}

[System.Serializable]
public struct LayerModifier
{
    public string layer;
    [Min(0)]
    public float modifier;
    public float max;
}

[System.Serializable]
public class TakeDamageEvents
{
    [Range(0, 1)]
    public float
        significantDamageThreshold,
        lowHullStrengthThreshold;
    [HideInInspector]
    public bool lowHullStrengthReached;

    public LayerMask enemyLayers;
    public LayerMask debrisLayers;
    public UnityEvent
        OnTakeAnyDamage,
        OnTakeDamage,
        OnTakeDamageEnemy,
        OnTakeDamageDebris,
        OnTakeDamageSignificant,
        OnTakeDamageEnemySignificant,
        OnTakeDamageDebrisSignificant,
        OnTakeDamageShield,
        OnReachLowHullStrength;
}