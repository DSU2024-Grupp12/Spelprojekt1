using UnityEngine;

public class HealthBar : MonoBehaviour
{
    [SerializeField]
    private SerializedInterface<IUIValueProvider<float>> providerObject;
    private IUIValueProvider<float> provider;

    private RectTransform rect;

    private void Start() {
        provider = providerObject.extract;
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