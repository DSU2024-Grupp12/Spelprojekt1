using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Cloud : MonoBehaviour
{
    private Dictionary<Collider2D, float> currentlyClouding;

    [SerializeField]
    private LayerMask affectedLayers;

    [SerializeField]
    private float damageInterval;
    [SerializeField]
    private float damage;

    [HideInInspector]
    public bool containsPlayer;

    private void Start() {
        currentlyClouding = new();
    }

    private void Update() {
        Collider2D[] cols = currentlyClouding.Keys.ToArray();
        foreach (Collider2D col in cols) {
            if (!col) { // if the collider has been destroyed from an external source we remove it
                currentlyClouding.Remove(col);
                continue;
            }
            if (Time.time >= currentlyClouding[col]) {
                Hull hull = col.GetComponent<Hull>();
                if (hull.TakeDamage(damage, gameObject.layer)) {
                    // if the hull explodes from the damage we remove it from the dictionary
                    currentlyClouding.Remove(col);
                    continue;
                }
                currentlyClouding[col] = Time.time + damageInterval;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (affectedLayers.Contains(other.gameObject.layer) && other.HasComponent(out Hull hull)) {
            if (!hull.inCloud) { // if a hull is already in a cloud when it enters this cloud, don't add it
                hull.inCloud = true;
                currentlyClouding.TryAdd(other, Time.time + damageInterval);
            }
        }
        if (other.gameObject.layer == LayerMask.NameToLayer("PlayerShip")) {
            containsPlayer = true;
        }
    }

    private void OnTriggerStay2D(Collider2D other) {
        if (affectedLayers.Contains(other.gameObject.layer) && other.HasComponent(out Hull hull)) {
            if (!hull.inCloud) { // if a hull exits another cloud while inside this cloud, add it to the dict
                hull.inCloud = true;
                currentlyClouding.TryAdd(other, Time.time + damageInterval);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if (currentlyClouding.ContainsKey(other)) {
            other.GetComponent<Hull>().inCloud = false;
            currentlyClouding.Remove(other);
        }
        if (other.gameObject.layer == LayerMask.NameToLayer("PlayerShip")) {
            containsPlayer = false;
        }
    }
}