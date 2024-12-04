using UnityEngine;
using UnityEngine.InputSystem;

public class GravityGun : Tool
{
    public float
        holdDistance,
        massLimit,
        range,
        marginBeforeLosingContact,
        power;

    public LayerMask beamableLayers;

    private Transform beam;
    private Rigidbody2D pickedUpBody;

    private bool performed;

    void Start() {
        beam = transform.GetChild(0);
        SetBeamLength(0);
    }

    void Update() {
        Debug.Log(performed);
        if (pickedUpBody) {
            Debug.Log("picked up");
            Vector3 toPickedUpObject = pickedUpBody.transform.position - transform.position;
            float distance = toPickedUpObject.magnitude;
            SetBeamLength(distance);
            float angle = Vector2.SignedAngle(transform.up, toPickedUpObject.normalized);
            SetBeamRotation(angle);
        }
        else {
            SetBeamLength(0);
        }
    }

    public override void PrimaryActivation(InputAction.CallbackContext context) {
        if (context.performed && !pickedUpBody) {
            performed = true;
            RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.up, range, beamableLayers);
            if (hit.collider) {
                IBeamable beamable = hit.collider.GetComponent<IBeamable>();
                if (beamable != null) {
                    beamable.PickUp();
                    pickedUpBody = hit.collider.GetComponent<Rigidbody2D>();
                    pickedUpBody.bodyType = RigidbodyType2D.Kinematic;
                    Debug.Log(pickedUpBody.mass);
                }
            }
        }
        Debug.Log(performed);
    }

    public override void SecondaryActivation(InputAction.CallbackContext context) {
        if (context.performed) { }
    }

    private void SetBeamLength(float length) {
        Vector3 scale = beam.transform.localScale;
        scale.y = length;
        beam.transform.localScale = scale;
    }

    private void SetBeamRotation(float degrees) {
        Vector3 rotation = beam.transform.localEulerAngles;
        rotation.z = degrees;
        beam.transform.localEulerAngles = rotation;
    }
}