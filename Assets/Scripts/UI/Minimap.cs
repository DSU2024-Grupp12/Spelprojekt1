using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class Minimap : MonoBehaviour
{
    public static bool Exists { get; private set; }

    [SerializeField, Min(0), Tooltip("The distance from the center of the map that markers whose real " +
                                     "position are outside the map will be placed." +
                                     "0 is the center of the map and 1 is the very edge of the map. A value greater " +
                                     "than 1 will put the marker outside the map.")]
    private float outsideScreenProxyDistance = 0.9f;

    private static List<MinimapMarker> markers;

    private float radius;
    [SerializeField]
    private Camera minimapCamera;
    [SerializeField]
    private Canvas canvas;

    [SerializeField]
    private float largeSize, largeCameraSize;
    private Vector2 smallSize;
    private Vector3 smallPosition;
    private float smallCameraSize;

    private RectTransform rect;

    private void Awake() {
        markers = new();
        Exists = true;
    }

    void Start() {
        radius = minimapCamera.orthographicSize;
        rect = GetComponent<RectTransform>();
        smallSize = rect.sizeDelta;
        smallPosition = rect.anchoredPosition;
        smallCameraSize = minimapCamera.orthographicSize;
    }

    void Update() {
        foreach (MinimapMarker marker in markers) {
            if (marker.disabled) continue;

            Vector2 toMarker = marker.transform.position - minimapCamera.transform.position;

            if (outsideScreenProxyDistance > 1) {
                if (toMarker.magnitude > radius) {
                    if (marker.useProxy) {
                        if (!marker.currentProxy) marker.InstatiateProxy();
                        marker.currentProxy.position = GetProxyPosition(toMarker.normalized);
                    }
                }
            }
            else if (toMarker.magnitude > radius * outsideScreenProxyDistance) {
                if (marker.useProxy) {
                    if (!marker.currentProxy) marker.InstatiateProxy();
                    if (marker.currentProxy) marker.currentProxy.position = GetProxyPosition(toMarker.normalized);
                    marker.Hide();
                }
            }
            else {
                if (marker.active) marker.Unhide();
                else marker.Hide();
                marker.DestroyProxy();
            }
            if (toMarker.magnitude < marker.disableDistance) marker.Disable();
        }
        // destroy all disabled markers and remove them from markers
        markers.Where(m => m.disabled).ToList().ForEach(m => Destroy(m.gameObject));
        markers.RemoveAll(m => m.disabled);
    }

    private Vector2 GetProxyPosition(Vector2 direction) {
        return (Vector2)minimapCamera.transform.position +
               direction * (radius * outsideScreenProxyDistance);
    }

    public static void AddMarker(MinimapMarker marker) {
        markers.Add(marker);
    }
    public static void RemoveMarker(MinimapMarker marker) {
        markers.Remove(marker);
    }

    public void HoldLarge(InputAction.CallbackContext context) {
        if (context.performed) {
            rect.sizeDelta = new Vector2(largeSize, largeSize);
            rect.anchoredPosition = new Vector3(
                -canvas.pixelRect.width / 2,
                canvas.pixelRect.height / 2,
                smallPosition.z
            );
            minimapCamera.orthographicSize = largeCameraSize;
            radius = largeCameraSize;
        }
        if (context.canceled) {
            rect.sizeDelta = smallSize;
            rect.anchoredPosition = smallPosition;
            minimapCamera.orthographicSize = smallCameraSize;
            radius = smallCameraSize;
        }
    }

    public static MinimapMarker PlaceMarker(MinimapMarker prefab, Vector3 position) {
        return PlaceMarker(prefab, null, position);
    }
    public static MinimapMarker PlaceMarker(MinimapMarker prefab, Transform parent) {
        return PlaceMarker(prefab, parent, parent.position);
    }
    public static MinimapMarker PlaceMarker(MinimapMarker prefab, Transform parent, Vector3 position) {
        MinimapMarker marker = Instantiate(prefab, parent);
        marker.transform.position = position;
        return marker;
    }
}