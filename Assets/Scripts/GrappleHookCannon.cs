using UnityEngine;
using UnityEngine.InputSystem;

public class GrappleHookCannon : MonoBehaviour
{
    [SerializeField]
    private GameObject ship, grappleHook, cable;

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

    private GrappleHook hook {
        get {
            if (currentGrappleHook) {
                return currentGrappleHook.GetComponent<GrappleHook>();
            }
            else return null;
        }
    }

    private Rigidbody2D shipBody;

    [SerializeField]
    private float
        cannonTurnRange,
        cannonPower,
        cableSpeed,
        maxCableLength,
        maxCableTwist,
        maxCableTurn,
        marginBeforeBreak,
        reelingSpeed,
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
        shipBody = ship.GetComponent<Rigidbody2D>();
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
            if (hook.hooked && GetDistanceToGrappleHook() < 0.3f) {
                ReturnHook(false);
                return;
            }

            float stretch = cable.transform.localScale.y / activeMaxCableLength;
            float twist = Vector2.SignedAngle(currentGrappleHook.transform.up, transform.up) / maxCableTwist;
            float turn = Vector2.SignedAngle(cable.transform.up, ship.transform.up) / maxCableTurn;

            ConsoleUtility.OneLineLog(stretch);

            if (!hook.hooked && stretch > 1) {
                ReturnHook(true);
                return;
            }

            // break cable if it is too long or too turned
            // TODO recalculate break force with stress on cable
            // if (stretch > 1 + marginBeforeBreak) {
            //     ReturnHook(true);
            //     return;
            // }

            float massRatioHookShip = currentHookBody.mass / ship.GetComponent<Rigidbody2D>().mass;
            if (stretch > 1) {
                Vector2 hookToCannon = (transform.position - currentGrappleHook.transform.position).normalized;
                Vector2 draggingVelocity = Vector3.Project(currentHookBody.velocity, hookToCannon);
                Vector2 pullingVelocity = Vector3.Project(shipBody.velocity, hookToCannon);

                if (Vector2.Dot(draggingVelocity, hookToCannon) < Vector2.Dot(pullingVelocity, hookToCannon)) {
                    currentHookBody.velocity += pullingVelocity * (Time.fixedDeltaTime / massRatioHookShip);
                }

                Vector2 pullingForces = Vector3.Project(ship.GetComponent<Ship>().totalForces, hookToCannon);

                Vector2 forceUnit = pullingForces / (massRatioHookShip + 1);
                currentHookBody.AddForce(forceUnit * (massRatioHookShip * stretch));
                shipBody.AddForce(-pullingForces);
                shipBody.AddForce(forceUnit / stretch);
            }

            if (reeling) {
                Vector2 hookToCannon = (transform.position - currentGrappleHook.transform.position).normalized;
                Vector2 speedTowardsCannon = Vector3.Project(currentHookBody.velocity, hookToCannon);
                if (speedTowardsCannon.magnitude < reelingSpeed) {
                    currentHookBody.velocity += hookToCannon * (reelingSpeed * Time.fixedDeltaTime);
                }
            }

            if (hook.hooked) {
                //apply tangent force when angle between cable and ship is too high
                if (Mathf.Abs(turn) > 1) {
                    Vector2 perp = Vector2.Perpendicular(transform.up);
                    currentHookBody.AddForce(perp * (turn * 50));
                    if (Vector2.Angle(transform.up, ship.transform.up) > cannonTurnRange) {
                        shipBody.angularVelocity *= (Mathf.Pow(0.5f, Time.fixedDeltaTime) / massRatioHookShip);
                    }
                }
                else {
                    Vector2 perp = Vector2.Perpendicular(ship.transform.up);
                    Vector2 perpVelocity = Vector3.Project(currentHookBody.velocity, perp);
                    if (perpVelocity.magnitude > 0.2f) {
                        currentHookBody.AddForce(-perpVelocity * turn);
                    }
                }
                Vector2 centrifugalDirection =
                    (currentHookBody.worldCenterOfMass - (Vector2)transform.position).normalized;
                Vector2 awayForces = Vector3.Project(currentHookBody.totalForce, centrifugalDirection);
                if (Vector2.Dot(awayForces, centrifugalDirection) > 0) {
                    // currentHookBody.velocity += -awayVelocity * (stretch * 30);
                    currentHookBody.AddForce(-awayForces * (currentHookBody.mass * stretch));
                }

                if (Mathf.Abs(twist) > 1) {
                    currentHookBody.AddTorque(twist * bendiness);
                    currentHookBody.angularDrag = 0;
                }
                if (Mathf.Abs(twist) < 1) {
                    currentHookBody.angularVelocity *= 0.8f;
                }
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
                currentHookBody.velocity = shipBody.velocity;
                currentHookBody.AddForce(transform.up * cannonPower);
                // hook.OnHookCollision += _ => {
                //     activeMaxCableLength = cable.transform.localScale.y;
                // };
            }
            else {
                if (!hook.hooked) {
                    ReturnHook(true);
                }
                else {
                    activeMaxCableLength = cable.transform.localScale.y;
                }
            }
        }
        if (context.canceled) {
            if (currentGrappleHook) {
                if (hook.hooked) {
                    activeMaxCableLength = maxCableLength;
                }
            }
        }
    }

    private void DetachHook(bool inheritVelocity) {
        if (currentGrappleHook) {
            hook.Detach(inheritVelocity);
        }
    }

    private void ReturnHook(bool inheritVelocity) {
        DetachHook(inheritVelocity);
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
            ReturnHook(true);
        }
    }

    private void TurnGrappleCannon() {
        // turn the grapple hook cannon towards the mouse position but within the range
        if (!currentGrappleHook) {
            Vector3 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            Vector3 difference = mousePosition - transform.position;
            Vector2 direction = new Vector2(difference.x, difference.y).normalized;
            float angle = Vector2.SignedAngle(ship.transform.up, direction);
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
        return Vector2.SignedAngle(ship.transform.up, direction);
    }
}