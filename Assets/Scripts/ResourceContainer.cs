using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class ResourceContainer : MonoBehaviour
{
    public ContainedResource[] resources;

    private Rigidbody2D body;

    public void Start() {
        body = GetComponent<Rigidbody2D>();
    }

    public void BreakContainer() {
        float tenthOfMass = body.mass / 10f;
        foreach (ContainedResource resource in resources) {
            // get random number of dropped units based on mass but no smaller than minNumber and no larger than maxNumber
            int low = (int)Mathf.Floor(resource.minDroppedPer10UnitMass * tenthOfMass);
            int high = (int)Mathf.Ceil(resource.maxDroppedPer10UnitMass * tenthOfMass);
            int number = Mathf.Clamp(Random.Range(low, high), resource.minNumber, resource.maxNumber);

            Debug.Log($"low: {low}, high: {high}, number: {number}");

            for (int i = 0; i < number; i++) {
                Vector2 position = MathExtensions.GetRandomPolarCoordinate(0.3f, 0.5f, transform.position);
                Resource r = Instantiate(resource.resource, position, Quaternion.identity);

                // get random value based on mass but no smaller than minValue and no larger than maxValue
                int rLow = (int)Mathf.Floor(resource.maxValuePer10UnitMass * tenthOfMass);
                int rHigh = (int)Mathf.Ceil(resource.maxValuePer10UnitMass * tenthOfMass);
                int value = Random.Range(rLow, rHigh);

                Debug.Log($"rlow: {rLow}, rhigh: {rHigh}, value: {value}");

                r.value = Mathf.Clamp(value, resource.minValue, resource.maxValue);
            }
        }
    }
}

[System.Serializable]
public class ContainedResource
{
    public Resource resource;

    [Min(0)]
    public float minDroppedPer10UnitMass, maxDroppedPer10UnitMass;
    [Min(0)]
    public float minValuePer10UnitMass, maxValuePer10UnitMass;

    public int
        minNumber,
        maxNumber,
        minValue,
        maxValue;
}