using UnityEngine;
using UnityEngine.Events;

public class Perimeter : MonoBehaviour
{
    [SerializeField]
    private AsteriodSpawner asteroidSpawner;

    [SerializeField]
    private Hull playerHull;

    [SerializeField]
    private float
        damageInterval,
        damageAmount;

    private float outsideTimer;
    private bool exitPerimeter;

    public UnityEvent OnExitPerimeter;

    void Start() {
        SetNebulaScale();
    }

    void Update() {
        if (playerHull) {
            float distanceFromCenter = playerHull.transform.position.magnitude;
            if (distanceFromCenter > asteroidSpawner.radius) {
                outsideTimer += Time.deltaTime;
                if (!exitPerimeter) {
                    exitPerimeter = true;
                    OnExitPerimeter?.Invoke();
                }
            }
            else {
                outsideTimer = 0f;
                exitPerimeter = false;
            }

            if (outsideTimer - damageInterval >= 0) {
                playerHull.TakeRawDamage(damageAmount, 0.AsLayerMask());
                outsideTimer = 0f;
            }
        }
    }

    public void SetNebulaScale() {
        Vector3 scale = transform.localScale;
        scale.x = asteroidSpawner.radius * 2;
        scale.y = asteroidSpawner.radius * 2;
        transform.localScale = scale;
    }
}