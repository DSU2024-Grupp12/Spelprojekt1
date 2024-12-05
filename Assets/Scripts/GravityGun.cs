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
    private Rigidbody2D pickedUpBody;

    private ParticleSystem container;

    void Start() {
        SetBeamLength(0);
        container = GetComponentInChildren<ParticleSystem>();
    }

    void Update() {
        if (pickedUpBody) {
            Vector2 toPickedUpObject = pickedUpBody.transform.position - transform.position;
            float distance = toPickedUpObject.magnitude;
            SetBeamLength(distance);
            float angle = Vector2.SignedAngle(mount.up, toPickedUpObject.normalized);
            SetBeamRotation(angle);
            container.Play();
        }
        else {
            SetBeamLength(0);
            container.Stop();
            container.Clear();
        }
    }

    private void FixedUpdate() {
        if (pickedUpBody) {
            Vector2 holdPosition = mount.position + mount.up * holdDistance;
            // Vector2 diff = pickedUpBody.
            pickedUpBody.transform.position = holdPosition; // maybe change velocity instead?
            pickedUpBody.transform.eulerAngles = mount.eulerAngles;
        }
    }

    public override void PrimaryActivation() {
        if (!pickedUpBody) {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.up, range, beamableLayers);
            if (hit.collider) {
                if (hit.collider.GetComponent<Rigidbody2D>().mass >= massLimit) return;
                IBeamable beamable = hit.collider.GetComponent<IBeamable>();
                if (beamable != null) {
                    beamable.PickUp();
                    pickedUpBody = hit.collider.GetComponent<Rigidbody2D>();
                    pickedUpBody.bodyType = RigidbodyType2D.Kinematic;
                }
            }
        }
    }

    public override void SecondaryActivation() {
        if (pickedUpBody) {
            Blast();
        }
    }

    private void Blast() {
        pickedUpBody.bodyType = RigidbodyType2D.Dynamic;
        pickedUpBody.AddForce(mount.up * power);
        pickedUpBody = null;
    }

    private void SetBeamLength(float length) {
        Vector3 scale = transform.localScale;
        scale.y = length;
        transform.localScale = scale;
    }

    private void SetBeamRotation(float degrees) {
        Vector3 rotation = transform.localEulerAngles;
        rotation.z = degrees;
        transform.localEulerAngles = rotation;
    }
}