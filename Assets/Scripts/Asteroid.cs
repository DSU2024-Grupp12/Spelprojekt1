using UnityEngine;

public class Asteroid : MonoBehaviour, IBeamable
{
    public ParticleSystem explosionPrefab;
    [Tooltip("The hull strength of the asteroid will be equal to its mass multiplied by this factor")]
    public float hullStrengthFactor;

    private Rigidbody2D body;

    private void Start() {
        body = GetComponent<Rigidbody2D>();

        Vector2 initialDirection = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
        float magnitude = Random.Range(20f, 40f);
        float torque = Random.Range(-10f, 10f);

        body.AddForce(initialDirection * magnitude);
        body.AddTorque(torque);
    }

    public void Explode() {
        if (explosionPrefab) {
            Vector3 explosionPosition = new Vector3(transform.position.x, transform.position.y, -3);
            ParticleSystem explosion = Instantiate(explosionPrefab, explosionPosition, transform.rotation);
            explosion.transform.localScale = transform.localScale;
            Destroy(gameObject);
        }
    }
    /// <inheritdoc />
    public bool PickUp() {
        body.angularVelocity = 0;
        body.velocity = Vector2.zero;
        return true;
    }
    /// <inheritdoc />
    public void Dropped() { }
}