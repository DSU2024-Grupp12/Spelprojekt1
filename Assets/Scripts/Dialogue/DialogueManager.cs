using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    private SpeedSettings speedSettings;

    [SerializeField]
    private int maxConversationsInQueue;

    [SerializeField]
    private TMP_Text nameField, dialogueField;
    [SerializeField]
    private Image portrait;

    [SerializeField]
    private Animator dialogueBoxAnimator;

    [SerializeField]
    private AudioSource typewriterSource;
    [SerializeField]
    private AudioClip typewriterClip;

    public UnityEvent<string> OnDialogueFinished;

    private SortedList<int, Dialogue> queuedDialogues;

    private bool inDialogue {
        set {
            if (dialogueBoxAnimator) dialogueBoxAnimator.SetBool("InDialogue", value);
        }
    }

    private bool processingDialogue;

    private void Start() {
        queuedDialogues = new SortedList<int, Dialogue>();
    }

    private void Update() {
        if (queuedDialogues.Count > 0 && !processingDialogue) {
            StartCoroutine(ProcessDialogueQueue());
        }
    }

    public void QueueDialogue(Dialogue dialogue) {
        if (queuedDialogues.Count == maxConversationsInQueue) return;
        // queue queueable dialogue or if there are no dialogues in queue, queue unqueueable dialogue
        if (dialogue.queueable || (queuedDialogues.Count == 0 && !processingDialogue)) {
            if (new System.Random().NextDouble() <= dialogue.probability) {
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
            OnDialogueFinished?.Invoke(highestPriority.onFinishMessage);
            yield return new WaitForSecondsRealtime(speedSettings.endOfDialogueLingerTime);
            EndDialogue();
            yield return new WaitForSecondsRealtime(speedSettings.timeBetweenQueuedDialogue);
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
            yield return new WaitForSecondsRealtime(speedSettings.timeBetweenLines);
        }
    }

    private IEnumerator ProcessLine(Line line) {
        if (nameField) nameField.text = line.speaker;
        if (portrait) portrait.sprite = line.portrait;

        Queue<string> sentences = new();
        foreach (string sentence in line.sentences) {
            sentences.Enqueue(sentence);
        }

        while (sentences.Count > 0) {
            yield return ProcessSentences(sentences.Dequeue());
            yield return new WaitForSecondsRealtime(speedSettings.timeBetweenSentences);
        }
    }

    public IEnumerator ProcessSentences(string sentence) {
        yield return TypeSentence(sentence);
    }

    private IEnumerator TypeSentence(string sentence) {
        dialogueField.text = "";
        foreach (char c in sentence) {
            dialogueField.text += c;
            if (c != ' ') typewriterSource.PlayOneShot(typewriterClip);
            yield return new WaitForSecondsRealtime(speedSettings.typewriterSpeed);
        }
    }

    public void EndDialogue() {
        inDialogue = false;
        dialogueField.text = "";
        if (nameField) nameField.text = "";
    }

    public void StartDialogue() {
        inDialogue = true;
    }
}