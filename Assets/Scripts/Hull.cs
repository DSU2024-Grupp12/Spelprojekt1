using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody2D))]
public class Hull : MonoBehaviour
{
    [SerializeField, Tooltip("The total amount of kinetic energy the hull can absorb before breaking.")]
    private float strength;

    public float fullStrength => strength;

    public float currentStrength { get; private set; }
    public bool hullDestroyed { get; private set; }

    private float invincibilityWindow = 0.2f;
    private float endOfInvincibility;

    [SerializeField, Tooltip("The minimum amount of kinetic energy required before any damage is dealt.")]
    private float threshold;

    [SerializeField, Range(0, 1), Tooltip("The coefficient with which the damage taken is multiplied.")]
    private float dampener;

    public LayerMask collideWith;
    public UnityEvent HullDestroyed;
    public TakeDamageEvents takeDamageEvents;

    void Start() {
        endOfInvincibility = Time.time;
        currentStrength = strength;
    }

    private void OnCollisionEnter2D(Collision2D other) {
        if (!collideWith.Contains(other.gameObject.layer)) return;
        if (Time.time < endOfInvincibility) return;

        endOfInvincibility = Time.time + invincibilityWindow;

        float kineticEnergy = 0.5f * other.otherRigidbody.mass * Mathf.Pow(other.relativeVelocity.magnitude, 2);
        TakeDamage(kineticEnergy, other.gameObject.layer);
    }

    public void TakeDamage(float damage, int incurringLayer = 0) {
        float modifiedDamage = Mathf.Max((damage - threshold) * (1 - dampener), 0);

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
        OnReachLowHullStrength;
}