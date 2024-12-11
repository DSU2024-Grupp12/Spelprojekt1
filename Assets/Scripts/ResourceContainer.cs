using UnityEngine;

public class ResourceContainer : MonoBehaviour
{
    public ContainedResource[] resources;

    public void BreakContainer() {
        foreach (ContainedResource resource in resources) {
            int number = Random.Range(resource.minDroppedNumber, resource.maxDroppedNumber);
            for (int i = 0; i < number; i++) {
                Vector2 position = MathExtensions.GetRandomPolarCoordinate(0.3f, 0.5f, transform.position);
                Resource r = Instantiate(resource.resource, position, Quaternion.identity);
                r.value = Random.Range(resource.minValue, resource.maxValue);
            }
        }
    }
}

[System.Serializable]
public class ContainedResource
{
    public Resource resource;

    public int minDroppedNumber;
    public int maxDroppedNumber;

    [Min(1)]
    public int minValue, maxValue;
}