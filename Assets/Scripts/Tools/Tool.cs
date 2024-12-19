using UnityEngine;
using UnityEngine.InputSystem;

public abstract class Tool : MonoBehaviour
{
    protected Transform mount => transform.parent.transform;
    protected Rigidbody2D shipBody => GetComponentInParent<Rigidbody2D>();

    public abstract void ActivateTool(InputAction.CallbackContext context);
    public virtual void Cancel() { }
    public virtual void Hide() { }
    public virtual void Unhide() { }
}