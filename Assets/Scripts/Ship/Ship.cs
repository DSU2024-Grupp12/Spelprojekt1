using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Ship : MonoBehaviour, IUIValueProvider<float>
{
    [SerializeField]
    private ParticleSystem explosionPrefab;
    [SerializeField]
    private Thrusters thrusters;

    [Header("Thruster Power")]
    [SerializeField, Tooltip("(Min 0)")]
    private Upgradeable forwardThrust;
    [SerializeField, Tooltip("(Min 0)")]
    private Upgradeable
        backwardThrust,
        sideThrust,
        turningForce;

    [Header("Maximums")]
    [SerializeField, Tooltip("(Min 0)")]
    private Upgradeable maxVelocity;
    [SerializeField, Tooltip("(Min 0)")]
    private Upgradeable maxAngularVelocity;

    [Space]
    public Boosters boosters;

    [Header("Handling")]
    [SerializeField, Min(0)]
    private float stoppingThrust;
    [SerializeField, Min(0)]
    private float stoppingTorque;
    [SerializeField, Tooltip("(Min 0)")]
    private Upgradeable handlingFactor;
    [SerializeField, Range(0, 1)]
    private float adjustmentFactor;

    private float
        currentThrust,
        currentTorque;
    private float boost => !stopping ? boosters.GetBoost(forwardThrust) : 0;

    [HideInInspector]
    public bool
        accelerating,
        deaccelerating,
        strafingStarBoard,
        strafingPort,
        turningClockwise,
        turningCounterClockwise;

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

        currentThrust += accelerating ? forwardThrust : 0;
        if (boosters) currentThrust += boosters.boosting ? boost : 0;
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
        else if (Mathf.Abs(body.angularVelocity) > maxAngularVelocity) {
            body.angularVelocity = Mathf.Sign(body.angularVelocity) * maxAngularVelocity;
        }

        // deaccelerate in traveling direction if it is very different from acceleration direction
        if (currentThrust > 0 && !stopping) {
            Vector2 accelerationDirection = transform.up;
            float differenceFactor = -(Vector2.Dot(accelerationDirection, velocityDirection) - 1f) / 2f;

            if (differenceFactor > 0.3f) {
                float handlingModifier = handlingFactor * differenceFactor * body.mass * Time.fixedDeltaTime;
                // body.velocity -= body.velocity.normalized * handlingModifier;
                body.AddForce(-body.velocity.normalized * handlingModifier);
                body.AddForce(accelerationDirection * handlingModifier);
                // body.AddForce(body.totalForce / Mathf.Pow(spaceFriction, Time.fixedDeltaTime / 2));
            }
        }

        currentThrust = 0;
        currentTorque = 0;

        UpdateThrusters();
    }

    public void Explode() {
        if (explosionPrefab) {
            Vector3 explosionPosition = new Vector3(transform.position.x, transform.position.y, -3);
            Instantiate(explosionPrefab, explosionPosition, transform.rotation);
            Destroy(gameObject);
        }
    }

    public void StopAllThrusters() {
        accelerating = false;
        deaccelerating = false;
        turningClockwise = false;
        turningCounterClockwise = false;
        strafingPort = false;
        strafingStarBoard = false;
    }

    private void UpdateThrusters() {
        if (stopping) {
            thrusters.back.Stop();
            thrusters.front.Stop();
            thrusters.clockwise.Stop();
            thrusters.counterClockwise.Stop();
            thrusters.rightSide.Stop();
            thrusters.leftSide.Stop();
            return;
        }

        if (accelerating) thrusters.back.Play();
        else if (boosters && boosters.boosting) thrusters.back.Play();
        else thrusters.back.Stop();

        if (deaccelerating) thrusters.front.Play();
        else thrusters.front.Stop();

        if (turningClockwise) thrusters.clockwise.Play();
        else thrusters.clockwise.Stop();

        if (turningCounterClockwise) thrusters.counterClockwise.Play();
        else thrusters.counterClockwise.Stop();

        if (strafingStarBoard) thrusters.leftSide.Play();
        else thrusters.leftSide.Stop();

        if (strafingPort) thrusters.rightSide.Play();
        else thrusters.rightSide.Stop();

        if (accelerating || turningClockwise || turningCounterClockwise) thrusters.PlayLargeThrusterSound();
        else thrusters.StopLargeThrusterSound();

        if (deaccelerating || strafingPort || strafingStarBoard) thrusters.PlaySmallThrusterSound();
        else thrusters.StopSmallThrusterSound();
    }

    private Vector2 GetDirectionVector(float eulerAngleZ) {
        float theta = (eulerAngleZ % 360) * Mathf.Deg2Rad;
        return new Vector2(-Mathf.Sin(theta), Mathf.Cos(theta));
    }
    /// <inheritdoc />
    public string GetID() {
        return "ship";
    }
    /// <inheritdoc />
    public float BaseValue() {
        return 0;
    }
    /// <inheritdoc />
    public float CurrentValue() {
        return body.velocity.magnitude;
    }
}

[System.Serializable]
public class Thrusters
{
    [SerializeField]
    private AudioPlayer thrusterSounds;
    [SerializeField]
    private string
        largeThrusterAudioAssetName,
        smallThrusterAudioAssetName;

    public ThrusterGroup
        back,
        front,
        clockwise,
        counterClockwise,
        rightSide,
        leftSide;

    public void PlaySmallThrusterSound() {
        if (smallThrusterAudioAssetName == "") return;
        thrusterSounds.Play(smallThrusterAudioAssetName);
    }

    public void StopSmallThrusterSound() {
        if (smallThrusterAudioAssetName == "") return;
        thrusterSounds.Stop(smallThrusterAudioAssetName);
    }

    public void PlayLargeThrusterSound() {
        if (largeThrusterAudioAssetName == "") return;
        thrusterSounds.Play(largeThrusterAudioAssetName);
    }

    public void StopLargeThrusterSound() {
        if (largeThrusterAudioAssetName == "") return;
        thrusterSounds.Stop(largeThrusterAudioAssetName);
    }
}