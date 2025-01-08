using UnityEngine;
using UnityEngine.InputSystem;

public class DialogueTrigger : MonoBehaviour
{
    private DialogueManager manager;

    private void Awake() {
        manager = FindObjectOfType<DialogueManager>();
    }

    public void QueueDialogue(Dialogue dialogue) {
        manager.QueueDialogue(dialogue);
    }

    public void Skip(InputAction.CallbackContext context) {
        if (context.performed) manager.SkipToEndOfLine();
    }

    public void SetSkippable(bool value) {
        manager.SetSkippable(value);
    }

    public void EndDialogue() {
        FindObjectOfType<DialogueManager>().EndDialogue();
    }
}