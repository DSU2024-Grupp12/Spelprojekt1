using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class Explosion : MonoBehaviour
{
    private ParticleSystem explosion;

    void Start() {
        explosion = GetComponent<ParticleSystem>();
        explosion.Play();
    }

    void Update() {
        if (!explosion.IsAlive()) Destroy(this);
    }
}