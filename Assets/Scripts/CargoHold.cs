using System.Collections.Generic;
using UnityEngine;

public class CargoHold : MonoBehaviour
{
    private Dictionary<Resource.Color, int> cargo;

    private void Start() {
        cargo = new();
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.layer == LayerMask.NameToLayer("Resource")) {
            Resource resource = other.GetComponent<Resource>();
            resource.Attract(this);
        }
    }

    public void CollectResource(Resource.Color color, int value) {
        if (!cargo.TryAdd(color, value)) {
            cargo[color] += value;
        }

        Debug.Log($"Color: {color.ToString()}, value: {cargo[color]}");
    }
}