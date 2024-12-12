using UnityEngine;

[CreateAssetMenu(fileName = "New Dialogue", menuName = "Dialogue")]
public class Dialogue : ScriptableObject
{
    public SpeedSettings speedSettings;

    public int priority;
    public bool queueable;

    [Range(0, 1)]
    public float probability = 1f;

    public Conversation[] conversationVariations;

    public Conversation conversation {
        get {
            int randomIndex = Random.Range(0, conversationVariations.Length);
            return conversationVariations[randomIndex];
        }
    }
}

[System.Serializable]
public struct Conversation
{
    public Line[] lines;
}

[System.Serializable]
public struct Line
{
    public string speaker;

    [TextArea(1, 10)]
    public string[] sentences;
}

[System.Serializable]
public struct SpeedSettings
{
    public float
        typewriterSpeed,
        timeBetweenSentences,
        timeBetweenLines,
        timeBetweenQueuedDialogue;
}