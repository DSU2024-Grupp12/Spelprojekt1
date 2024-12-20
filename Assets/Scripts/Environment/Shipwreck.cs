using UnityEngine;

public class Shipwreck : MonoBehaviour, IInteractable
{
    [SerializeField]
    private Log log;

    [SerializeField]
    private Canvas highlighPrompt;

    [SerializeField]
    private SpriteRenderer sprite;

    private Rigidbody2D player;

    private void Start() {
        Unhighlight();
        if (sprite) sprite.transform.Rotate(0, 0, Random.Range(0f, 360f));
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

    /// <inheritdoc />
    public void Interact() {
        MenuManager.Instance.OpenMenu("LogReader", BuildMenuInfo());
        IInteractable.LockPlayer(player);
    }
    /// <inheritdoc />
    public void Highlight() {
        highlighPrompt.enabled = true;
    }
    /// <inheritdoc />
    public void Unhighlight() {
        highlighPrompt.enabled = false;
    }

    private MenuInfo BuildMenuInfo() {
        MenuInfo info = new MenuInfo();
        info.AddEntry("WreckType", log?.wreckType ?? "<<UNKNOWN>>");
        info.AddEntry("Content", log?.entryContent ?? "<<ERROR>>");
        return info;
    }
}