using UnityEngine;

public class HealthBar : MonoBehaviour
{
    public Hull hull;

    private float fullHealth;
    private RectTransform rect;

    private void Start() {
        fullHealth = hull.fullStrength;
        rect = GetComponent<RectTransform>();
    }

    private void Update() {
        Vector3 scale = rect.localScale;
        if (hull) {
            scale.x = Mathf.Max(hull.currentStrength / fullHealth, 0);
        }
        else {
            scale.x = 0f;
        }
        rect.localScale = scale;
    }
}