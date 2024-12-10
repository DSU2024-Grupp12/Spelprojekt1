using UnityEngine;

public abstract class Tool : MonoBehaviour
{
    protected Transform mount => transform.parent.transform;
    protected Rigidbody2D shipBody => GetComponentInParent<Rigidbody2D>();

    public abstract void ActivateTool();
}