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
    }

    public int GetCargoContent(Resource.Color color) {
        if (cargo.TryGetValue(color, out int value)) {
            return value;
        }
        else {
            return 0;
        }
    }

    public bool CheckBalance(int greenReq, int purpleReq) {
        bool hasEnough = true;

        if (GetCargoContent(Resource.Color.Green) < greenReq) hasEnough = false;
        if (GetCargoContent(Resource.Color.Purple) < purpleReq) hasEnough = false;

        return hasEnough;
    }

    /// <summary>
    /// Pays the amount specified if the player has at least that many resources in the hold.
    /// </summary>
    /// <returns>True if player payed, false otherwise</returns>
    public bool Pay(int green, int purple) {
        if (!CheckBalance(green, purple)) return false;
        if (cargo.ContainsKey(Resource.Color.Green)) cargo[Resource.Color.Green] -= green;
        if (cargo.ContainsKey(Resource.Color.Purple)) cargo[Resource.Color.Purple] -= purple;
        return true;
    }
}