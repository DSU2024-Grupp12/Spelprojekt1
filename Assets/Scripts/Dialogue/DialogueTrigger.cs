using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public void StartDialogue(Dialogue dialogue) {
        FindObjectOfType<DialogueManager>().QueueDialogue(dialogue);
    }

    public void EndDialogue() {
        FindObjectOfType<DialogueManager>().EndDialogue();
    }
}