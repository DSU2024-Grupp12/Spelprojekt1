using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : MonoBehaviour, IBeamable
{
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
        Debug.Log($"Exploded: {gameObject.GetInstanceID()}");
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