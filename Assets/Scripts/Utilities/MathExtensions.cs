using UnityEngine;
using Random = Unity.Mathematics.Random;

public static class MathExtensions
{
    public static bool Between(this float f, float min, float max, BetweenMode mode = BetweenMode.InclusiveInclusive) {
        switch (mode) {
            case BetweenMode.ExclusiveExclusive:
                return f > min && f < max;
            case BetweenMode.ExclusiveInclusive:
                return f > min && f <= max;
            case BetweenMode.InclusiveExclusive:
                return f >= min && f < max;
            case BetweenMode.InclusiveInclusive:
                return f >= min && f <= max;
            default: return false;
        }
    }

    public enum BetweenMode
    {
        ExclusiveExclusive,
        ExclusiveInclusive,
        InclusiveExclusive,
        InclusiveInclusive
    }

    public static Vector3 GetNextPositionInRadius(ref this Random random, float r, Vector3 origin) {
        Vector3 startingPosition = new Vector3(-r + origin.x, -r + origin.y, 0);

        Vector3 position;
        do {
            float randomX = random.NextFloat() * r * 2;
            float randomY = random.NextFloat() * r * 2;
            position = startingPosition + new Vector3(randomX, randomY, 0);
        } while ((position - origin).magnitude > r);

        return position;
    }

    public static Vector2 GetNextPolarCoordinate(ref this Random random,
                                                 float minMagnitude,
                                                 float maxMagnitude,
                                                 Vector2 origin) {
        float angle = 2 * Mathf.PI * random.NextFloat();
        float magnitude = minMagnitude + (maxMagnitude - minMagnitude) * random.NextFloat();
        Vector2 coord = new Vector2(magnitude * Mathf.Cos(angle), magnitude * Mathf.Sin(angle));
        return origin + coord;
    }

    public static Vector2 GetRandomPolarCoordinate(float minMagintude, float maxMagnitude, Vector2 origin) {
        Random random = new((uint)UnityEngine.Random.Range(0, int.MaxValue));
        return GetNextPolarCoordinate(ref random, minMagintude, maxMagnitude, origin);
    }

    //https://stackoverflow.com/questions/218060/random-gaussian-variables
    public static float NextGaussian(ref this Random rand, float _mean, float stdDev) {
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