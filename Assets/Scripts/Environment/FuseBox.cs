using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Hull))]
public class FuseBox : MonoBehaviour
{
    [SerializeField]
    private ParticleSystem explosionPrefab;

    [SerializeField]
    private Transform connectorPrefab;

    [SerializeField]
    private SerializedInterface<IElectrical>[] devices;

    private List<Transform> connectors;

    private void Start() {
        connectors = new();
        if (connectorPrefab) {
            foreach (SerializedInterface<IElectrical> device in devices) {
                Vector2 position = device.gameObject.transform.position;
                Vector2 to = position - (Vector2)transform.position;
                Transform connector = Instantiate(
                    connectorPrefab,
                    transform.position + Vector3.forward * 5,
                    Quaternion.FromToRotation(Vector3.up, to.normalized)
                );

                connector.localScale = new Vector3(1, to.magnitude, 1);

                foreach (ParticleSystem system in connector.GetComponentsInChildren<ParticleSystem>()) {
                    ParticleSystem.MainModule main = system.main;
                    main.startLifetime = connector.localScale.y;
                    system.Play();
                }

                connectors.Add(connector);
            }
        }
    }

    public void Explode() {
        foreach (SerializedInterface<IElectrical> device in devices) {
            device.extract.Disable();
        }
        if (explosionPrefab) {
            Vector3 explosionPosition = new Vector3(transform.position.x, transform.position.y, -3);
            Instantiate(explosionPrefab, explosionPosition, transform.rotation)
                .transform.localScale = transform.localScale;
        }
        foreach (Transform connector in connectors) {
            Destroy(connector.gameObject);
        }
        Destroy(gameObject);
    }
}