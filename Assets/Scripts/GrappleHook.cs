using System;
using Unity.VisualScripting;
using UnityEngine;

public class GrappleHook : MonoBehaviour
{
    public event Action<Collision2D> OnHookCollision;

    private Rigidbody2D body;
    public LayerMask mask;

    public bool hooked => collidedObject;

    private Transform collidedObject;
    private Vector3 offsetFromHook;
    private Vector3 offsetFromCollided;

    // Start is called before the first frame update
    void Start() {
        body = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update() { }

    private void OnCollisionEnter2D(Collision2D other) {
        if (!mask.Contains(other.gameObject.layer)) return;

        if (!collidedObject) {
            collidedObject = other.transform;
            collidedObject.SetParent(transform);
            body.angularVelocity = 0;
            body.velocity *= 0.2f;

            Rigidbody2D collidedBody = collidedObject.GetComponent<Rigidbody2D>();
            collidedBody.simulated = false;
            body.mass += collidedBody.mass;

            CircleCollider2D collidedCollider = transform.GetChild(2).AddComponent<CircleCollider2D>();
            collidedCollider.transform.position = collidedObject.position;
            collidedCollider.transform.rotation = collidedObject.rotation;
            collidedCollider.transform.localScale = collidedObject.localScale;
            collidedCollider.radius = collidedObject.GetComponent<CircleCollider2D>().radius;
            OnHookCollision?.Invoke(other);
        }
    }

    public void Detach() {
        if (collidedObject) {
            Rigidbody2D collidedBody = collidedObject.GetComponent<Rigidbody2D>();
            body.mass -= collidedBody.mass;
            collidedBody.simulated = true;
            collidedObject.transform.SetParent(null);
            Destroy(transform.GetChild(2).GetComponent<PolygonCollider2D>());
            collidedBody.velocity = body.velocity;
        }
    }
}