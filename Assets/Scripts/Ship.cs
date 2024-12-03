using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Ship : MonoBehaviour
{
    [SerializeField]
    private ThrusterGroup
        backThrusters,
        frontThrusters,
        clockwiseThrusters,
        counterClockwiseThrusters,
        rightSideThrusters,
        leftSideThrusters;

    [SerializeField]
    private float
        forwardThrust,
        backwardThrust,
        sideThrust,
        turningForce,
        maxVelocity,
        maxAngularVelocity,
        boostFactor,
        stoppingThrust,
        stoppingTorque,
        dampeningTime;

    [SerializeField, Range(0, 1)]
    private float adjustmentFactor, handlingFactor;

    // [SerializeField, Range(-1, 1)]
    // private float handlingCutoff;

    private float
        currentThrust,
        currentTorque;
    private float boostedForwardThrust => boosting && !stopping ? forwardThrust * boostFactor : forwardThrust;

    [HideInInspector]
    public bool
        accelerating,
        deaccelerating,
        strafingStarBoard,
        strafingPort,
        turningClockwise,
        turningCounterClockwise,
        boosting;

    public bool stopping {
        get => _stopping;
        set {
            _stopping = value;
            adjustedMaxVelocityActive = false;
            adjustedMaxAngularVelocityActive = false;
        }
    }

    private bool
        _stopping,
        adjustedMaxVelocityActive,
        adjustedMaxAngularVelocityActive;

    private Rigidbody2D body;
    private Vector2 velocityDirection {
        get {
            if (body.velocity.magnitude < 0.001f) {
                return transform.up;
            }
            return body.velocity.normalized;
        }
    }

    private void Start() {
        body = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate() {
        if (stopping) {
            if (body.velocity.magnitude > 0.001f) {
                Vector2 breakVector = -body.velocity;
                body.AddForce(breakVector * (stoppingThrust * Time.fixedDeltaTime));
            }
            if (body.angularVelocity is > 0.001f or < -0.001f) {
                float breakingTorque = stoppingTorque * -Mathf.Sign(body.angularVelocity);
                body.AddTorque(breakingTorque * Time.fixedDeltaTime);
            }
        }

        currentThrust += accelerating || boosting ? boostedForwardThrust : 0;
        currentThrust += deaccelerating ? -backwardThrust : 0;
        currentTorque += turningClockwise ? -turningForce : 0;
        currentTorque += turningCounterClockwise ? turningForce : 0;

        // adjust angular drag
        if (currentThrust != 0 && currentTorque == 0) body.angularDrag = 1f;
        else body.angularDrag = 0;

        // accelerate the ship forwards or backwards
        body.AddForce(
            transform.up * (currentThrust * Time.fixedDeltaTime)
        );

        // strafe in the starboard or port direction
        float strafeDirection = transform.eulerAngles.z;
        if (strafingStarBoard) strafeDirection -= 90;
        if (strafingPort) strafeDirection += 90;

        if (strafingStarBoard || strafingPort) {
            body.AddForce(GetDirectionVector(strafeDirection) * (sideThrust * Time.fixedDeltaTime));
        }

        body.AddTorque(currentTorque * Time.fixedDeltaTime);

        ConsoleUtility.ClearLog();

        // prevent ship from exceeding maxVelocity
        float adjustedMaxVelocity = maxVelocity * adjustmentFactor;
        if (body.velocity.magnitude < adjustedMaxVelocity) adjustedMaxVelocityActive = true;
        if (stopping && adjustedMaxVelocityActive) {
            if (body.velocity.magnitude > adjustedMaxVelocity) {
                body.velocity = body.velocity.normalized * adjustedMaxVelocity;
            }
        }
        else if (body.velocity.magnitude > maxVelocity) {
            body.velocity = body.velocity.normalized * maxVelocity;
        }

        // prevent ship from exceeding maxAngularVelocity
        float adjustedMaxAngularVelocity = maxAngularVelocity * adjustmentFactor;
        if (Mathf.Abs(body.angularVelocity) < adjustedMaxAngularVelocity) adjustedMaxAngularVelocityActive = true;
        if (stopping && adjustedMaxAngularVelocityActive) {
            if (Mathf.Abs(body.angularVelocity) > adjustedMaxAngularVelocity) {
                body.angularVelocity = Mathf.Sign(body.angularVelocity) * adjustedMaxAngularVelocity;
            }
        }
        else if (body.angularVelocity > maxAngularVelocity) {
            body.angularVelocity = Mathf.Sign(body.angularVelocity) * maxAngularVelocity;
        }

        // deaccelerate in traveling direction if it is very different from acceleration direction
        if (currentThrust != 0 && !stopping) {
            Vector2 accelerationDirection = transform.up;
            float differenceFactor = -(Vector2.Dot(accelerationDirection, velocityDirection) - 1f) / 2f;

            ConsoleUtility.ClearLog();
            Debug.Log($"dot: {differenceFactor}");

            if (differenceFactor > 0) {
                float spaceFriction = 1 - handlingFactor * differenceFactor;
                body.velocity *= Mathf.Pow(spaceFriction, Time.fixedDeltaTime);
                body.AddForce(body.totalForce / Mathf.Pow(spaceFriction, Time.fixedDeltaTime / 2));
            }
        }

        currentThrust = 0;
        currentTorque = 0;

        UpdateThrusterParticles();
    }

    private void UpdateThrusterParticles() {
        if (stopping) {
            backThrusters.Stop();
            frontThrusters.Stop();
            clockwiseThrusters.Stop();
            counterClockwiseThrusters.Stop();
            rightSideThrusters.Stop();
            leftSideThrusters.Stop();
            return;
        }

        if (accelerating || boosting) backThrusters.Play();
        else backThrusters.Stop();

        if (deaccelerating) frontThrusters.Play();
        else frontThrusters.Stop();

        if (turningClockwise) clockwiseThrusters.Play();
        else clockwiseThrusters.Stop();

        if (turningCounterClockwise) counterClockwiseThrusters.Play();
        else counterClockwiseThrusters.Stop();

        if (strafingStarBoard) leftSideThrusters.Play();
        else leftSideThrusters.Stop();

        if (strafingPort) rightSideThrusters.Play();
        else rightSideThrusters.Stop();
    }

    private Vector2 GetDirectionVector(float eulerAngleZ) {
        float theta = (eulerAngleZ % 360) * Mathf.Deg2Rad;
        return new Vector2(-Mathf.Sin(theta), Mathf.Cos(theta));
    }
}