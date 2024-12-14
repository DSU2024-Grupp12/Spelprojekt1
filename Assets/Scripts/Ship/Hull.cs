using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody2D))]
public class Hull : MonoBehaviour, IUIValueProvider<float>
{
    public UpgradeMatrix matrix;
    
    [SerializeField]
    private Shield shield;

    [SerializeField, Tooltip("The total amount of kinetic energy the hull can absorb before breaking.")]
    private float strength;

    private float currentStrength;
    public bool hullDestroyed { get; private set; }

    private float invincibilityWindow = 0.2f;
    private float endOfInvincibility;

    [SerializeField, Tooltip("The minimum amount of kinetic energy required before any damage is dealt.")]
    private float threshold;

    [SerializeField, Range(0, 1), Tooltip("The coefficient with which the damage taken is multiplied.")]
    private float dampener;

    public LayerMask collideWith;
    public LayerModifier[] layerModifiers;
    public UnityEvent HullDestroyed;
    public TakeDamageEvents takeDamageEvents;

    private bool hullInitialized;

    void Start() {
        endOfInvincibility = Time.time;
        currentStrength = strength;
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

    public void TakeDamage(float damage, int incurringLayer = 0) {
        float layerModifier = 1;
        float layerMax = float.MaxValue;
        foreach (LayerModifier modifier in layerModifiers) {
            if (incurringLayer == LayerMask.NameToLayer(modifier.layer)) {
                layerModifier = modifier.modifier;
                layerMax = modifier.max;
                break;
            }
        }
        float modifiedDamage = Mathf.Clamp((damage - threshold) * (1 - dampener) * layerModifier, 0, layerMax);

        if (shield) {
            if (shield.AbsorbDamage(ref modifiedDamage)) {
                takeDamageEvents.OnTakeDamageShield?.Invoke();
            }
        }

        TakeRawDamage(modifiedDamage, incurringLayer);
    }

    public void TakeRawDamage(float rawDamage, int incurringLayer = 0) {
        if (rawDamage <= 0) return;

        currentStrength -= rawDamage;

        if (currentStrength <= 0 && !hullDestroyed) {
            hullDestroyed = true;
            HullDestroyed.Invoke();
            return;
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
    }

    public void RepairHull(float repairAmount) {
        currentStrength = Mathf.Min(strength, currentStrength + repairAmount);
        if (currentStrength > strength * takeDamageEvents.lowHullStrengthThreshold) {
            takeDamageEvents.lowHullStrengthReached = false;
        }
    }

    public void SetHullStrength(float newStrength) {
        if (!hullInitialized) {
            strength = newStrength;
            currentStrength = newStrength;
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
        OnTakeDamage,
        OnTakeDamageEnemy,
        OnTakeDamageDebris,
        OnTakeDamageSignificant,
        OnTakeDamageEnemySignificant,
        OnTakeDamageDebrisSignificant,
        OnTakeDamageShield,
        OnReachLowHullStrength;
}