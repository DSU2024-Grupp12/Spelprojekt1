using System.Collections.Generic;
using UnityEngine;
using Random = Unity.Mathematics.Random;

public class AsteriodSpawner : MonoBehaviour
{
    [SerializeField]
    private bool recreateAsteroidsOnStart;

    [SerializeField]
    private AsteroidVariant[] variants;
    private Dictionary<AsteroidVariant, float> weightTable;
    private List<(float, AsteroidVariant)> probabilityTable;

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

    [SerializeField, HideInInspector]
    private uint seed, numberOfAsteroids;

    [SerializeField, HideInInspector]
    public float radius;

    void Awake() {
        if (recreateAsteroidsOnStart) CreateAsteroids();
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
        GenerateTables();

        AsteroidVariant variant = MathExtensions.GetRandomEntryByWeight(probabilityTable, ref random);
        Asteroid asteroid = variant.asteroidInfo.CreateAsteroid(position, ref random);
        asteroid.transform.SetParent(transform);
    }

    private void GenerateTables() {
        weightTable = new();
        probabilityTable = new();
        foreach (AsteroidVariant variant in variants) {
            weightTable.Add(variant, variant.weight);
        }
        probabilityTable = MathExtensions.GenerateProbabiltyTable(weightTable);
    }
}

[System.Serializable]
public class AsteroidVariant
{
    public AsteroidInfo asteroidInfo;
    public float weight;
}