using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody2D))]
public class Hull : MonoBehaviour
{
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
    public UnityEvent HullDestroyed;
    public UnityEvent OnTakeDamage;
    [Range(0, 1)]
    public float significantDamageThreshold;
    public UnityEvent OnTakeDamageSignificant;

    void Start() {
        endOfInvincibility = Time.time;
        currentStrength = strength;
    }

    private void OnCollisionEnter2D(Collision2D other) {
        if (!collideWith.Contains(other.gameObject.layer)) return;
        if (Time.time < endOfInvincibility) return;

        endOfInvincibility = Time.time + invincibilityWindow;

        float kineticEnergy = 0.5f * other.otherRigidbody.mass * Mathf.Pow(other.relativeVelocity.magnitude, 2);
        TakeDamage(kineticEnergy);
    }

    public void TakeDamage(float damage) {
        float modifiedDamage = Mathf.Max((damage - threshold) * (1 - dampener), 0);
        if (modifiedDamage == 0) return;

        currentStrength -= modifiedDamage;

        if (currentStrength <= 0 && !hullDestroyed) {
            hullDestroyed = true;
            HullDestroyed.Invoke();
            return;
        }
        if (damage >= significantDamageThreshold * strength) OnTakeDamageSignificant?.Invoke();
        else OnTakeDamage?.Invoke();
    }

    public void RepairHull(float repairAmount) {
        currentStrength = Mathf.Min(strength, currentStrength + repairAmount);
    }
}