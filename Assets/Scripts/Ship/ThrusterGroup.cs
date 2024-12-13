using UnityEngine;

[System.Serializable]
public class ThrusterGroup
{
    public ParticleSystem[] thrusters;

    public void Play() {
        foreach (ParticleSystem s in thrusters) {
            s.Play();
        }
    }

    public void Stop() {
        foreach (ParticleSystem s in thrusters) {
            s.Stop();
        }
    }
}