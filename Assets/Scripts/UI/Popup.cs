using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Canvas))]
public class Popup : MonoBehaviour
{
    private static Popup instance;
    private static Canvas canvas;
    private static TMP_Text textField;
    private static Image container;
    private static Color containerBaseColor;
    private static float lifeTime;
    private static float deathTime;

    [SerializeField]
    private TMP_Text popupTextElement;

    void Awake() {
        if (instance) {
            Destroy(gameObject);
            return;
        }
        instance = this;

        canvas = GetComponent<Canvas>();
        textField = popupTextElement;
        container = textField.GetComponentInParent<Image>();
        containerBaseColor = container.color;
        canvas.enabled = false;
    }

    void Update() {
        if (canvas.enabled) {
            float timeRemaining = deathTime - Time.time;
            SetTextAlpha(timeRemaining / lifeTime);
            if (timeRemaining <= 0) {
                canvas.enabled = false;
            }
        }
    }

    public static void Display(string message, float length) {
        canvas.enabled = true;
        textField.text = message;
        SetTextAlpha(255);
        lifeTime = length;
        deathTime = Time.time + lifeTime;
    }

    private static void SetTextAlpha(float alpha) {
        textField.alpha = alpha;
        container.color = new Color(
            containerBaseColor.r,
            containerBaseColor.g,
            containerBaseColor.b,
            Mathf.Lerp(0, containerBaseColor.a, alpha)
        );
    }
}