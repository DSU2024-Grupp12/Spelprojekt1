using UnityEngine;
using UnityEngine.InputSystem;
using CallbackContext = UnityEngine.InputSystem.InputAction.CallbackContext;

[RequireComponent(typeof(Ship))]
public class PlayerController : MonoBehaviour
{
    private Ship playerShip;

    [SerializeField]
    private Tool
        defaultPrimaryTool,
        defaultSecondaryTool;

    private Tool primaryTool;
    private Tool secondaryTool;

    [SerializeField]
    private Transform toolMount;

    [SerializeField]
    private string onDeathSceneLoadName;
    [SerializeField]
    private float onDeathSceneLoadDelay;

    private void Awake() {
        PlayerReference.SetPlayer(gameObject);
    }

    void Start() {
        playerShip = GetComponent<Ship>();
        if (defaultPrimaryTool) primaryTool = Instantiate(defaultPrimaryTool, toolMount, false);
        if (defaultSecondaryTool) secondaryTool = Instantiate(defaultSecondaryTool, toolMount, false);
    }

    public void Accelerate(CallbackContext context) {
        playerShip.accelerating = context.ReadValueAsButton();
    }

    public void Deaccelerate(CallbackContext context) {
        playerShip.deaccelerating = context.ReadValueAsButton();
    }

    public void TurnClockwise(CallbackContext context) {
        playerShip.turningClockwise = context.ReadValueAsButton();
    }

    public void TurnCounterClockwise(CallbackContext context) {
        playerShip.turningCounterClockwise = context.ReadValueAsButton();
    }

    public void StrafeStarBoard(CallbackContext context) {
        playerShip.strafingStarBoard = context.ReadValueAsButton();
    }

    public void StrafePort(CallbackContext context) {
        playerShip.strafingPort = context.ReadValueAsButton();
    }

    public void Boost(CallbackContext context) {
        playerShip.boosters.boosting = context.ReadValueAsButton();
    }

    public void Stop(CallbackContext context) {
        playerShip.stopping = context.ReadValueAsButton();
    }

    public void PrimaryActivateTool(CallbackContext context) {
        primaryTool.Unhide();
        primaryTool.ActivateTool(context);
        if (secondaryTool) {
            secondaryTool.Hide();
        }
    }

    public void SecondaryActivateTool(CallbackContext context) {
        if (secondaryTool) {
            primaryTool.Hide();
            secondaryTool.Unhide();
            secondaryTool.ActivateTool(context);
        }
    }

    public void CancelTool(CallbackContext context) {
        if (context.performed) {
            if (primaryTool) primaryTool.Cancel();
            if (secondaryTool) secondaryTool.Cancel();
        }
    }

    public void HoldCheatSheet(CallbackContext context) {
        if (context.performed) {
            MenuManager.Instance.ToggleMenuAsOverlay("CheatSheet", true);
        }
        if (context.canceled) {
            MenuManager.Instance.ToggleMenuAsOverlay("CheatSheet", false);
        }
    }

    public void LoadOnDeathScene() {
        ApplicationHandler.ChangeSceneDelayed(onDeathSceneLoadName, onDeathSceneLoadDelay);
    }

    public void ReturnToGameplay(CallbackContext context) {
        if (context.performed) {
            MenuManager.Instance.ReturnToGameplay();
        }
    }
}