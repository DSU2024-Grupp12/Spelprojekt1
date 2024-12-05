using System.Collections.Generic;
using System.Linq;
using UnityEditor.VersionControl;
using UnityEngine;

public class AudioPlayer : MonoBehaviour
{
    [SerializeField]
    private AudioAsset[] assets;

    private Dictionary<AudioAsset, AudioSource> sourcePool;

    private void Start() {
        sourcePool = new();
        foreach (AudioAsset asset in assets) {
            if (sourcePool.ContainsKey(asset)) continue;
            AudioSource source = gameObject.AddComponent<AudioSource>();
            source.enabled = false;
            asset.InitializeAudioSource(ref source);
            sourcePool.Add(asset, source);
        }
    }

    private void Update() {
        foreach (AudioSource source in sourcePool.Values) {
            if (!source.isPlaying) source.enabled = false;
        }
    }

    public void Play(string assetName) {
        AudioAsset asset = AssetByName(assetName);
        if (!sourcePool[asset].enabled) sourcePool[asset].enabled = true;
        if (asset.loop && sourcePool[asset].isPlaying) return;
        asset.Play(sourcePool[asset]);
    }

    public void PlayOneShot(string assetName) {
        AudioAsset asset = AssetByName(assetName);
        if (!sourcePool[asset].enabled) sourcePool[asset].enabled = true;
        if (asset.loop) {
            Debug.LogWarning(
                $"AudioAsset warning ({assetName}): Looping audio clips will not loop if played as one shots");
        }
        asset.PlayOneShot(sourcePool[asset]);
    }

    public void Stop(string assetName) {
        AudioAsset asset = AssetByName(assetName);
        asset.Stop(sourcePool[asset]);
        sourcePool[asset].enabled = false;
    }

    private AudioAsset AssetByName(string assetName) {
        return assets.Where(a => a.name == assetName).First();
    }
}