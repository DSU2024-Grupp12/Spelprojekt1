using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    private SpeedSettings speedSettings;
    private bool skippable;

    [SerializeField]
    private int maxConversationsInQueue;

    [SerializeField]
    private TMP_Text nameField, dialogueField;
    [SerializeField]
    private Image portrait;

    [SerializeField]
    private Animator dialogueBoxAnimator;

    public UnityEvent Typewriter;

    public UnityEvent<string> OnDialogueFinished;

    private SortedList<int, Dialogue> queuedDialogues;

    private bool inDialogue {
        set {
            if (dialogueBoxAnimator) dialogueBoxAnimator.SetBool("InDialogue", value);
        }
    }

    private bool processingDialogue;
    private bool skipToEndOfLine;
    private bool interrupt;

    private void Awake() {
        queuedDialogues = new SortedList<int, Dialogue>();
    }

    private void Update() {
        if (queuedDialogues.Count > 0 && !processingDialogue) {
            StartCoroutine(ProcessDialogueQueue());
        }
    }

    public void QueueDialogue(Dialogue dialogue) {
        if (dialogue.interrupt) {
            interrupt = true;
            if (queuedDialogues.Count > 0 && queuedDialogues.Last().Value.interrupt) {
                queuedDialogues.RemoveAt(queuedDialogues.Count - 1);
            }
            queuedDialogues.Add(int.MaxValue, dialogue);
            return;
        }

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
            interrupt = false;
            StartDialogue();
            Dialogue highestPriority = queuedDialogues.Last().Value;
            queuedDialogues.RemoveAt(queuedDialogues.Count - 1);
            speedSettings = highestPriority.speedSettings;
            yield return ProcessConversation(highestPriority.conversation);
            OnDialogueFinished?.Invoke(highestPriority.onFinishMessage);
            yield return new WaitForSecondsRealtime(interrupt ? 0f : speedSettings.endOfDialogueLingerTime);
            EndDialogue();
            yield return new WaitForSecondsRealtime(interrupt ? 0f : speedSettings.timeBetweenQueuedDialogue);
        }

        processingDialogue = false;
    }

    private IEnumerator ProcessConversation(Conversation conversation) {
        Queue<Line> lines = new Queue<Line>();
        foreach (Line line in conversation.lines) {
            lines.Enqueue(line);
        }

        while (lines.Count > 0 && !interrupt) {
            yield return ProcessLine(lines.Dequeue());
            if (!interrupt) {
                yield return new WaitForRealSecondsOrSkip(speedSettings.timeBetweenLines, () => skipToEndOfLine);
            }
        }
    }

    private IEnumerator ProcessLine(Line line) {
        if (nameField) nameField.text = line.speaker;
        if (portrait) portrait.sprite = line.portrait;

        Queue<string> sentences = new();
        foreach (string sentence in line.sentences) {
            sentences.Enqueue(sentence);
        }

        while (sentences.Count > 0 && !interrupt) {
            yield return TypeSentence(sentences.Dequeue());
            if (!interrupt) {
                yield return new WaitForRealSecondsOrSkip(speedSettings.timeBetweenLines, () => skipToEndOfLine);
            }
        }
    }

    private IEnumerator TypeSentence(string sentence) {
        skipToEndOfLine = false;
        dialogueField.text = "";
        foreach (char c in sentence.TakeWhile(_ => !skipToEndOfLine)) {
            dialogueField.text += c;
            if (c != ' ') Typewriter?.Invoke();
            yield return new WaitForRealSecondsOrSkip(speedSettings.typewriterSpeed, () => interrupt);
            if (interrupt) break;
        }
        if (skipToEndOfLine) {
            dialogueField.text = sentence;
        }
        skipToEndOfLine = false;
    }

    public void EndDialogue() {
        inDialogue = false;
        dialogueField.text = "";
        if (nameField) nameField.text = "";
    }

    public void StartDialogue() {
        inDialogue = true;
    }

    public void SkipToEndOfLine() {
        if (processingDialogue && skippable) skipToEndOfLine = true;
    }

    public void SetSkippable(bool value) {
        skippable = value;
    }

    private class WaitForRealSecondsOrSkip : CustomYieldInstruction
    {
        private readonly float breakTime;
        private readonly Func<bool> shouldSkip;

        public WaitForRealSecondsOrSkip(float time, Func<bool> skipPredicate) {
            breakTime = Time.realtimeSinceStartup + time;
            shouldSkip = skipPredicate;
        }

        /// <inheritdoc />
        public override bool keepWaiting => Time.realtimeSinceStartup < breakTime && !shouldSkip();
    }
}