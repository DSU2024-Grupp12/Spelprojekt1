using UnityEngine;
using UnityEngine.Events;

public class OnTriggerEvents : MonoBehaviour
{
    public UnityEvent<Collider2D> Enter;
    public UnityEvent<Collider2D> Stay;
    public UnityEvent<Collider2D> Exit;

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.layer == LayerMask.NameToLayer("PlayerShip")) Enter?.Invoke(other);
    }

    private void OnTriggerStay2D(Collider2D other) {
        if (other.gameObject.layer == LayerMask.NameToLayer("PlayerShip")) Stay?.Invoke(other);
    }

    private void OnTriggerExit2D(Collider2D other) {
        if (other.gameObject.layer == LayerMask.NameToLayer("PlayerShip")) Exit?.Invoke(other);
    }
}