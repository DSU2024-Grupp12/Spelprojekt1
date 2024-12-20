using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class Drill : Tool
{
    [SerializeField]
    private Animator drillAnimator;
    [SerializeField]
    private ParticleSystem leftParticles, rightParticles;

    public LayerMask drillLayers;

    [SerializeField, Tooltip("(Min 0)")]
    private Upgradeable damageInterval, damage;

    private Dictionary<Collider2D, float> currentlyDrilling;

    public bool drilling {
        get => drillAnimator.GetBool("drilling");
        private set => drillAnimator.SetBool("drilling", value);
    }

    private bool startedDrilling;
    public UnityEvent StartedDrilling;
    public UnityEvent StoppedDrilling;

    void Start() {
        currentlyDrilling = new();
    }

    private void Update() {
        if (currentlyDrilling.Count > 0) {
            leftParticles.Play();
            rightParticles.Play();
            if (!startedDrilling) {
                startedDrilling = true;
                StartedDrilling?.Invoke();
            }
        }
        else {
            leftParticles.Stop();
            rightParticles.Stop();
            if (startedDrilling) {
                startedDrilling = false;
                StoppedDrilling?.Invoke();
            }
        }
    }

    private void FixedUpdate() {
        List<Collider2D> toRemove = new();
        foreach (Collider2D col in currentlyDrilling.Keys) {
            if (!col) toRemove.Add(col);
        }
        foreach (Collider2D col in toRemove) {
            currentlyDrilling.Remove(col);
        }
    }

    private void OnTriggerStay2D(Collider2D other) {
        if (drillLayers.Contains(other.gameObject.layer)) {
            Hull drillingHull = other.gameObject.GetComponent<Hull>();
            if (drillingHull) {
                if (currentlyDrilling.TryGetValue(other, out float time)) {
                    if (Time.time > time + damageInterval) {
                        drillingHull.TakeDamage(damage, LayerMask.NameToLayer("Drill"));
                        currentlyDrilling[other] = Time.time;
                    }
                    Rigidbody2D body = other.gameObject.GetComponent<Rigidbody2D>();
                    body.velocity *= Mathf.Pow(0.8f, Time.fixedDeltaTime);
                    body.angularVelocity *= Mathf.Pow(0.8f, Time.fixedDeltaTime);
                }
                else {
                    currentlyDrilling.Add(other, Time.time);
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        currentlyDrilling.Remove(other);
    }

    public override void ActivateTool(InputAction.CallbackContext context) {
        if (context.performed) {
            drilling = true;
        }
        if (context.canceled) {
            drilling = false;
        }
    }
}