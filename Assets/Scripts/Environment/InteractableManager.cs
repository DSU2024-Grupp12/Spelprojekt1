using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InteractableManager : MonoBehaviour
{
    private static List<IInteractable> Interactables;

    public static InteractableManager Instance;

    void Awake() {
        if (Instance) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        Interactables = new();
    }

    public void Interact(InputAction.CallbackContext context) {
        if (context.performed) {
            if (Interactables.Count > 0) {
                Interactables[0].Interact();
            }
        }
    }

    public static void QueueInteractable(IInteractable interactable) {
        if (!Interactables.Contains(interactable)) {
            Interactables.Add(interactable);
            if (Interactables.Count == 1) {
                // if this is the only element then highlight it
                interactable.Highlight();
            }
        }
    }

    public static void RemoveInteractableFromQueue(IInteractable interactable) {
        if (Interactables.Contains(interactable)) {
            if (Interactables.IndexOf(interactable) == 0) {
                interactable.Unhighlight();
                if (Interactables.Count > 1) {
                    Interactables[1].Highlight();
                }
            }
            Interactables.Remove(interactable);
        }
    }
}