using UnityEngine;
using UnityEngine.Events;

public class DialogueFinishedListener : MonoBehaviour
{
    [SerializeField]
    private MessageListener[] listeners;

    // Start is called before the first frame update
    void Start() {
        FindObjectOfType<DialogueManager>().OnDialogueFinished.AddListener(InvokeEvent);
    }

    private void InvokeEvent(string message) {
        foreach (MessageListener messageListener in listeners) {
            if (messageListener.message == message) {
                messageListener.OnReceiveMessage?.Invoke(message);
            }
        }
    }
}

[System.Serializable]
public class MessageListener
{
    public string message;
    public UnityEvent<string> OnReceiveMessage;
}