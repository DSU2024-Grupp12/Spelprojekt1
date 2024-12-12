using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class ResourceContainer : MonoBehaviour
{
    public ContainedResource[] resources;

    public void BreakContainer() {
        float tenthOfMass = GetComponent<Rigidbody2D>().mass / 10f;
        foreach (ContainedResource resource in resources) {
            Debug.Log(resource.minNumber);
            // get random number of dropped units based on mass but no smaller than minNumber and no larger than maxNumber
            int low = (int)Mathf.Floor(resource.minDroppedPer10UnitMass * tenthOfMass);
            int high = (int)Mathf.Ceil(resource.maxDroppedPer10UnitMass * tenthOfMass);
            int number = Mathf.Clamp(Random.Range(low, high + 1), resource.minNumber, resource.maxNumber);

            for (int i = 0; i < number; i++) {
                Vector2 position = MathExtensions.GetRandomPolarCoordinate(0.3f, 0.5f, transform.position);
                Resource r = Instantiate(resource.resource, position, Quaternion.identity);

                // get random value based on mass but no smaller than minValue and no larger than maxValue
                int rLow = (int)Mathf.Floor(resource.maxValuePer10UnitMass * tenthOfMass);
                int rHigh = (int)Mathf.Ceil(resource.maxValuePer10UnitMass * tenthOfMass);
                int value = Random.Range(rLow, rHigh + 1);

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

    [Tooltip("The number of units dropped will never be lower than this value no matter how small the body is")]
    public int minNumber = 1;
    [Tooltip("The number of units dropped will never exceed this value no matter how large the body is")]
    public int maxNumber = 1;
    [Tooltip("The value of the units dropped will never be lower than this no matter how small the body is"), Min(1)]
    public int minValue = 1;
    [Tooltip("The value of the units dropped will never exceed this value no matter how large the body is"), Min(1)]
    public int maxValue = 1;
}