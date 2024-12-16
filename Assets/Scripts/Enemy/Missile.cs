using UnityEngine;

[RequireComponent(typeof(Ship))]
public class Missile : MonoBehaviour, IBeamable
{
    public Transform target;
    public bool homing;
    [HideInInspector]
    public int createdByLayer;

    [SerializeField, Min(0)]
    private float
        inertTime,
        lifeTime,
        turningSpeed,
        explosionRadius,
        explosionPower;

    [SerializeField]
    private LayerMask willExplodeOn;

    private float deathTime, activationTime;
    private Ship missile;

    private LayerMask rigidbodyExcludeMask;
    private bool createdByLayerNotExcludedInRigidBody;
    private bool createdByLayerInExplodeOn;

    // Start is called before the first frame update
    void Start() {
        deathTime = Time.time + lifeTime;
        activationTime = Time.time + inertTime;

        rigidbodyExcludeMask = GetComponent<Rigidbody2D>().excludeLayers;
        if (inertTime > 0) {
            createdByLayerInExplodeOn = willExplodeOn.Remove(createdByLayer);
            createdByLayerNotExcludedInRigidBody = rigidbodyExcludeMask.Add(createdByLayer);
        }
        GetComponent<Rigidbody2D>().excludeLayers = rigidbodyExcludeMask;

        missile = GetComponent<Ship>();
        missile.accelerating = true;
    }

    // Update is called once per frame
    void Update() {
        if (Time.time >= activationTime) {
            if (createdByLayerInExplodeOn) willExplodeOn.Add(createdByLayer);
            if (createdByLayerNotExcludedInRigidBody) {
                rigidbodyExcludeMask.Remove(createdByLayer);
                GetComponent<Rigidbody2D>().excludeLayers = rigidbodyExcludeMask;
            }
        }
        if (Time.time >= deathTime) {
            Explode();
        }
    }

    private void FixedUpdate() {
        if (!target) return;
        Vector2 missileToTarget = target.transform.position - transform.position;
        Vector2 directionToTarget = missileToTarget.normalized;
        float angleToTarget = Vector2.SignedAngle(transform.up, directionToTarget);

        // turn towards player
        if (Mathf.Abs(angleToTarget) > turningSpeed * Time.fixedDeltaTime) {
            switch (Mathf.Sign(angleToTarget)) {
                case > 0:
                    transform.Rotate(Vector3.forward, turningSpeed * Time.fixedDeltaTime);
                    break;
                case < 0:
                    transform.Rotate(Vector3.forward, -turningSpeed * Time.fixedDeltaTime);
                    break;
            }
        }
        else {
            missile.turningClockwise = false;
            missile.turningCounterClockwise = false;
        }

        if (homing) {
            Rigidbody2D body = missile.GetComponent<Rigidbody2D>();
            float magnitude = body.velocity.magnitude;
            body.velocity = Vector2.zero;
            body.velocity = transform.up * magnitude;
        }
    }

    private void OnCollisionEnter2D(Collision2D other) {
        if (Time.time < activationTime) return;
        if (willExplodeOn.Contains(other.gameObject.layer)) {
            Explode();
        }
    }

    public void Explode() {
        // find all hulls within explosion radius
        foreach (Hull hull in FindObjectsOfType<Hull>()) {
            float distance = ((Vector2)hull.transform.position - (Vector2)transform.position).magnitude;
            if (distance <= explosionRadius) {
                // Debug.Log(explosionPower);
                hull.TakeDamage(explosionPower, gameObject.layer);
            }
        }

        missile.Explode();
    }

    /// <inheritdoc />
    public bool PickUp() {
        target = null;
        return true;
    }

    /// <inheritdoc />
    public void Dropped() { }
}