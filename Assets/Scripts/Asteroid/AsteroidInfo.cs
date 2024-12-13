using UnityEngine;
using Random = Unity.Mathematics.Random;

[CreateAssetMenu(fileName = "NewAsteroid", menuName = "Asteroid Info")]
public class AsteroidInfo : ScriptableObject
{
    public Asteroid basePrefab;

    [Header("Sprite settings")]
    [SerializeField]
    private Sprite sprite;
    [SerializeField]
    private Color colorModifier = Color.white;

    [Header("Hull settings")]
    [Tooltip("The hull strength of the asteroid will be equal to its mass multiplied by this factor")]
    public float hullStrengthFactor;

    [Header("Mass Settings")]
    [SerializeField]
    private MassDistribution distribution;
    [SerializeField]
    private float
        mean,
        standardDeviation,
        minMass,
        maxMass;
    public float massAtScale1 => basePrefab.GetComponent<Rigidbody2D>().mass;

    [Header("Resource settings")]
    [SerializeField]
    private ContainedResource[] resources;

    [Header("Explosion Settings")]
    public ParticleSystem explosionPrefab;
    [Tooltip("The minimume number of child asteroids created when this asteroid is destroyed")]
    public int minExplodeInto;
    [Tooltip("The maximum number of child asteroids created when this asteroid is destroyed")]
    public int maxExplodeInto;
    [Tooltip("The different types of child asteroids that can be created when this asteroid is destroyed. " +
             "Each asteroid is equally likely to spawn")]
    public AsteroidInfo[] explodeInto;

    public Asteroid CreateAsteroid(Vector3 position, ref Random random) {
        Vector3 randomRotation = new Vector3(0, 0, random.NextFloat(360f));
        Asteroid created = Instantiate(basePrefab, position, Quaternion.Euler(randomRotation));
        created.info = this;

        // set sprite parameters
        SpriteRenderer spriteRenderer = created.GetComponentInChildren<SpriteRenderer>();
        spriteRenderer.sprite = sprite;
        spriteRenderer.color = colorModifier;

        // set resources
        ResourceContainer resourceContainer = created.GetComponent<ResourceContainer>();
        resourceContainer.resources = resources;

        // set rigidbody parameters
        created.mass = GetRandomMass(ref random, distribution);

        // adjust z postion after mass so smaller asteroids are behind larger ones
        // this hides when some asteroids appear inside other ones
        Vector3 o = created.transform.position;
        created.transform.position = new Vector3(o.x, o.y, o.z - created.mass / 100f);

        return created;
    }

    public Asteroid CreateAsteroid(Vector3 position) {
        Random random = new Random((uint)UnityEngine.Random.Range(0, int.MaxValue));
        return CreateAsteroid(position, ref random);
    }

    private float GetRandomMass(ref Random random, MassDistribution dist) {
        float mass = massAtScale1;
        switch (dist) {
            case MassDistribution.Linear:
                mass = minMass + random.NextFloat(maxMass - minMass);
                break;
            case MassDistribution.Normal:
                mass = random.NextGaussian(mean, standardDeviation);
                mass = Mathf.Clamp(mass, minMass, maxMass);
                break;
        }
        return mass;
    }

    public enum MassDistribution
    {
        Linear,
        Normal
    }
}