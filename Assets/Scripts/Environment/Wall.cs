using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour, IElectrical
{
    [SerializeField]
    private Rigidbody2D[] connectedBodies;

    void Start() {
        foreach (Rigidbody2D body in connectedBodies) {
            body.transform.SetParent(transform);
            body.bodyType = RigidbodyType2D.Kinematic;
        }
    }

    public void Disable() {
        foreach (Rigidbody2D body in connectedBodies) {
            body.transform.SetParent(null);
            body.bodyType = RigidbodyType2D.Dynamic;
        }

        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<Rigidbody2D>().simulated = false;
        GetComponent<BoxCollider2D>().enabled = false;
    }
    /// <inheritdoc />
    public string GetID() {
        return "Wall";
    }
}