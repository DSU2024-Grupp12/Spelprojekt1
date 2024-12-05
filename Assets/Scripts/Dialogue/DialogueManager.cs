using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    private SpeedSettings speedSettings;

    [SerializeField]
    private int maxConversationsInQueue;

    [SerializeField]
    private TMP_Text nameField, dialogueField;

    [SerializeField]
    private Animator dialogueBoxAnimator;

    private SortedList<int, Dialogue> queuedDialogues;

    private bool inDialogue {
        get => dialogueBoxAnimator.GetBool("InDialogue");
        set => dialogueBoxAnimator.SetBool("InDialogue", value);
    }

    private bool processingDialogue;

    private void Start() {
        queuedDialogues = new();
    }

    private void Update() {
        if (queuedDialogues.Count > 0 && !processingDialogue) {
            StartCoroutine(ProcessDialogueQueue());
        }
    }

    public void QueueDialogue(Dialogue dialogue) {
        if (queuedDialogues.Count == maxConversationsInQueue) return;
        // queue queueable dialogue or if there are no dialogues in queue, queue unqueueable dialogue
        if (dialogue.queueable || queuedDialogues.Count == 0) {
            if (new System.Random().NextDouble() < dialogue.probability) {
                queuedDialogues.Add(dialogue.priority, dialogue);
            }
        }
    }

    private IEnumerator ProcessDialogueQueue() {
        processingDialogue = true;

        while (queuedDialogues.Count > 0) {
            StartDialogue();
            Dialogue highestPriority = queuedDialogues[queuedDialogues.Count - 1];
            queuedDialogues.RemoveAt(queuedDialogues.Count - 1);
            speedSettings = highestPriority.speedSettings;
            yield return ProcessConversation(highestPriority.conversation);
            EndDialogue();
            yield return new WaitForSeconds(speedSettings.timeBetweenQueuedDialogue);
        }

        processingDialogue = false;
    }

    private IEnumerator ProcessConversation(Conversation conversation) {
        Queue<Line> lines = new Queue<Line>();
        foreach (Line line in conversation.lines) {
            lines.Enqueue(line);
        }

        while (lines.Count > 0) {
            yield return ProcessLine(lines.Dequeue());
            yield return new WaitForSeconds(speedSettings.timeBetweenLines);
        }
    }

    private IEnumerator ProcessLine(Line line) {
        nameField.text = line.speaker;

        Queue<string> sentences = new();
        foreach (string sentence in line.sentences) {
            sentences.Enqueue(sentence);
        }

        while (sentences.Count > 0) {
            yield return ProcessSentences(sentences.Dequeue());
            yield return new WaitForSeconds(speedSettings.timeBetweenSentences);
        }
    }

    public IEnumerator ProcessSentences(string sentence) {
        yield return TypeSentence(sentence);
    }

    private IEnumerator TypeSentence(string sentence) {
        dialogueField.text = "";
        foreach (char c in sentence) {
            dialogueField.text += c;
            yield return new WaitForSeconds(speedSettings.typewriterSpeed);
        }
    }

    public void EndDialogue() {
        inDialogue = false;
        dialogueField.text = "";
        nameField.text = "";
    }

    public void StartDialogue() {
        inDialogue = true;
    }
}