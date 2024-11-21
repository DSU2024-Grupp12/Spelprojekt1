using UnityEngine;
using UnityEngine.InputSystem;

public class Ship : MonoBehaviour
{
    [SerializeField]
    private ParticleSystem
        RightBackThruster,
        LeftBackThruster,
        RightSideThruster,
        LeftSideThruster;

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

    private float
        currentThrust,
        currentTorque;
    private float boostedForwardThrust => boosting ? forwardThrust * boostFactor : forwardThrust;

    private bool
        accelerating,
        deaccelerating,
        strafingStarBoard,
        strafingPort,
        turningClockwise,
        turningCounterClockwise,
        boosting,
        stopping;

    [SerializeField]
    private Rigidbody2D body;

    private void UpdateThrusterParticles() {
        if (accelerating) {
            RightBackThruster.Play();
            LeftBackThruster.Play();
        }
        else {
            RightBackThruster.Stop();
            LeftBackThruster.Stop();
        }

        if (strafingStarBoard) LeftSideThruster.Play();
        else LeftSideThruster.Stop();
        if (strafingPort) RightSideThruster.Play();
        else RightSideThruster.Stop();
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
            return;
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
        if (body.velocity.magnitude > maxVelocity) body.velocity = body.velocity.normalized * maxVelocity;

        // turn ship clockwise or counterclockwise
        body.AddTorque(currentTorque * Time.fixedDeltaTime);

        // prevent ship from exceeding maxAngularVelocity
        if (body.angularVelocity > maxAngularVelocity) body.angularVelocity = maxAngularVelocity;
        if (body.angularVelocity < -maxAngularVelocity) body.angularVelocity = -maxAngularVelocity;

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