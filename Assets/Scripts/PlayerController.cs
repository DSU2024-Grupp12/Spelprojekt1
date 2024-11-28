using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Ship))]
public class PlayerController : MonoBehaviour
{
    private Ship playerShip;

    void Start() {
        playerShip = GetComponent<Ship>();
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
}