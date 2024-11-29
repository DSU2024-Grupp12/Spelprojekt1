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
}