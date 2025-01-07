using UnityEngine;
using UnityEngine.Events;

public class OnTriggerEvents : MonoBehaviour
{
    [Tooltip("If checked, Enter and Exit events will only be called a maximum of one time, and Stay" +
             "will only be triggered inbetween the first enter and first exit.")]
    public bool oneTimeTrigger;

    private bool enterTriggered;
    private bool exitTriggered;
    
    public UnityEvent<Collider2D> Enter;
    public UnityEvent<Collider2D> Stay;
    public UnityEvent<Collider2D> Exit;

    private void OnTriggerEnter2D(Collider2D other) {
        if (oneTimeTrigger) {
            if (OnLayerPlayer(other) && !enterTriggered) {
                Enter?.Invoke(other);
                enterTriggered = true;
            } 
        } 
        else if (OnLayerPlayer(other)) Enter?.Invoke(other);
    }

    private void OnTriggerStay2D(Collider2D other) {
        if (oneTimeTrigger) {
            if (OnLayerPlayer(other) && !exitTriggered) Stay?.Invoke(other);
        }
        else if (OnLayerPlayer(other)) Stay?.Invoke(other);
    }

    private void OnTriggerExit2D(Collider2D other) {
        if (oneTimeTrigger) {
            if (OnLayerPlayer(other) && !exitTriggered) {
                Exit?.Invoke(other);
                exitTriggered = true;
            }
        }
        else if (OnLayerPlayer(other)) Exit?.Invoke(other);
    }

    private static bool OnLayerPlayer(Collider2D other) {
        return other.gameObject.layer == LayerMask.NameToLayer("PlayerShip");
    }
}