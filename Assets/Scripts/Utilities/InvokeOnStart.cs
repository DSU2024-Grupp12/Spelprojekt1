using UnityEngine;
using UnityEngine.Events;

public class InvokeOnStart : MonoBehaviour
{
    public UnityEvent OnStart;

    void Start() {
        OnStart?.Invoke();
    }
}