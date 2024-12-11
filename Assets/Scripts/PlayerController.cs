using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Ship))]
public class PlayerController : MonoBehaviour
{
    private Ship playerShip;

    [SerializeField]
    private Tool defaultPrimaryTool;

    private Tool primaryTool;
    private Tool secondaryTool;

    [SerializeField]
    private Transform toolMount;

    void Start() {
        playerShip = GetComponent<Ship>();
        if (!transform.GetComponentInChildren<Tool>()) {
            primaryTool = Instantiate(defaultPrimaryTool, toolMount, false);
        }
    }

    public void Accelerate(InputAction.CallbackContext context) {
        playerShip.accelerating = context.ReadValueAsButton();
    }

    public void Deaccelerate(InputAction.CallbackContext context) {
        playerShip.deaccelerating = context.ReadValueAsButton();
    }

    public void TurnClockwise(InputAction.CallbackContext context) {
        playerShip.turningClockwise = context.ReadValueAsButton();
    }

    public void TurnCounterClockwise(InputAction.CallbackContext context) {
        playerShip.turningCounterClockwise = context.ReadValueAsButton();
    }

    public void StrafeStarBoard(InputAction.CallbackContext context) {
        playerShip.strafingStarBoard = context.ReadValueAsButton();
    }

    public void StrafePort(InputAction.CallbackContext context) {
        playerShip.strafingPort = context.ReadValueAsButton();
    }

    public void Boost(InputAction.CallbackContext context) {
        playerShip.boosting = context.ReadValueAsButton();
    }

    public void Stop(InputAction.CallbackContext context) {
        playerShip.stopping = context.ReadValueAsButton();
    }

    public void PrimaryActivateTool(InputAction.CallbackContext context) {
        if (context.performed) {
            primaryTool.ActivateTool();
        }
    }

    public void SecondaryActivateTool(InputAction.CallbackContext context) {
        if (context.performed) {
            if (secondaryTool) secondaryTool.ActivateTool();
        }
    }

    public void CancelTool(InputAction.CallbackContext context) {
        if (context.performed) {
            if (primaryTool) primaryTool.Cancel();
            if (secondaryTool) secondaryTool.Cancel();
        }
    }
}