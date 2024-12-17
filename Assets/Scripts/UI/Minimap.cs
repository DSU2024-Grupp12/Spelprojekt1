using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
    private Camera minimapCamera;

    private void Awake() {
        markers = new();
        Exists = true;
    }

    void Start() {
        minimapCamera = GetComponentInChildren<Camera>();
        radius = minimapCamera.orthographicSize;
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
                        marker.Deactivate();
                    }
                }
            }
            else if (toMarker.magnitude > radius * outsideScreenProxyDistance) {
                if (marker.useProxy) {
                    if (!marker.currentProxy) marker.InstatiateProxy();
                    marker.currentProxy.position = GetProxyPosition(toMarker.normalized);
                    marker.Deactivate();
                }
            }
            else {
                marker.Activate();
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