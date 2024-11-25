using System;
using UnityEngine;
using Random = Unity.Mathematics.Random;

public class AsteriodSpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject asteroidPrefab;

    [SerializeField, HideInInspector]
    private uint seed, numberOfAsteroids;

    [SerializeField, HideInInspector]
    private float width, height;

    [SerializeField, HideInInspector]
    private MassDistribution distributionFunction;

    [SerializeField, HideInInspector, Min(0)]
    private float mean, standardDeviation, minMass, maxMass;

    void Start() {
        CreateAsteroids();
    }

    public void CreateAsteroids() {
        ClearAsteroids();

        Vector3 startingPosition = new Vector3(-width / 2, -height / 2, 0);
        Random randomPosition;
        Random randomMass;
        if (seed != 0) {
            randomPosition = new Random(seed);
            randomMass = new Random(seed);
        }
        else {
            uint randomSeed = (uint)UnityEngine.Random.Range(1, int.MaxValue);
            randomPosition = new Random(randomSeed);
            randomMass = new Random(randomSeed);
        }

        for (int i = 0; i < numberOfAsteroids; i++) {
            GameObject asteroid = Instantiate(asteroidPrefab, transform, true);
            
            // randomize position
            float randomX = randomPosition.NextFloat() * width;
            float randomY = randomPosition.NextFloat() * height;
            asteroid.transform.position = startingPosition + new Vector3(randomX, randomY, 900);

            // randomize rotation
            asteroid.transform.eulerAngles = new Vector3(0, 0, randomPosition.NextFloat() * 360);

            // randomize mass
            Rigidbody2D body = asteroid.GetComponent<Rigidbody2D>();

            switch (distributionFunction) {
                case MassDistribution.Linear:
                    body.mass = minMass + randomMass.NextFloat() * (maxMass - minMass);
                    break;
                case MassDistribution.Normal:
                    body.mass = NextGaussian(ref randomMass, mean, standardDeviation);
                    body.mass = Mathf.Clamp(body.mass, minMass, maxMass);
                    break;
            }

            // adjust scale according to mass
            float massAtScale1 = asteroidPrefab.GetComponent<Rigidbody2D>().mass;
            float cubeSquareScale = Mathf.Sqrt(body.mass / massAtScale1);
            asteroid.transform.localScale = new Vector3(cubeSquareScale, cubeSquareScale, 1);
        }
    }

    public void ClearAsteroids() {
        for (int i = transform.childCount - 1; i >= 0; i--) {
            DestroyImmediate(transform.GetChild(0).gameObject);
        }
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
        double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) *
                               Math.Sin(2.0 * Math.PI * u2); //random normal(0,1)
        double randNormal =
            _mean + stdDev * randStdNormal; //random normal(mean,stdDev^2)
        return (float)randNormal;
    }
}