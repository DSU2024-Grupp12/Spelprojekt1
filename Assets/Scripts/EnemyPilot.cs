using UnityEngine;

[RequireComponent(typeof(Ship))]
public class EnemyPilot : MonoBehaviour
{
    private Ship ship;
    private Rigidbody2D shipBody;

    public Transform target;

    [SerializeField]
    private float minDistance;

    // Start is called before the first frame update
    void Start() {
        ship = GetComponent<Ship>();
        shipBody = ship.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate() {
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
            shipBody.angularVelocity *= 0.8f;
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
    }
}