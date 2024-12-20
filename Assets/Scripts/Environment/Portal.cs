using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Portal : MonoBehaviour, IInteractable
{
    [SerializeField]
    private Canvas highlightPrompt;

    [SerializeField, TextArea(3, 5)]
    private string description;

    [SerializeField]
    private string teleportToSceneName;

    [SerializeField]
    private float teleportDelay;

    private bool teleportActivated;
    private Rigidbody2D player;

    private void Start() {
        Unhighlight();
    }

    public void Interact() {
        if (teleportActivated) return;
        IInteractable.LockPlayer(player);
        MenuManager.Instance.OpenMenu("PortalMenu", BuildPortalMenuInfo());
        MenuManager.OnReturnToGameplay += ClosePortalMenu;
        Unhighlight();
    }
    /// <inheritdoc />
    public void Highlight() {
        if (teleportActivated) return;
        highlightPrompt.enabled = true;
    }
    /// <inheritdoc />
    public void Unhighlight() {
        highlightPrompt.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.layer == LayerMask.NameToLayer("PlayerShip")) {
            InteractableManager.QueueInteractable(this);
            if (other.gameObject.GetComponent<Rigidbody2D>()) player = other.gameObject.GetComponent<Rigidbody2D>();
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if (other.gameObject.layer == LayerMask.NameToLayer("PlayerShip")) {
            InteractableManager.RemoveInteractableFromQueue(this);
            player = null;
        }
    }

    private MenuInfo BuildPortalMenuInfo() {
        MenuInfo info = new MenuInfo();
        info.AddEntry("Description", description);
        info.AddEntry("YesButton", TeleportToSceneDelegate());
        return info;
    }

    private UnityAction TeleportToSceneDelegate() {
        return () => {
            ApplicationHandler.ChangeSceneDelayed(teleportToSceneName, teleportDelay);
            teleportActivated = true;
            MenuManager.Instance.ReturnToGameplay();
        };
    }

    private void ClosePortalMenu() {
        Highlight();
        MenuManager.OnReturnToGameplay -= ClosePortalMenu;
    }
}