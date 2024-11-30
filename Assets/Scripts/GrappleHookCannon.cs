using UnityEngine;
using UnityEngine.InputSystem;

public class GrappleHookCannon : MonoBehaviour
{
    [SerializeField]
    private GameObject grappleHook, cable;

    private GameObject currentGrappleHook;
    private Rigidbody2D _currentHookBody;
    private Rigidbody2D currentHookBody {
        get {
            if (currentGrappleHook) {
                if (!_currentHookBody) _currentHookBody = currentGrappleHook.GetComponent<Rigidbody2D>();
            }
            else {
                _currentHookBody = null;
            }
            return _currentHookBody;
        }
    }

    [SerializeField]
    private float
        cannonTurnRange,
        cannonPower,
        cableSpeed,
        maxCableLength,
        marginBeforeBreak,
        reelingSpeed,
        maxCableTwist,
        stiffness,
        bendiness;

    [SerializeField]
    private float fireRate;

    private float nextFireTime;
    private Camera mainCamera;

    private float activeMaxCableLength;

    private bool reeling;

    // Start is called before the first frame update
    void Start() {
        mainCamera = FindFirstObjectByType<Camera>();
        SetCableLength(1);
    }

    void Update() {
        UpdateCablePositionAndLength();
        TurnGrappleCannon();
        if (reeling) {
            activeMaxCableLength -= reelingSpeed * Time.deltaTime;
        }
    }

    private void FixedUpdate() {
        if (currentGrappleHook) {
            float stretch = cable.transform.localScale.y / activeMaxCableLength;
            float twist = Vector2.SignedAngle(currentGrappleHook.transform.up, transform.up) / maxCableTwist;

            // break cable if it is too long
            // TODO recalculate break force with stress on cable
            if (stretch > 1 + marginBeforeBreak) {
                ReturnHook();
                return;
            }

            // TODO: apply pulling force depending on relative mass
            if (stretch > 1) {
                Vector2 hookToCannon = (transform.position - currentGrappleHook.transform.position).normalized;
                currentHookBody.AddForce(hookToCannon * (stretch * stretch * stiffness));
            }
            // if (stretch < 1) {
            //     Vector2 hookToCannon = (transform.position - currentGrappleHook.transform.position).normalized;
            //     currentHookBody.AddForce(-hookToCannon * (stretch * stiffness));
            // }

            // TODO: apply tangent force when angle between cable and ship is too high
            if (Mathf.Abs(twist) > 1) {
                currentHookBody.AddTorque(twist * bendiness);
            }
            if (twist < 1) {
                currentHookBody.angularVelocity *= 1 - (1 / bendiness);
            }
        }
    }

    public void FireGrappleHook(InputAction.CallbackContext context) {
        if (context.performed) {
            if (!currentGrappleHook) {
                activeMaxCableLength = maxCableLength;
                currentGrappleHook = Instantiate(grappleHook);
                currentGrappleHook.transform.position = transform.position;
                currentGrappleHook.transform.rotation = transform.rotation;
                currentGrappleHook.transform.localScale = transform.localScale;
                currentHookBody.AddForce(transform.up * cannonPower);
                currentGrappleHook.GetComponent<GrappleHook>().OnHookCollision += _ => {
                    activeMaxCableLength = cable.transform.localScale.y;
                };
            }
            else {
                if (!currentGrappleHook.GetComponent<GrappleHook>().hooked) {
                    ReturnHook();
                }
            }
        }
    }

    private void DetachHook() {
        if (currentGrappleHook) {
            currentGrappleHook.GetComponent<GrappleHook>().Detach();
        }
    }

    private void ReturnHook() {
        DetachHook();
        Destroy(currentGrappleHook);
        currentGrappleHook = null;
    }

    public void ReelHook(InputAction.CallbackContext context) {
        if (context.performed) {
            reeling = true;
        }
        if (context.canceled) {
            reeling = false;
        }
    }

    public void ReturnHook(InputAction.CallbackContext context) {
        if (context.performed) {
            ReturnHook();
        }
    }

    private void TurnGrappleCannon() {
        // turn the grapple hook cannon towards the mouse position but within the range
        if (!currentGrappleHook) {
            Vector3 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            Vector3 difference = mousePosition - transform.position;
            Vector2 direction = new Vector2(difference.x, difference.y).normalized;
            float angle = Vector2.SignedAngle(transform.parent.transform.up, direction);
            Vector3 euler = transform.localEulerAngles;
            euler.z = Mathf.Clamp(angle, -cannonTurnRange, cannonTurnRange);
            transform.localEulerAngles = euler;
        }
        else {
            float angle = AngleBetweenShipAndHook();
            Vector3 euler = transform.localEulerAngles;
            euler.z = angle;
            transform.localEulerAngles = euler;
        }
    }

    private void UpdateCablePositionAndLength() {
        if (currentGrappleHook) {
            SetCableLength(GetDistanceToGrappleHook());
        }
        else SetCableLength(1);
    }

    private float GetDistanceToGrappleHook() {
        if (currentGrappleHook) {
            Vector3 difference = currentGrappleHook.transform.position - transform.position;
            return difference.magnitude;
        }
        else return 1;
    }

    private void SetCableLength(float length) {
        Vector3 cableScale = cable.transform.localScale;
        cableScale.y = length;
        cable.transform.localScale = cableScale;
    }

    private float AngleBetweenShipAndHook() {
        Vector3 difference = currentGrappleHook.transform.position - transform.position;
        Vector2 direction = new Vector2(difference.x, difference.y).normalized;
        return Vector2.SignedAngle(transform.parent.transform.up, direction);
    }
}