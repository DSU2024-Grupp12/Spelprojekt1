using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public void QueueDialogue(Dialogue dialogue) {
        FindObjectOfType<DialogueManager>().QueueDialogue(dialogue);
    }

    public void EndDialogue() {
        FindObjectOfType<DialogueManager>().EndDialogue();
    }
}