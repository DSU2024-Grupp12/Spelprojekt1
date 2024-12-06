using UnityEngine;

public class GravityGun : Tool
{
    public float
        holdDistance,
        massLimit,
        range,
        snappingSpeed,
        firingVelocity;

    public LayerMask beamableLayers;
    private Rigidbody2D pickedUpBody;

    private ParticleSystem container;
    private ParticleSystem indicator;

    private SpriteRenderer spriteRenderer;

    void Start() {
        SetBeamLength(0);
        container = transform.GetChild(0).GetComponent<ParticleSystem>();
        indicator = transform.GetChild(1).GetComponent<ParticleSystem>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update() {
        if (pickedUpBody) {
            spriteRenderer.enabled = true;
            ConnectBeamToPoint(pickedUpBody.transform.position);
            container.Play();
            indicator.Stop();
            indicator.Clear();
        }
        else {
            spriteRenderer.enabled = false;
            SetBeamLength(0);
            container.Stop();
            container.Clear();

            if (ValidBodyInRange(out Rigidbody2D inRange)) {
                ConnectBeamToPoint(inRange.transform.position);
                indicator.Play();
            }
            else {
                indicator.Stop();
                indicator.Clear();
            }
        }
    }

    private void FixedUpdate() {
        if (pickedUpBody) {
            Vector2 holdPosition = mount.position + mount.up * holdDistance;
            Vector2 diff = holdPosition - (Vector2)pickedUpBody.transform.position;
            pickedUpBody.velocity = diff / snappingSpeed;

            pickedUpBody.transform.eulerAngles = mount.eulerAngles;
        }
    }

    public override void PrimaryActivation() {
        if (!pickedUpBody) {
            if (ValidBodyInRange(out Rigidbody2D toPickUp)) {
                PickUp(toPickUp);
            }
        }
        else {
            Detach();
        }
    }

    public override void SecondaryActivation() {
        if (pickedUpBody) {
            Blast();
        }
    }

    private void PickUp(Rigidbody2D body) {
        IBeamable[] beamables = body.GetComponents<IBeamable>();
        if (beamables != null) {
            foreach (IBeamable beamable in beamables) {
                if (!beamable.PickUp()) return;
            }
            pickedUpBody = body;
            pickedUpBody.bodyType = RigidbodyType2D.Kinematic;
            pickedUpBody.useFullKinematicContacts = true;
        }
    }

    private void Blast() {
        pickedUpBody.bodyType = RigidbodyType2D.Dynamic;
        pickedUpBody.velocity = (Vector2)mount.up * firingVelocity;
        pickedUpBody = null;
    }

    private void Detach() {
        pickedUpBody.bodyType = RigidbodyType2D.Dynamic;
        IBeamable[] beamables = pickedUpBody.GetComponents<IBeamable>();
        foreach (IBeamable beamable in beamables) {
            beamable.Dropped();
        }
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

    private void ConnectBeamToPoint(Vector3 point) {
        Vector2 toPoint = point - transform.position;
        float distance = toPoint.magnitude;
        SetBeamLength(distance);
        float angle = Vector2.SignedAngle(mount.up, toPoint.normalized);
        SetBeamRotation(angle);
    }

    private bool ValidBodyInRange(out Rigidbody2D bodyInRange) {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, mount.up, range, beamableLayers);
        bodyInRange = null;
        if (hit.collider) {
            bodyInRange = hit.collider.GetComponent<Rigidbody2D>();
            if (bodyInRange.mass >= massLimit) return false;
            return true;
        }
        return false;
    }
}