using UnityEngine;
using UnityEngine.Events;

public class OnTriggerEvents : MonoBehaviour
{
    public UnityEvent<Collider2D> Enter;
    public UnityEvent<Collider2D> Stay;
    public UnityEvent<Collider2D> Exit;

    private void OnTriggerEnter2D(Collider2D other) {
        Enter?.Invoke(other);
    }

    private void OnTriggerStay2D(Collider2D other) {
        Stay?.Invoke(other);
    }

    private void OnTriggerExit2D(Collider2D other) {
        Stay?.Invoke(other);
    }
}