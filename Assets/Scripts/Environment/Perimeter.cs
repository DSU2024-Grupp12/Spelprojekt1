using UnityEngine;
using UnityEngine.Events;

public class Perimeter : MonoBehaviour
{
    [SerializeField]
    private float perimeterRadius;

    [SerializeField]
    private Hull playerHull;

    [SerializeField]
    private float
        damageInterval,
        damageAmount;

    private float outsideTimer;
    private bool exitPerimeter;

    private ParticleSystem nebula;

    public UnityEvent OnExitPerimeter;

    void Start() {
        nebula = GetComponentInChildren<ParticleSystem>();
    }

    void Update() {
        if (playerHull) {
            float distanceFromCenter = playerHull.transform.position.magnitude;
            if (distanceFromCenter > perimeterRadius) {
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
                Debug.Log("deal perimeter damage");
                playerHull.TakeRawDamage(damageAmount * (distanceFromCenter - perimeterRadius));
                outsideTimer = 0f;
            }
        }
    }
}