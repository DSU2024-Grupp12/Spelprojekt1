using UnityEngine;

public interface IInteractable
{
    public void Interact();
    public void Highlight();
    public void Unhighlight();
    public static void LockPlayer(Rigidbody2D player) {
        player.velocity *= 0.01f;
        player.angularVelocity *= 0.01f;
    }
}