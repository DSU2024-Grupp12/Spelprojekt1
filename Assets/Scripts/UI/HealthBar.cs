using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField]
    private SerializedInterface<IUIValueProvider<float>> providerObject;
    private IUIValueProvider<float> provider;

    [SerializeField]
    private PlayerReference playerRef;
    [SerializeField]
    private string shieldOrHull;

    [SerializeField]
    private bool hideWhenFull;

    private RectTransform rect;
    private Image image;

    private void Start() {
        if (providerObject.Defined()) provider = providerObject.extract;
        else {
            provider = playerRef.player.GetComponents<IUIValueProvider<float>>()
                                .Where(p => p.GetID() == shieldOrHull)
                                .First();
        }
        rect = GetComponent<RectTransform>();
        image = GetComponent<Image>();
    }

    private void LateUpdate() {
        Vector3 scale = rect.localScale;
        if (providerObject != null && provider != null) {
            scale.x = Mathf.Max(provider.CurrentValue() / provider.BaseValue(), 0);
            if (hideWhenFull && Mathf.Approximately(provider.CurrentValue(), provider.BaseValue())) {
                image.enabled = false;
            }
            else {
                image.enabled = true;
            }
        }
        else {
            scale.x = 0f;
        }
        rect.localScale = scale;
    }
}