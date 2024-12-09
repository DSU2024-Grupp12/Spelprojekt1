using System.Collections.Generic;
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
    private float radius;

    [SerializeField, HideInInspector]
    private MassDistribution distributionFunction;

    [SerializeField, HideInInspector, Min(0)]
    private float mean, standardDeviation, minMass, maxMass;

    [HideInInspector]
    public List<Vector3> clusterPositions;

    void Start() {
        CreateAsteroids();
    }

    public void CreateClusters(ref Random random) {
        clusterPositions = new();
        for (int i = 0; i < numberOfClusters; i++) {
            Vector3 origin = GetRandomPositionInRadius(ref random, radius, Vector3.zero);
            clusterPositions.Add(origin);
            float clusterRadius = minClusterRadius + (maxClusterRadius - minClusterRadius) * random.NextFloat();
            int number = random.NextInt(minClusterChildren, maxClusterChildren);
            for (int j = 0; j < number; j++) {
                Vector3 position = GetRandomPolarCoordinate(ref random, 1f, clusterRadius, origin);
                CreateAsteroid(ref random, position);
            }
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
            Vector3 position = GetRandomPositionInRadius(ref random, radius, Vector3.zero);
            CreateAsteroid(ref random, position);
        }
    }

    public void ClearAsteroids() {
        for (int i = transform.childCount - 1; i >= 0; i--) {
            DestroyImmediate(transform.GetChild(0).gameObject);
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
                body.mass = NextGaussian(ref random, mean, standardDeviation);
                body.mass = Mathf.Clamp(body.mass, minMass, maxMass);
                break;
        }

        // adjust scale according to mass
        float massAtScale1 = asteroidPrefab.GetComponent<Rigidbody2D>().mass;
        float cubeSquareScale = Mathf.Sqrt(body.mass / massAtScale1);
        asteroid.transform.localScale = new Vector3(cubeSquareScale, cubeSquareScale, 1);

        // randomize angular velocity
        body.angularVelocity = random.NextFloat() * 10f;
    }

    private Vector3 GetRandomPositionInRadius(ref Random random, float r, Vector3 origin) {
        Vector3 startingPosition = new Vector3(-r + origin.x, -r + origin.y, 0);

        Vector3 position;
        do {
            float randomX = random.NextFloat() * radius * 2;
            float randomY = random.NextFloat() * radius * 2;
            position = startingPosition + new Vector3(randomX, randomY, 0);
        } while ((position - origin).magnitude > r);

        return position;
    }

    private Vector3 GetRandomPolarCoordinate(ref Random random,
                                             float minMagnitude,
                                             float maxMagnitude,
                                             Vector2 origin) {
        float angle = 2 * Mathf.PI * random.NextFloat();
        float magnitude = minMagnitude + (maxMagnitude - minMagnitude) * random.NextFloat();
        Vector2 coord = new Vector2(magnitude * Mathf.Cos(angle), magnitude * Mathf.Sin(angle));
        return origin + coord;
    }

    public enum MassDistribution
    {
        Linear,
        Normal
    }

    //https://stackoverflow.com/questions/218060/random-gaussian-variables
    private float NextGaussian(ref Random rand, float _mean, float stdDev) {
        // Random rand = new Random(); //reuse this if you are generating many
        double u1 = 1.0 - rand.NextDouble(); //uniform(0,1] random doubles
        double u2 = 1.0 - rand.NextDouble();
        double randStdNormal = System.Math.Sqrt(-2.0 * System.Math.Log(u1)) *
                               System.Math.Sin(2.0 * System.Math.PI * u2); //random normal(0,1)
        double randNormal =
            _mean + stdDev * randStdNormal; //random normal(mean,stdDev^2)
        return (float)randNormal;
    }
}