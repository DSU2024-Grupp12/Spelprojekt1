using UnityEngine;

public class Ship : MonoBehaviour
{
    [SerializeField]
    private float
        forwardThrust,
        breakThrust,
        maxSpeed,
        turningForce,
        maxTurningSpeed;

    private float currentThrust, currentTorque;

    [SerializeField]
    private Rigidbody2D body;

    // Start is called before the first frame update
    void Start() { }

    void FixedUpdate() {
        body.AddForce(GetDirectionVector(transform.eulerAngles.z) * currentThrust);
        if (body.velocity.magnitude > maxSpeed) {
            body.velocity = body.velocity.normalized * maxSpeed;
        }
        body.AddTorque(currentTorque);
    }

    private Vector2 GetDirectionVector(float eulerAngleZ) {
        float theta = (eulerAngleZ % 360) * Mathf.Deg2Rad;
        return new Vector2(-Mathf.Sin(theta), Mathf.Cos(theta));
    }
}