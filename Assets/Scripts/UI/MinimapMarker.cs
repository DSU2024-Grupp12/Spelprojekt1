using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class MinimapMarker : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer proxyPrefab;
    [SerializeField]
    private bool seperateProxySprite;
    [SerializeField, Tooltip("If this value is null the proxy will use the same sprite as the marker.")]
    private Sprite proxySprite;
    [SerializeField]
    private Color proxyColor;

    public bool disabled { get; private set; }
    public bool active = true;
    public bool useProxy = true;
    [Tooltip("If the distance to the center of the map (i.e. the player's location) " +
             "is less than this value the marker will be disabled. If the value is negative " +
             "the marker will not be disable when getting close to it.")]
    public float disableDistance = -1;

    public Transform currentProxy { get; private set; }

    private SpriteRenderer spriteRenderer;

    private void Start() {
        if (!Minimap.Exists) {
            Destroy(gameObject);
            return;
        }

        Minimap.AddMarker(this);
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (!active) spriteRenderer.enabled = false;
    }

    public void Activate() {
        spriteRenderer.enabled = true;
    }
    public void Deactivate() {
        spriteRenderer.enabled = false;
    }
    public void Disable() {
        disabled = true;
        DestroyProxy();
        Deactivate();
    }

    public void InstatiateProxy() {
        if (!currentProxy) {
            SpriteRenderer proxy = Instantiate(proxyPrefab, transform);
            proxy.sprite = seperateProxySprite ? proxySprite ?? spriteRenderer.sprite : spriteRenderer.sprite;
            proxy.color = seperateProxySprite ? proxyColor : spriteRenderer.color;
            currentProxy = proxy.transform;
        }
    }

    /// <summary>
    /// Destroys the marker's proxy if one exists
    /// </summary>
    public void DestroyProxy() {
        if (currentProxy) Destroy(currentProxy.gameObject);
    }
}