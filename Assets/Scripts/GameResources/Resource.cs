using UnityEngine;

public class Resource : MonoBehaviour
{
    public Color resourceColor;
    [HideInInspector]
    public int value;

    public float attractionVelocity;

    private CargoHold target;

    private void Update() {
        if (target) {
            Vector2 toTarget = target.transform.position - transform.position;

            if (toTarget.magnitude < 0.2f) {
                CollectIn(target);
                return;
            }

            transform.Translate(toTarget.normalized * (attractionVelocity * Time.deltaTime));
        }
    }

    public void Attract(CargoHold pickedUpByCargoHold) {
        if (!target) target = pickedUpByCargoHold;
    }

    private void CollectIn(CargoHold cargo) {
        cargo.CollectResource(resourceColor, value);
        Destroy(gameObject);
    }

    public enum Color
    {
        Green,
        Purple
    }
}