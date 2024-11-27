using UnityEngine;
using UnityEngine.InputSystem;

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
        maxVelocity,
        turningForce,
        maxAngularVelocity,
        boostFactor,
        stoppingThrust,
        stoppingTorque;

    [SerializeField, Range(0, 1)]
    private float adjustmentFactor;

    private float
        currentThrust,
        currentTorque;
    private float boostedForwardThrust => boosting && !stopping ? forwardThrust * boostFactor : forwardThrust;

    private bool
        accelerating,
        deaccelerating,
        strafingStarBoard,
        strafingPort,
        turningClockwise,
        turningCounterClockwise,
        boosting,
        stopping;

    private Rigidbody2D body;

    private void Start() {
        body = GetComponent<Rigidbody2D>();
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

        if (accelerating) backThrusters.Play();
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

        currentThrust += accelerating ? boostedForwardThrust : 0;
        currentThrust += deaccelerating ? -backwardThrust : 0;
        currentTorque += turningClockwise ? -turningForce : 0;
        currentTorque += turningCounterClockwise ? turningForce : 0;

        // accelerate the ship forwards or backwards
        body.AddForce(GetDirectionVector(transform.eulerAngles.z) * (currentThrust * Time.fixedDeltaTime));

        // strafe in the starboard or port direction
        float strafeDirection = transform.eulerAngles.z;
        if (strafingStarBoard) strafeDirection -= 90;
        if (strafingPort) strafeDirection += 90;

        if (strafingStarBoard || strafingPort) {
            body.AddForce(GetDirectionVector(strafeDirection) * (sideThrust * Time.fixedDeltaTime));
        }

        // prevent ship from exceeding maxVelocity
        float realMaxVelocity = maxVelocity;
        if (body.velocity.magnitude < maxVelocity * adjustmentFactor) {
            // only reduce the maximum velocity if the current velocity is less than reduced max
            realMaxVelocity *= (stopping ? adjustmentFactor : 1f);
        }
        if (body.velocity.magnitude > realMaxVelocity) body.velocity = body.velocity.normalized * realMaxVelocity;

        // prevent ship from exceeding maxAngularVelocity
        float realMaxAngularVelocity = maxAngularVelocity;
        if (body.angularVelocity.Between(-realMaxAngularVelocity, realMaxAngularVelocity)) {
            // only reduce the maximum angular velocity if the current angular velocity is less than reduced max
            realMaxAngularVelocity *= (stopping ? adjustmentFactor : 1f);
        }

        if (!stopping || body.angularVelocity.Between(-realMaxAngularVelocity, realMaxAngularVelocity)) {
            // turn ship clockwise or counterclockwise
            body.AddTorque(currentTorque * Time.fixedDeltaTime);
        }
        if (body.angularVelocity > realMaxAngularVelocity) body.angularVelocity = realMaxAngularVelocity;
        if (body.angularVelocity < -realMaxAngularVelocity) body.angularVelocity = -realMaxAngularVelocity;

        currentThrust = 0;
        currentTorque = 0;

        UpdateThrusterParticles();
    }

    private Vector2 GetDirectionVector(float eulerAngleZ) {
        float theta = (eulerAngleZ % 360) * Mathf.Deg2Rad;
        return new Vector2(-Mathf.Sin(theta), Mathf.Cos(theta));
    }

    public void Accelerate(InputAction.CallbackContext context) {
        accelerating = context.ReadValueAsButton();
    }

    public void Deaccelerate(InputAction.CallbackContext context) {
        deaccelerating = context.ReadValueAsButton();
    }

    public void TurnClockwise(InputAction.CallbackContext context) {
        turningClockwise = context.ReadValueAsButton();
    }

    public void TurnCounterClockwise(InputAction.CallbackContext context) {
        turningCounterClockwise = context.ReadValueAsButton();
    }

    public void StrafeStarBoard(InputAction.CallbackContext context) {
        strafingStarBoard = context.ReadValueAsButton();
    }

    public void StrafePort(InputAction.CallbackContext context) {
        strafingPort = context.ReadValueAsButton();
    }

    public void Boost(InputAction.CallbackContext context) {
        boosting = context.ReadValueAsButton();
    }

    public void Stop(InputAction.CallbackContext context) {
        stopping = context.ReadValueAsButton();
    }
}