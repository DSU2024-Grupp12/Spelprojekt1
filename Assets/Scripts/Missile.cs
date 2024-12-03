using UnityEngine;

[RequireComponent(typeof(Ship))]
public class Missile : MonoBehaviour, IExplodable
{
    public Transform target;
    [HideInInspector]
    public int createdByLayer;

    [SerializeField]
    private float inertTime, lifeTime, turningSpeed;

    [SerializeField]
    private Explosion explosionPrefab;

    [SerializeField]
    private LayerMask willExplodeOn;

    private float deathTime, activationTime;
    private Ship missile;

    private LayerMask rigidbodyExcludeMask;
    private bool createdByLayerNotExcludedInRigidBody;
    private bool createdByLayerInExplodeOn;

    // Start is called before the first frame update
    void Start() {
        deathTime = Time.time + lifeTime;
        activationTime = Time.time + inertTime;

        rigidbodyExcludeMask = GetComponent<Rigidbody2D>().excludeLayers;
        if (inertTime > 0) {
            createdByLayerInExplodeOn = willExplodeOn.Remove(createdByLayer);
            createdByLayerNotExcludedInRigidBody = rigidbodyExcludeMask.Add(createdByLayer);
        }
        GetComponent<Rigidbody2D>().excludeLayers = rigidbodyExcludeMask;

        missile = GetComponent<Ship>();
        missile.accelerating = true;
    }

    // Update is called once per frame
    void Update() {
        if (Time.time >= activationTime) {
            if (createdByLayerInExplodeOn) willExplodeOn.Add(createdByLayer);
            if (createdByLayerNotExcludedInRigidBody) {
                rigidbodyExcludeMask.Remove(createdByLayer);
                GetComponent<Rigidbody2D>().excludeLayers = rigidbodyExcludeMask;
            }
        }
        if (Time.time >= deathTime) {
            Explode();
        }
    }

    private void FixedUpdate() {
        Vector2 missileToTarget = target.transform.position - transform.position;
        Vector2 directionToTarget = missileToTarget.normalized;
        float angleToTarget = Vector2.SignedAngle(transform.up, directionToTarget);

        // turn towards player
        if (Mathf.Abs(angleToTarget) > turningSpeed * Time.fixedDeltaTime) {
            switch (Mathf.Sign(angleToTarget)) {
                case > 0:
                    transform.Rotate(Vector3.forward, turningSpeed * Time.fixedDeltaTime);
                    break;
                case < 0:
                    transform.Rotate(Vector3.forward, -turningSpeed * Time.fixedDeltaTime);
                    break;
            }
        }
        else {
            missile.turningClockwise = false;
            missile.turningCounterClockwise = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D other) {
        if (Time.time < activationTime) return;
        if (willExplodeOn.Contains(other.gameObject.layer)) {
            Explode();
        }
    }

    public void Explode() {
        if (explosionPrefab) {
            Debug.Log("Explode");
            Vector3 explosionPosition = new Vector3(transform.position.x, transform.position.y, -3);
            Instantiate(explosionPrefab, explosionPosition, transform.rotation);
            Destroy(gameObject);
        }
    }
}