using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class OnClickManager : MonoBehaviour
{
    public UnityEvent OnButtonClick;

    void Start() {
        foreach (Button button in FindObjectsOfType<Button>()) {
            button.onClick.AddListener(InvokeEvent);
        }
    }

    private void InvokeEvent() {
        OnButtonClick?.Invoke();
    }
}