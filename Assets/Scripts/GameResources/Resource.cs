using UnityEngine;

public class Resource : MonoBehaviour
{
    public Color resourceColor;
    [HideInInspector]
    public int value;

    public float attractionVelocity;

    private CargoHold target;

    public void CollectIn(CargoHold cargo) {
        cargo.CollectResource(resourceColor, value);
        Destroy(gameObject);
    }

    public enum Color
    {
        Green,
        Purple
    }
}