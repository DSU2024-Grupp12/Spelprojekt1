using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraZoomControl : MonoBehaviour
{
    [SerializeField]
    private SerializedInterface<IUIValueProvider<float>> playerObject;
    private IUIValueProvider<float> playerShipSpeed;

    private float baseSize;
    [SerializeField]
    private float maxZoom, startZoomOutAtSpeed, timeUntilFullyZoomOut;

    private Camera cam;

    private float currentSize, timeSinceStartZoomOut, timeSinceStartZoomIn;
    private bool zoomingOut;

    private void Start() {
        playerShipSpeed = playerObject.extract;
        cam = GetComponent<Camera>();
        baseSize = cam.orthographicSize;
        currentSize = cam.orthographicSize;
    }

    private void Update() {
        if (!zoomingOut && playerShipSpeed.CurrentValue() > startZoomOutAtSpeed) {
            StopAllCoroutines();
            float time = Mathf.Min(Time.time - timeSinceStartZoomIn, timeUntilFullyZoomOut);
            StartCoroutine(SetCameraZoom(maxZoom, time));
            timeSinceStartZoomOut = Time.time;
            zoomingOut = true;
        }
        if (zoomingOut && playerShipSpeed.CurrentValue() < startZoomOutAtSpeed) {
            StopAllCoroutines();
            float time = Mathf.Min(Time.time - timeSinceStartZoomOut, timeUntilFullyZoomOut);
            StartCoroutine(SetCameraZoom(baseSize, time));
            timeSinceStartZoomIn = Time.time;
            zoomingOut = false;
        }
    }

    private void LateUpdate() {
        cam.orthographicSize = currentSize;
    }

    private IEnumerator SetCameraZoom(float size, float overSeconds) {
        float startTime = Time.time;
        float startSize = currentSize;
        while (!Mathf.Approximately(currentSize, size)) {
            float timePassed = Time.time - startTime;
            float factor = Mathf.Clamp01(timePassed / overSeconds);
            currentSize = Mathf.Lerp(startSize, size, factor);
            yield return null;
        }
        currentSize = size; // make sure the size was reached.
        if (Mathf.Approximately(currentSize, baseSize)) zoomingOut = false;
    }
}