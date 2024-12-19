using UnityEngine;

public class Shield : MonoBehaviour, IUIValueProvider<float>
{
    public float absorbtionCapacity;
    private float currentCapacity;
    [Range(0f, 1f)]
    public float absorbtionRate;
    public float absorbtionLimit;

    [Tooltip("If this option is selected, the damage that is not absorbed is dealt to the hull. " +
             "If it is not selected, all the damage will be absorbed if any capacity remains but the capacity will" +
             "only be reduced by the absorbed damage")]
    public bool fallThroughDamage = true;

    public bool autoRegenerate;

    [SerializeField, HideInInspector]
    private float regenerationRate, regenerationCooldown;
    private float timeUntilRegenStart;

    private void Start() {
        currentCapacity = absorbtionCapacity;
    }

    private void Update() {
        if (autoRegenerate && Time.time >= timeUntilRegenStart) {
            currentCapacity = Mathf.Min(currentCapacity + regenerationRate * Time.deltaTime, absorbtionCapacity);
        }
    }

    /// <summary>
    /// Absorbs a portion of the damage and reducing the damage by the absorbed amount.
    /// </summary>
    /// <returns>Returns true if any damage was absorbed, and false if not</returns>
    public bool AbsorbDamage(ref float damage) {
        if (damage <= 0) return false;
        timeUntilRegenStart = Time.time + regenerationCooldown;
        if (currentCapacity <= 0) return false;

        // absorb damage based on absorbtion rate, unless it exceeds absorbtionlimit
        float absorbedDamage = Mathf.Min(damage * absorbtionRate, absorbtionLimit);
        // only absorb up to remaining capacity;
        absorbedDamage = Mathf.Min(currentCapacity, absorbedDamage);

        currentCapacity -= absorbedDamage;
        if (fallThroughDamage) damage -= absorbedDamage;
        else damage = 0;

        return true;
    }

    public float BaseValue() {
        return absorbtionCapacity;
    }
    public float CurrentValue() {
        return currentCapacity;
    }
    public string GetID() {
        return "shield";
    }
}