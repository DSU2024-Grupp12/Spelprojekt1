using UnityEngine;

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
    private float minDistance, fireRate;

    private float nextFireTime;

    // Start is called before the first frame update
    void Start() {
        ship = GetComponent<Ship>();
        shipBody = ship.GetComponent<Rigidbody2D>();
        if (fireRate > 0) {
            nextFireTime = Time.time + fireRate;
        }
    }

    void Update() {
        FireMissile();
    }

    void FixedUpdate() {
        if (!target) {
            ship.StopAllThrusters();
            ship.stopping = true;
            fireRate = 0;
            return;
        }
        Vector2 shipToTarget = target.transform.position - transform.position;
        float distanceToTarget = shipToTarget.magnitude;
        Vector2 directionToTarget = shipToTarget.normalized;
        float angleToTarget = Vector2.SignedAngle(transform.up, directionToTarget);

        // turn towards player
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
        if (distanceToTarget > minDistance) {
            ship.accelerating = true;
            ship.stopping = false;
        }
        else {
            ship.accelerating = false;
            ship.stopping = true;
        }

        if (shipBody.velocity.magnitude < 0.1f) {
            ship.stopping = false;
        }
    }

    private void FireMissile() {
        if (fireRate <= 0) return;
        if (Time.time > nextFireTime) {
            nextFireTime = Time.time + fireRate;

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
}