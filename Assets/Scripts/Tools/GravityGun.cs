using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class GravityGun : Tool
{
    public float
        holdDistance,
        width,
        snappingSpeed;

    public Upgradeable
        massLimit,
        range,
        firingVelocity;

    public LayerMask beamableLayers;
    private Rigidbody2D pickedUpBody;

    [SerializeField]
    private ParticleSystem container, indicator, invalidIndicator;

    [SerializeField]
    private SpriteRenderer spriteRenderer;

    [SerializeField]
    private BoxCollider2D inRangeCollider;

    void Start() {
        SetBeamLength(0);
        SetColliderDimensions();
    }

    void Update() {
        if (pickedUpBody) {
            spriteRenderer.enabled = true;
            ConnectBeamToPoint(pickedUpBody.transform.position);
            container.Play();
            invalidIndicator.Stop();
            invalidIndicator.Clear();
            indicator.Stop();
            indicator.Clear();
        }
        else {
            spriteRenderer.enabled = false;
            container.Stop();
            container.Clear();

            bool valid = ValidBodyInRange(out Rigidbody2D inRange);
            if (inRange) {
                ConnectBeamToPoint(inRange.transform.position);
                if (valid) {
                    invalidIndicator.Stop();
                    invalidIndicator.Clear();
                    indicator.Play();
                }
                else {
                    indicator.Stop();
                    indicator.Clear();
                    invalidIndicator.Play();
                }
            }
            else {
                indicator.Stop();
                indicator.Clear();
                invalidIndicator.Stop();
                invalidIndicator.Clear();
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

    public override void ActivateTool(InputAction.CallbackContext context) {
        if (!context.performed) return;

        if (!pickedUpBody) {
            if (ValidBodyInRange(out Rigidbody2D toPickUp)) {
                PickUp(toPickUp);
            }
        }
        else {
            Blast();
        }
    }

    public override void Cancel() {
        if (pickedUpBody) {
            Detach();
        }
    }

    private void PickUp(Rigidbody2D body) {
        IBeamable[] beamables = body.GetComponents<IBeamable>();
        if (beamables != null) {
            foreach (IBeamable beamable in beamables) {
                if (!beamable.PickUp()) return;
                beamable.OnIllegalCollision += Detach;
            }
            pickedUpBody = body;
            container.transform.localScale = pickedUpBody.transform.localScale;
        }
    }

    private void Blast() {
        pickedUpBody.velocity = (Vector2)mount.up * firingVelocity;
        Detach();
    }

    private void Detach(IBeamable beamable) => Detach();
    private void Detach() {
        IBeamable[] beamables = pickedUpBody.GetComponents<IBeamable>();
        foreach (IBeamable beamable in beamables) {
            beamable.OnIllegalCollision -= Detach;
            beamable.Dropped();
        }
        pickedUpBody = null;
    }

    private void SetColliderDimensions() {
        inRangeCollider.size = new Vector2(width, range);
        inRangeCollider.offset = new Vector2(0, ((range + 0.23f) / 2));
    }

    private void SetBeamLength(float length) {
        Vector3 scale = spriteRenderer.transform.localScale;
        scale.y = length;
        spriteRenderer.transform.localScale = scale;
    }

    private void SetBeamRotation(float degrees) {
        Vector3 rotation = spriteRenderer.transform.localEulerAngles;
        rotation.z = degrees;
        spriteRenderer.transform.localEulerAngles = rotation;
    }

    private void ConnectBeamToPoint(Vector3 point) {
        Vector2 toPoint = point - transform.position;
        float distance = toPoint.magnitude;
        SetBeamLength(distance);
        float angle = Vector2.SignedAngle(mount.up, toPoint.normalized);
        SetBeamRotation(angle);
    }

    private bool ValidBodyInRange(out Rigidbody2D bodyInRange) {
        List<Collider2D> colldersInRange = new();
        inRangeCollider.GetContacts(colldersInRange);
        List<Rigidbody2D> bodiesInRange = colldersInRange
                                          .Where(col => col.GetComponent<IBeamable>() != null)
                                          .Select(col => col.attachedRigidbody)
                                          .ToList();
        bodyInRange = null;
        if (bodiesInRange.Count == 0) return false;

        bodiesInRange.Sort(CompareDistance);
        bodyInRange = bodiesInRange.First();

        if (bodyInRange.bodyType != RigidbodyType2D.Dynamic) return false;
        if (bodyInRange.mass >= massLimit) return false;
        return true;
    }

    // https://gamedev.stackexchange.com/questions/166811/sorting-a-list-of-objects-by-distance
    private int CompareDistance(Rigidbody2D r1, Rigidbody2D r2) {
        float squaredRangeA = (r1.transform.position - transform.position).sqrMagnitude;
        float squaredRangeB = (r2.transform.position - transform.position).sqrMagnitude;
        return squaredRangeA.CompareTo(squaredRangeB);
    }
}