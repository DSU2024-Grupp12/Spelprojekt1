using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(fileName = "New Audio Asset", menuName = "Audio Asset", order = 21)]
public class AudioAsset : ScriptableObject
{
    public new string name;

    public AudioClip[] clips;
    [HideInInspector]
    public AudioClip loopedOneShotClip;
    public AudioMixerGroup output;

    public bool playOnAwake;
    public bool loop;

    [Range(0, 256)]
    public int priority = 128;

    [Range(0, 1)]
    public float volume = 1;

    [Range(-3, 3)]
    public float pitch = 1;
    [Tooltip(
        "When random pitch is selected the pitch will randomised between the " +
        "Min Pitch Variation and Max Pitch Variation values"
    )]
    public bool randomPitch;
    [Range(-3, 3)]
    public float minPitchVariation = 1f, maxPitchVariation = 1f;

    // [Range(-1, 1), Tooltip("-1 is only left, 1 is only right")]
    // public float stereoPan;

    // [Range(0, 1), Tooltip("0 is completely 2D, 1 is completely 3D")]
    // public float spatialBlend;

    public void Play(AudioSource audioSource) {
        RandomizeClip(ref audioSource);
        audioSource.Play();
    }
    public void PlayOneShot(AudioSource audioSource) {
        RandomizeClip(ref audioSource);
        audioSource.PlayOneShot(audioSource.clip);
    }
    public void Stop(AudioSource audioSource) {
        audioSource.Stop();
    }
    public void InitializeAudioSource(ref AudioSource audioSource) {
        audioSource.clip = clips[Random.Range(0, clips.Length)];
        audioSource.outputAudioMixerGroup = output;
        audioSource.playOnAwake = playOnAwake;
        audioSource.loop = loop;
        audioSource.priority = priority;
        audioSource.volume = volume;
        if (randomPitch) {
            audioSource.pitch = Random.Range(minPitchVariation, maxPitchVariation);
        }
        else {
            audioSource.pitch = pitch;
        }
    }
    public void RandomizeClip(ref AudioSource audioSource) {
        audioSource.clip = clips[Random.Range(0, clips.Length)];
        if (randomPitch) {
            audioSource.pitch = Random.Range(minPitchVariation, maxPitchVariation);
        }
    }
}