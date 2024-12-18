using System.Linq;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    [SerializeField]
    private SerializedInterface<IUIValueProvider<float>> providerObject;
    private IUIValueProvider<float> provider;

    [SerializeField]
    private PlayerReference playerRef;
    [SerializeField]
    private string shieldOrHull;

    private RectTransform rect;

    private void Start() {
        if (providerObject.Defined()) provider = providerObject.extract;
        else {
            provider = playerRef.player.GetComponents<IUIValueProvider<float>>()
                                .Where(p => p.GetID() == shieldOrHull)
                                .First();
        }
        rect = GetComponent<RectTransform>();
    }

    private void Update() {
        Vector3 scale = rect.localScale;
        if (providerObject != null && provider != null) {
            scale.x = Mathf.Max(provider.CurrentValue() / provider.BaseValue(), 0);
        }
        else {
            scale.x = 0f;
        }
        rect.localScale = scale;
    }
}