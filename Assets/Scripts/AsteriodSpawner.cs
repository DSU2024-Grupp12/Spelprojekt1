using UnityEngine;
using Random = Unity.Mathematics.Random;

public class AsteriodSpawner : MonoBehaviour
{
    [SerializeField, HideInInspector]
    private uint numberOfClusters;

    [SerializeField, HideInInspector]
    private float
        minClusterRadius,
        maxClusterRadius;

    [SerializeField, HideInInspector]
    private int
        minClusterChildren,
        maxClusterChildren;

    [SerializeField]
    private Asteroid asteroidPrefab;

    [SerializeField, HideInInspector]
    private uint seed, numberOfAsteroids;

    [SerializeField, HideInInspector]
    public float radius;

    [SerializeField, HideInInspector]
    private MassDistribution distributionFunction;

    [SerializeField, HideInInspector, Min(0)]
    private float mean, standardDeviation, minMass, maxMass;

    void Start() {
        CreateAsteroids();
    }

    public void ClearAsteroids() {
        for (int i = transform.childCount - 1; i >= 0; i--) {
            DestroyImmediate(transform.GetChild(0).gameObject);
        }
    }

    public void CreateAsteroids() {
        ClearAsteroids();

        Random random;
        if (seed != 0) {
            random = new Random(seed);
        }
        else {
            uint randomSeed = (uint)UnityEngine.Random.Range(1, int.MaxValue);
            random = new Random(randomSeed);
        }

        CreateClusters(ref random);

        for (int i = 0; i < numberOfAsteroids; i++) {
            Vector3 position = random.GetNextPositionInRadius(radius, Vector3.zero);
            CreateAsteroid(ref random, position);
        }
    }

    public void CreateClusters(ref Random random) {
        for (int i = 0; i < numberOfClusters; i++) {
            Vector3 origin = random.GetNextPositionInRadius(radius, Vector3.zero);
            float clusterRadius = minClusterRadius + (maxClusterRadius - minClusterRadius) * random.NextFloat();
            int number = random.NextInt(minClusterChildren, maxClusterChildren);
            for (int j = 0; j < number; j++) {
                Vector3 position = random.GetNextPolarCoordinate(1f, clusterRadius, origin);
                CreateAsteroid(ref random, position);
            }
        }
    }

    private void CreateAsteroid(ref Random random, Vector3 position) {
        Asteroid asteroid = Instantiate(asteroidPrefab, transform, true);
        asteroid.transform.position = position;

        // randomize rotation
        asteroid.transform.eulerAngles = new Vector3(0, 0, random.NextFloat() * 360);

        // randomize mass
        Rigidbody2D body = asteroid.GetComponent<Rigidbody2D>();

        switch (distributionFunction) {
            case MassDistribution.Linear:
                body.mass = minMass + random.NextFloat() * (maxMass - minMass);
                break;
            case MassDistribution.Normal:
                body.mass = random.NextGaussian(mean, standardDeviation);
                body.mass = Mathf.Clamp(body.mass, minMass, maxMass);
                break;
        }

        // adjust scale according to mass
        float massAtScale1 = asteroidPrefab.GetComponent<Rigidbody2D>().mass;
        float cubeSquareScale = Mathf.Sqrt(body.mass / massAtScale1);
        asteroid.transform.localScale = new Vector3(cubeSquareScale, cubeSquareScale, 1);

        // randomize angular velocity
        body.angularVelocity = -10 + random.NextFloat() * 20f;
    }

    public enum MassDistribution
    {
        Linear,
        Normal
    }
}