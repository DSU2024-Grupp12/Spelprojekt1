using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class Asteroid : MonoBehaviour, IBeamable
{
    [HideInInspector]
    public AsteroidInfo info;

    [SerializeField]
    private ParticleSystem defaultExplosionPrefab;

    private Rigidbody2D body;

    private bool pickedUp;

    public float mass {
        get {
            if (body) return body.mass;
            else {
                body = GetComponent<Rigidbody2D>();
                if (body) return body.mass;
                else return 0f;
            }
        }
        set {
            if (body) {
                body.mass = value;
                AdjustScaleHullToMass();
            }
            else {
                body = GetComponent<Rigidbody2D>();
                if (body) {
                    body.mass = value;
                    AdjustScaleHullToMass();
                }
            }
        }
    }

    private void Start() {
        body = GetComponent<Rigidbody2D>();
        Vector2 initialDirection = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
        float magnitude = Random.Range(20f, 40f);
        float torque = Random.Range(-10f, 10f);

        body.AddForce(initialDirection * magnitude);
        body.AddTorque(torque);

        if (!info) info = ScriptableObject.CreateInstance<AsteroidInfo>();
    }

    public void Explode() {
        CreateExplosion(info.explosionPrefab ?? defaultExplosionPrefab);
        if (info.explodeInto != null && info.explodeInto.Length > 0) {
            int number = Random.Range(info.minExplodeInto, info.maxExplodeInto + 1);

            // create children 
            for (int i = 0; i < number; i++) {
                float minMag = 0.1f * transform.localScale.x;
                float maxMag = 0.3f * transform.localScale.x;
                Vector2 position = MathExtensions.GetRandomPolarCoordinate(minMag, maxMag, transform.position);
                AsteroidInfo randomChildInfo = info.explodeInto[Random.Range(0, info.explodeInto.Length)];
                Asteroid child = randomChildInfo.CreateAsteroid(position);
                Vector2 direction = (position - (Vector2)transform.position).normalized;
                child.GetComponent<Rigidbody2D>().AddForce(direction * body.mass * 20);
            }
        }
        Destroy(gameObject);
    }

    private void AdjustScaleHullToMass() {
        // adjust scale
        float cubeSquareScale = Mathf.Sqrt(body.mass / info.massAtScale1);
        transform.localScale = new Vector3(cubeSquareScale, cubeSquareScale, 1);

        // adjust hull strength
        Hull hull = GetComponent<Hull>();
        hull.SetHullStrength(mass * info.hullStrengthFactor);
    }
    /// <inheritdoc />
    public event Action<IBeamable> OnIllegalCollision;
    /// <inheritdoc />
    public bool PickUp() {
        pickedUp = true;
        body.angularVelocity = 0;
        body.velocity = Vector2.zero;
        return true;
    }
    /// <inheritdoc />
    public void Dropped() {
        pickedUp = false;
    }

    private void CreateExplosion(ParticleSystem prefab) {
        Vector3 explosionPosition = new Vector3(transform.position.x, transform.position.y, -3);
        ParticleSystem explosion = Instantiate(prefab, explosionPosition, transform.rotation);
        explosion.transform.localScale = transform.localScale;
    }

    private void OnCollisionEnter2D(Collision2D other) {
        if (other.gameObject.layer == LayerMask.NameToLayer("Obstacle") && pickedUp) {
            OnIllegalCollision?.Invoke(this);
        }
    }
}