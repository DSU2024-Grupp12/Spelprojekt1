using UnityEngine;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(Ship))]
public class EnemyPilot : MonoBehaviour
{
    public Transform target;

    private Ship ship;
    private Rigidbody2D shipBody;

    [SerializeField]
    private Missile missile;

    [SerializeField]
    private Transform[] cannons;

    private int cannonIndex;

    [SerializeField, Min(0)]
    private float
        stopDistance,
        maxFireDistance,
        fireRate;

    private float startTime;
    private const float deathTime = 45f;

    private float actualFireRate;

    private float nextFireTime;

    // Start is called before the first frame update
    void Start() {
        ship = GetComponent<Ship>();
        shipBody = ship.GetComponent<Rigidbody2D>();
        actualFireRate = fireRate;
        if (actualFireRate > 0) {
            nextFireTime = Time.time + fireRate;
        }
        startTime = Time.time;
    }

    void Update() {
        if (target) {
            FireMissile();
        }
        if (Time.time >= startTime + deathTime) {
            ship.Explode();
        }
    }

    void FixedUpdate() {
        if (!target) {
            ship.StopAllThrusters();
            ship.stopping = true;
            actualFireRate = 0;
            return;
        }
        ship.stopping = false;
        actualFireRate = fireRate;

        Vector2 shipToTarget = target.transform.position - transform.position;
        float distanceToTarget = shipToTarget.magnitude;
        Vector2 directionToTarget = shipToTarget.normalized;
        float angleToTarget = Vector2.SignedAngle(transform.up, directionToTarget);

        if (Mathf.Abs(angleToTarget) > 4) {
            switch (Mathf.Sign(angleToTarget)) {
                case > 0:
                    ship.turningClockwise = false;
                    ship.turningCounterClockwise = true;
                    break;
                case < 0:
                    ship.turningClockwise = true;
                    ship.turningCounterClockwise = false;
                    break;
            }
        }
        else if (Mathf.Abs(shipBody.angularVelocity) > 0.1f) { // stablize when looking at player
            shipBody.angularVelocity *= 0.93f;
        }
        else {
            ship.turningClockwise = false;
            ship.turningCounterClockwise = false;
        }

        // accelerate if target is too far away
        if (distanceToTarget > stopDistance) {
            ship.accelerating = true;
            ship.stopping = false;
        }
        else {
            ship.accelerating = false;
            ship.stopping = true;
        }

        bool thrustersOn = ship.accelerating || ship.turningClockwise || ship.turningCounterClockwise;

        if (shipBody.velocity.magnitude < 0.1f) {
            ship.stopping = false;
        }
    }

    private void FireMissile() {
        Vector2 shipToTarget = target.transform.position - transform.position;
        float distanceToTarget = shipToTarget.magnitude;
        if (actualFireRate <= 0) return;
        if (distanceToTarget > maxFireDistance) return;

        if (Time.time > nextFireTime) {
            nextFireTime = Time.time + actualFireRate;

            Vector3 missilePosition = cannons[cannonIndex].position;
            Quaternion missileRotation = cannons[cannonIndex].rotation;
            Missile _missile = Instantiate(missile, missilePosition, missileRotation);
            _missile.createdByLayer = gameObject.layer;
            _missile.target = target;
            _missile.GetComponent<Rigidbody2D>().velocity = cannons[cannonIndex].up * shipBody.velocity.magnitude;
            _missile.GetComponent<Rigidbody2D>().AddForce(cannons[cannonIndex].up * 100);
            cannonIndex++;
            cannonIndex %= cannons.Length;
        }
    }

    // explode if ship is inside collider for too long
    private float stuckTimer;
    private const float stuckLimit = 10f;

    private void OnCollisionEnter2D(Collision2D other) {
        if (OnStuckableLayer(other.gameObject.layer)) stuckTimer = 0f;
    }

    private void OnCollisionStay2D(Collision2D other) {
        if (OnStuckableLayer(other.gameObject.layer)) {
            stuckTimer += Time.fixedDeltaTime;
            if (stuckTimer >= stuckLimit) ship.Explode();
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.layer == LayerMask.NameToLayer("PlayerShip")) {
            startTime = Time.time;
        }
    }

    private static bool OnStuckableLayer(int layer) {
        return layer is 13 or 7;
    }
}