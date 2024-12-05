using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class DialogueManager : MonoBehaviour
{
    private SpeedSettings speedSettings;

    [SerializeField]
    private int maxConversationsInQueue;

    [SerializeField]
    private TMP_Text nameField, dialogueField;

    [SerializeField]
    private Animator dialogueBoxAnimator;

    private Queue<Line> currentLines;
    private Queue<string> currentSentences;
    private SortedList<int, Dialogue> queuedDialogues;

    private bool _d;
    private bool inDialogue {
        get => _d;
        set {
            _d = value;
            dialogueBoxAnimator.SetBool("InDialogue", _d);
        }
    }

    private void Start() {
        currentLines = new();
        currentSentences = new();
        queuedDialogues = new();
    }

    public void QueueDialogue(Dialogue dialogue) {
        Debug.Log("queue dialogue");

        if (queuedDialogues.Count == maxConversationsInQueue) return;
        // queue queueable dialogue or if there are no dialogues in queue, queue unqueueable dialogue
        if (dialogue.queueable || queuedDialogues.Count == 0) {
            queuedDialogues.Add(dialogue.priority, dialogue);
        }

        ProcessDialogueQueue();
    }

    private void ProcessDialogueQueue() {
        Debug.Log("process dialogue");
        if (!inDialogue) {
            Dialogue highestPriority = queuedDialogues[queuedDialogues.Count - 1];
            queuedDialogues.RemoveAt(queuedDialogues.Count - 1);
            StartConversation(highestPriority.conversation);
        }
    }

    private void StartConversation(Conversation conversation) {
        Debug.Log("start converstation");
        inDialogue = true;

        foreach (Line line in conversation.lines) {
            currentLines.Enqueue(line);
        }

        StartCoroutine(ProcessLines());
    }

    private IEnumerator ProcessLines() {
        Debug.Log("process lines");
        while (currentLines.Count > 0) {
            Line currentLine = currentLines.Dequeue();

            nameField.text = currentLine.speaker;
            foreach (string sentence in currentLine.sentences) {
                currentSentences.Enqueue(sentence);
            }
            yield return ProcessSentences();
        }

        EndDialogue();

        if (queuedDialogues.Count > 0) {
            yield return new WaitForSeconds(speedSettings.timeBetweenQueuedDialogue);
            ProcessDialogueQueue();
        }
    }

    public IEnumerator ProcessSentences() {
        Debug.Log("display next sentence");
        while (currentSentences.Count > 0) {
            yield return TypeSentence(currentSentences.Dequeue());
        }
        yield return new WaitForSeconds(speedSettings.timeBetweenSentences);
    }

    private IEnumerator TypeSentence(string sentence) {
        Debug.Log("type sentence");
        dialogueField.text = "";
        foreach (char c in sentence) {
            dialogueField.text += c;
            yield return new WaitForSeconds(speedSettings.typewriterSpeed);
        }
        yield return null;
    }

    public void EndDialogue() {
        Debug.Log("end dialogue");
        StopAllCoroutines();
        inDialogue = false;
        dialogueField.text = "";
        nameField.text = "";
    }
}