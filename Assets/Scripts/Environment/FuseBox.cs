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

    private void Start() {
        if (connectorPrefab) {
            foreach (SerializedInterface<IElectrical> device in devices) {
                Vector2 position = device.gameObject.transform.position;
                Vector2 to = position - (Vector2)transform.position;
                Transform connector = Instantiate(
                    connectorPrefab,
                    transform.position + Vector3.forward * 5,
                    Quaternion.FromToRotation(transform.up, to)
                );

                connector.localScale = new Vector3(1, to.magnitude, 1);

                foreach (ParticleSystem system in connector.GetComponentsInChildren<ParticleSystem>()) {
                    ParticleSystem.MainModule main = system.main;
                    main.startLifetime = connector.localScale.y;
                }

                connector.transform.SetParent(transform);
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
        Destroy(gameObject);
    }
}